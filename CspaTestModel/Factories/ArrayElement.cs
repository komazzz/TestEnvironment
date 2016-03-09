using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Factories
{
    public class ArrayElement
    {
        public Array Array { get; set; }
        public string Description { get; set; }
        public string PlcVarName { get; set; }
        public string OpcVarName { get; set; }
        public uint OrderNumber { get; set; }

        public override bool Equals(object obj)
        {
            ArrayElement ae = obj as ArrayElement;
            if (ae == null)
                return false;

            return (this.OrderNumber == ae.OrderNumber) &&
                    (this.Array == ae.Array) &&
                    (this.PlcVarName == ae.PlcVarName);
        }

        public override int GetHashCode()
        {
            return this.OrderNumber.GetHashCode() & this.Array.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Элемент № {0}, Перменная: {1}, Описание: {2}.", this.OrderNumber, this.PlcVarName, this.Description);
        }
    }
}
