using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface IProcessItemValue
    {
        string ItemAddress { get; }
        object Value { get; set; }
        int Quality { get; set; }
        DateTime TimeStamp { get; set; }
        bool ErrorOccured { get; set; }
        string ErrorDescription { get; set; }
    }
}
