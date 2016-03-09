using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model.Protections;
using CspaTestModel.Model;
using CspaTestModel;
using System.Runtime.Serialization;
using System.IO;
using CspaTestModel.Protections;

namespace CspaEnvironment
{
    public class VlvManager
    {
        public VlvManager(ProcessItemManager ProcessItemManager)
        {
            this.ProcessItemManager = ProcessItemManager;
        }
        public ProcessItemManager ProcessItemManager { get; private set; }


        Dictionary<int, IVlv> vlvStorage = new Dictionary<int, IVlv>();
        public IVlv GetVlv(int VlvId)
        {
            IVlv retVal = null;
            vlvStorage.TryGetValue(VlvId, out retVal);
            return retVal;
        }

        public void AddVlv(IVlv Vlv)
        {
            if (vlvStorage.ContainsKey(Vlv.Index))
                throw new ArgumentException("Уже есть задвижка с таким ID");
            vlvStorage.Add(Vlv.Index, Vlv);
        }

        public IEnumerable<IVlv> Vlvs { get { return vlvStorage.Values.OrderBy(vlv => vlv.Index); } }

        public void CreateVlv(VlvDefinition VlvDef, IProcessDataProvider DataProvider) 
        {
            try
            {
                CheckVlvDef(VlvDef);
                CreateProcessItems(VlvDef, DataProvider);
                CreateVlvObject(VlvDef);
            }
            catch(Exception e)
            {
                throw new ArgumentException("Не могу создать задвижку.", e);
            }
        }

        public void ImportXml(String FilePath, IProcessDataProvider DataProvider)
        {
            VlvDefinitions defs = null;
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                var dcs = new DataContractSerializer(typeof(VlvDefinitions));
                defs = (VlvDefinitions)dcs.ReadObject(fs);
            }

            StringBuilder errorString = new StringBuilder();
            foreach(VlvDefinition def in defs.VlvDefs)
            {
                try
                {
                    CreateVlv(def, DataProvider);
                }
                catch(Exception e)
                {
                    errorString.AppendFormat("Ошибка импорта задвижки id:{0} name:{1} - {2}",def.Index, def.Name, e.Message);   
                }
            }

            if(errorString.Length>0)
                throw new ArgumentException(errorString.ToString());
        }

        private void CheckVlvDef(VlvDefinition VlvDef)
        {
            if (VlvDef == null)
                throw new NullReferenceException("VlvDef == null");

            if(VlvDef.Index>0)
            {
                if (vlvStorage.ContainsKey(VlvDef.Index))
                    throw new ArgumentException("Задвижка с таким Id уже существует");
            }

            if (String.IsNullOrWhiteSpace(VlvDef.Name))
                throw new ArgumentException("Имя задвижки некорректно.");

            if (String.IsNullOrWhiteSpace(VlvDef.MaskStateTag))
                throw new ArgumentException("Адрес тега некорректен.");

            if (String.IsNullOrWhiteSpace(VlvDef.PositionQualityTag))
                throw new ArgumentException("Адрес тега некорректен.");

            if (String.IsNullOrWhiteSpace(VlvDef.PositionTag))
                throw new ArgumentException("Адрес тега некорректен.");

            if (String.IsNullOrWhiteSpace(VlvDef.SetMaskOffTag))
                throw new ArgumentException("Адрес тега некорректен.");

            if (String.IsNullOrWhiteSpace(VlvDef.SetMaskOnTag))
                throw new ArgumentException("Адрес тега некорректен.");
        }

        private void CreateProcessItems(VlvDefinition VlvDef, IProcessDataProvider DataProvider)
        {
            CreateItemIfNotExists(VlvDef.MaskStateTag, typeof(bool), DataProvider);
            CreateItemIfNotExists(VlvDef.PositionQualityTag, typeof(Int32), DataProvider);
            CreateItemIfNotExists(VlvDef.PositionTag, typeof(Int32), DataProvider);
            CreateItemIfNotExists(VlvDef.SetMaskOffTag, typeof(bool), DataProvider);
            CreateItemIfNotExists(VlvDef.SetMaskOnTag, typeof(bool), DataProvider);
        }

        private void CreateItemIfNotExists(string ItemAddress, Type ItemType, IProcessDataProvider DataProvider)
        {
            if (!ProcessItemManager.ContainsItem(ItemAddress))
                ProcessItemManager.CreateProcessItem(ItemAddress, ItemType, DataProvider);
            else
            {
                var itemDef = ProcessItemManager.GetProcessItemDefinition(ItemAddress);
                if (itemDef.Type != ItemType)
                    throw new ArgumentException(String.Format("Айтем с именем {0} уже создан с другим типом {1}", ItemAddress, itemDef.Type));
            }
        }

        private void CreateVlvObject(VlvDefinition VlvDef)
        {
            int vlvIndex;
            if (VlvDef.Index > 0)
                vlvIndex = VlvDef.Index;
            else
                vlvIndex = GetNewVlvIndex();
            VLV vlv = new VLV(vlvIndex, VlvDef.Name);
            vlv.MaskStateTag = ProcessItemManager.GetBoolProcessItem(VlvDef.MaskStateTag);
            vlv.PositionQualityTag = ProcessItemManager.GetIntProcessItem(VlvDef.PositionQualityTag);
            vlv.PositionTag = ProcessItemManager.GetIntProcessItem(VlvDef.PositionTag);
            vlv.SetMaskOffTag = ProcessItemManager.GetBoolProcessItem(VlvDef.SetMaskOffTag);
            vlv.SetMaskOnTag = ProcessItemManager.GetBoolProcessItem(VlvDef.SetMaskOnTag);

            vlvStorage.Add(vlv.Index, vlv);
        }

        private int GetNewVlvIndex()
        {
            int[] sortedIndexes = vlvStorage.Keys.OrderBy(index=>index).ToArray();

            if (sortedIndexes.Length == 0)
                return 1;

            if (sortedIndexes.Length <2)
                return sortedIndexes[0]+1;            
                
            for(int i=1; i<sortedIndexes.Length;i++)
            {
                int di = sortedIndexes[i] - sortedIndexes[i-1];
                if (di > 1)
                    return sortedIndexes[i - 1] + 1;
            }

            return sortedIndexes.Last() + 1;
        }


    }

    [DataContract]
    public class VlvDefinition
    {
        [DataMember]
        public int Index { get; set; }
        
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string PositionTag { get; set; }
        
        [DataMember]
        public string PositionQualityTag { get; set; }
        
        [DataMember]
        public string MaskStateTag { get; set; }
        
        [DataMember]
        public string SetMaskOnTag { get; set; }
     
        [DataMember]
        public string SetMaskOffTag { get; set; }
    }

    [DataContract]
    public class VlvDefinitions
    {
        [DataMember]
        public VlvDefinition[] VlvDefs { get; set; }
    }
}
