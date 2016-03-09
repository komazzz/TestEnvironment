using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface IProcessDataProvider : IDisposable
    {
         string ProviderName { get; }
         Guid ProviderId { get; }
        void Register(IProcessItemDefinition ProcessItem);
        void Deregister(IProcessItemDefinition ProcessItem);
        bool IsRegistered(IProcessItemDefinition ProcessItem);
        IEnumerable<IProcessItemDefinition> Items { get; }

        IProcessItemValue ReadItem(String ItemAddress);
        void WriteItem(String ItemAddress, object ItemValue);

        bool IsConnected { get; }
        void Connect();
        void Disconnect();
    }
}
