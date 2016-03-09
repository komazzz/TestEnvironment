using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CspaTestModel.Model;
using OPCDA;
using OPC;
using OPCDA.NET;
using CspaTestModel;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ProcessDataProviders.OpcDa
{
    [DataContract(IsReference = true)]
   // [KnownType(typeof(OpcDaProcessDataProvider))]

    public class OpcDaProcessDataProvider:IProcessDataProvider
    {
        OpcServer OpcServer { get; set; }
        SyncIOGroup SyncGroup { get; set; }
        RefreshGroup RefreshGroup { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public string OpcName { get; set; }

        [DataMember]
        public int UpdateRate { get; set; }

        private object locker = new object();
        private Task ItemsUpdateTask { get; set; }
        private Queue<ProcessItemUpdateElement> ItemUpdateQueue = new Queue<ProcessItemUpdateElement>();
        private Dictionary<string, IProcessItemDefinition> itemStorage = new Dictionary<string, IProcessItemDefinition>();
         
       public static Type[] GetTypes()
        {
            return new Type[] { typeof(OpcDaProcessDataProvider) };
        }

        public void Register(IProcessItemDefinition ProcessItemDef)
        {
            if (!itemStorage.ContainsKey(ProcessItemDef.Address))
                itemStorage.Add(ProcessItemDef.Address,ProcessItemDef);
        }

        public void Deregister(IProcessItemDefinition ProcessItemDef)
        {
            if (itemStorage.ContainsKey(ProcessItemDef.Address))
                itemStorage.Remove(ProcessItemDef.Address);
        }

        public bool IsRegistered(IProcessItemDefinition ProcessItemDef)
        {
            return itemStorage.ContainsKey(ProcessItemDef.Address);
        }

        public IEnumerable<IProcessItemDefinition> Items
        {
            get { return itemStorage.Values; }
        }

        public IProcessItemValue ReadItem(string ItemAddress)
        {
            if (!this.IsConnected)
                Connect();
            OPCItemState itemState = null;
            int ret = SyncGroup.Read(OPCDATASOURCE.OPC_DS_CACHE, ItemAddress,out itemState);
            var piv = CreatetProcessItemValue(ItemAddress, itemState);
            return piv;
        }

        public void WriteItem(string ItemAddress, object ItemValue)
        {
            if (!this.IsConnected)
                Connect();
            int ret = SyncGroup.Write(ItemAddress, ItemValue);
            if (HRESULTS.Failed(ret))
                throw new ProcessDataProviderException(ErrorDescriptions.GetErrorDescription(ret));
        }

        public bool IsConnected
        {
            get
            {
                if (OpcServer != null)
                    return OpcServer.isConnectedDA;
                else
                    return false;
            }
        }

        public void Connect()
        {
            try
            {
                if (OpcServer == null)
                    OpcServer = new OpcServer();

                OPC.Common.Host hostInfo = GetHostInfo();
                int ret = OpcServer.Connect(hostInfo, OpcName);
                if (HRESULTS.Failed(ret))
                    throw new ProcessDataProviderException( ErrorDescriptions.GetErrorDescription(ret));

                SyncGroup = new SyncIOGroup(OpcServer, UpdateRate);
                RefreshGroup = new RefreshGroup(OpcServer, UpdateRate, OnDataRefresh);
                
                string[] itemAdresses;
                lock (itemStorage)
                    itemAdresses = itemStorage.Keys.ToArray();

                AddItemsToRefreshGroup(itemAdresses);
                ReadItemsInRefreshGroup(itemAdresses);

            }
            catch(Exception e)
            {
                Disconnect();
                throw;
            }
        }

        public void Disconnect()
        {
            if (OpcServer == null)
                return;

            if (RefreshGroup != null)
            {
                RefreshGroup.OpcGrp.Remove(false);
                RefreshGroup = null;
            }
            if (SyncGroup != null)
            {
                SyncGroup.OpcGrp.Remove(false);
                SyncGroup = null;
            }

            OpcServer.Disconnect();
            OpcServer.Logoff();
            OpcServer = null;
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void AddItemsToRefreshGroup(String[] ItemAddresses)
        {
            if (ItemAddresses.GetLength(0) > 0)
            {
                int[] errCodes;
                int ret = RefreshGroup.Add(ItemAddresses, out errCodes);
                if(!HRESULTS.Succeeded(ret))
                {
                    StringBuilder strb = new StringBuilder();
                    strb.AppendFormat("При подписке произошла ошибка: {0}\n", ErrorDescriptions.GetErrorDescription(ret));

                    for (int i = 0; i < errCodes.Length; i++)
                    {
                        if (HRESULTS.Failed(errCodes[i]))
                            strb.AppendFormat("Тег {0} \t Ошибка: {1}", ItemAddresses[i], ErrorDescriptions.GetErrorDescription(errCodes[i]));
                    }
                    throw new ProcessDataProviderException(strb.ToString());
                }
            }
        }

        private void ReadItemsInRefreshGroup(String[] ItemAddresses)
        {
            int transactionID = 1111;
            StringBuilder errDescriptors = null;
            if (ItemAddresses.Length > 0)
                foreach (var item in ItemAddresses)
                {
                    int ret = RefreshGroup.Read(item, transactionID);
                    if(!HRESULTS.Succeeded(ret))
                    {
                        if (errDescriptors == null)
                            errDescriptors = new StringBuilder();
                        errDescriptors.AppendFormat("Тег {0} - ошибка: {1}\n", item, ErrorDescriptions.GetErrorDescription(ret));
;                    }
                }
            if (errDescriptors != null)
                throw new ProcessDataProviderException(errDescriptors.ToString());
        }

        protected void OnDataRefresh(object s, RefreshEventArguments args)
        {
            lock (locker)
            {
                if (args.Reason == RefreshEventReason.DataChanged)
                {
                    var refreshedItems = args.items;
                    foreach (var item in refreshedItems)
                    {
                        IProcessItemDefinition pid;
                        if (itemStorage.TryGetValue(item.OpcIDef.ItemID, out pid))
                        {
                            var updElement = new ProcessItemUpdateElement();

                            updElement.ProcessItemDefinition = pid;
                            updElement.ProcessItemValue = CreatetProcessItemValue(pid.Address, item.OpcIRslt);

                            lock (ItemUpdateQueue)
                                ItemUpdateQueue.Enqueue(updElement);
                        }
                    }
                    RunUpdateTask();
                }
            }
        }

        protected IProcessItemValue CreatetProcessItemValue(string ItemAddress, OPCItemState OpcIRslt)
        {
            ProcessItemValue piv = new ProcessItemValue(ItemAddress);
            piv.ErrorOccured = !HRESULTS.Succeeded(OpcIRslt.Error);
            if (piv.ErrorOccured)
            {
                piv.ErrorDescription = ErrorDescriptions.GetErrorDescription(OpcIRslt.Error);
                piv.Quality = 0;
                piv.TimeStamp = DateTime.UtcNow;
            }
            else
            {
                piv.Quality = OpcIRslt.Quality;
                piv.TimeStamp = OpcIRslt.TimeStampNet;
                piv.Value = OpcIRslt.DataValue;
            }

            return piv;
        }

        protected void RunUpdateTask()
        {
            if ((ItemsUpdateTask == null) ||
                (ItemsUpdateTask.Status == TaskStatus.Faulted) ||
                (ItemsUpdateTask.Status == TaskStatus.RanToCompletion) ||
                (ItemsUpdateTask.Status == TaskStatus.Canceled)
                )
                ItemsUpdateTask = Task.Factory.StartNew(UpdateAction);
            else
                ItemsUpdateTask.ContinueWith((t)=>UpdateAction());
        }

        protected void UpdateAction()
        {
            System.Threading.Thread.CurrentThread.Name = "OpcDataProviderUpdateThread";
            ProcessItemUpdateElement[] itemUpdateArray;
            lock(ItemUpdateQueue)
            {
                if(ItemUpdateQueue.Count == 0)
                    return;
                itemUpdateArray = ItemUpdateQueue.ToArray();
                ItemUpdateQueue.Clear();
            }
            foreach(var item in itemUpdateArray )
                item.ProcessItemDefinition.UpdateState(item.ProcessItemValue);
        }
        
        protected OPC.Common.Host GetHostInfo()
        {
            OPC.Common.Host hostInfo = new OPC.Common.Host();
            hostInfo.Domain = Domain;
            hostInfo.HostName = Host;
            hostInfo.UserName = User;
            hostInfo.Password = Password;
            return hostInfo;
        }

        private readonly string _providerName = "Data Provider OpcDA 2.05";
        public string ProviderName
        {
            get { return _providerName; }
        }


        private readonly Guid _providerId = new Guid("C8B77C2D-D5A8-4D39-9962-C661DC1F200B");
        public Guid ProviderId
        {
            get { return _providerId; }
        }
    }



    struct ProcessItemUpdateElement
    {
        public IProcessItemDefinition ProcessItemDefinition;
        public IProcessItemValue ProcessItemValue;
    }
}
