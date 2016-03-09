using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Factories
{
    public class Array
    {
        public string Name { get; set; }
        public uint ArrayNumber { get; set; }
        public DirectionEnum Direction { get; set; }
        public Type Type { get; set; }

        List<ArrayElement> _elements = new List<ArrayElement>();
        public List<ArrayElement> Elements { get { return _elements; } }

        
        public ArrayElement this[int ElementIndex]
        {
            get
            {
                if((ElementIndex > Count)||(ElementIndex < 1))
                    throw new ArgumentOutOfRangeException("Выход за границы массива: " +this.ArrayNumber +this.Direction);
                return Elements[ElementIndex - 1];

            }       

        }
        public int Count { get { return _elements.Count; } }

        public override bool Equals(object obj)
        {
            Array tmp = obj as Array;
            if (tmp == null)
                return false;

            return (this.ArrayNumber == tmp.ArrayNumber) && 
                    (this.Type == tmp.Type) && 
                    (this.Direction == tmp.Direction);
        }

        public override int GetHashCode()
        {
            return String.Format("{0},{1}",this.ArrayNumber,this.Direction).GetHashCode();
        }

        public override string ToString()
        {
            string tmpDir = "ХЗ";
            switch(this.Direction)
            {
                case DirectionEnum.FromCspa: tmpDir = "Из ЦСПА";
                                                break;
                case DirectionEnum.ToCspa: tmpDir = "В ЦСПА";
                                                break;
                case DirectionEnum.Unknown: tmpDir = "Неизвестно";
                                                break;
            }
                
            StringBuilder strB = new StringBuilder();

            strB.AppendFormat("Массив № {0}, {1}, {2}, Кол-во: {3}", this.ArrayNumber, tmpDir,this.Type, this.Elements.Count);
            return strB.ToString();
        }
    }

    public enum DirectionEnum { ToCspa,FromCspa, Unknown}
}
