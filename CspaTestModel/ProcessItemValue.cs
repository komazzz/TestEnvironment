using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;

namespace CspaTestModel
{
    public class ProcessItemValue : IProcessItemValue
    {
        public ProcessItemValue(string ItemAddress)
        {
            this.ItemAddress = ItemAddress;
        }

        public string ItemAddress { get; private set; }

        public object Value { get; set; }

        public int Quality { get; set; }

        public DateTime TimeStamp { get; set; }

        public bool ErrorOccured { get; set; }
        public string ErrorDescription { get; set; }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            ProcessItemValue piv = obj as ProcessItemValue;

            if (piv == null)
                return false;
            else
                return ((this.ItemAddress == piv.ItemAddress) &&
                        (this.Value == piv.Value) &&
                        (this.Quality == piv.Quality) &&
                        (this.TimeStamp == piv.TimeStamp)&&
                        (this.ErrorOccured == piv.ErrorOccured));
        }

    }
}
