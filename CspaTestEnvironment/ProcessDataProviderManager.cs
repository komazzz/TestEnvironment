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
    public class ProcessDataProviderManager
    {
        public void AddDataProvider(IProcessDataProvider DataProvider)
        {
            providerStorage.Add(DataProvider.ProviderName, DataProvider);
        }
        private Dictionary<string, IProcessDataProvider> providerStorage = new Dictionary<string, IProcessDataProvider>();

        public IProcessDataProvider DefaultDataProvider { get; set; }

        public IEnumerable<IProcessDataProvider> DataProviders { get { return providerStorage.Values; } }

        public void CreateOpcDataProvider(OpcDaProviderDefinition ProviderDefinition)
        {
            var provider = new ProcessDataProviders.OpcDa.OpcDaProcessDataProvider();
            provider.Host = ProviderDefinition.Host;
            provider.Domain = ProviderDefinition.Domain;
            provider.OpcName = ProviderDefinition.OpcName;
            provider.User = ProviderDefinition.User;
            provider.Password = ProviderDefinition.Password;
        }

    }
    [DataContract]
    public class OpcDaProviderDefinition
    {
        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string OpcName { get; set; }
        [DataMember]
        public string User { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
