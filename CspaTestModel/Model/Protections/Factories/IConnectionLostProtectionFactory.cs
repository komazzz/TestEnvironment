using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model.Protections;
using System.Xml.Linq;

namespace CspaTestModel.Model.Protections.Factories
{
    public interface IConnectionLostProtectionFactory
    {
        IConnectionLostProtection Create(XElement xElement);
    }
}
