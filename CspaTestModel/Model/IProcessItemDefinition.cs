using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CspaTestModel.Model
{
    public interface IProcessItemDefinition
    {
        string Address { get; }
        Type Type { get; }
        void UpdateState(IProcessItemValue ProcessItemValue);

    }
}
