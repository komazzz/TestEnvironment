using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CspaTestModel.Model.Factories
{
    public interface INpsFactory:IDisposable
    {
        INps Create(XElement xElement);
    }
}
