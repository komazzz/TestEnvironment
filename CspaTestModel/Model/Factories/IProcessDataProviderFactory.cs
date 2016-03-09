using System.Xml.Linq;

namespace CspaTestModel.Model.Factories
{
    public interface IProcessDataProviderFactory
    {
        IProcessDataProvider Create(XElement DataProviderElement);
    }
}
