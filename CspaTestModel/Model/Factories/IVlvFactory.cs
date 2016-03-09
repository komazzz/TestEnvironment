using System.Xml.Linq;

namespace CspaTestModel.Model.Factories
{
    public interface IVlvFactory
    {
        IVlv Create(XElement VlvNode);
    }
}
