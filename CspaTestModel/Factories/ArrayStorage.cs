using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Factories
{
    public class ArrayStorage
    {
        class ArrayPrimaryKey
        {
            public int ArrayNumber { get; set; }
            public DirectionEnum Direction { get; set; }

            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                ArrayPrimaryKey tmp = obj as ArrayPrimaryKey;

                if (tmp == null)
                    return false;

                return (this.ArrayNumber == tmp.ArrayNumber)&&(this.Direction == tmp.Direction);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(ArrayNumber.ToString());
                sb.Append(Direction.ToString());
                return sb.ToString();
            }
        }

        private Dictionary<ArrayPrimaryKey, Array> storage = new Dictionary<ArrayPrimaryKey, Array>();

        public virtual Array GetArray(int ArrayNumber, DirectionEnum Direction)
        {
            ArrayPrimaryKey tmp = (new ArrayPrimaryKey() { ArrayNumber = ArrayNumber, Direction = Direction });
            if (!storage.ContainsKey(tmp))
                throw new ArgumentOutOfRangeException("Запршенного массива в хранилище нет");
                return storage[tmp];
        }

        public virtual void Add(Array Array)
        {   
            if(Array==null)
                return;
            ArrayPrimaryKey tmpAPK = new ArrayPrimaryKey() { ArrayNumber = (int)Array.ArrayNumber, Direction = Array.Direction };
            storage.Add(tmpAPK, Array);
        }

        public virtual IEnumerable<Array> Arrays
        {
            get
            {
                return storage.Values.OrderBy(a=>a.ArrayNumber).ThenBy(a=>a.Direction);
            }
        }

        public virtual int Count
        {
            get { return storage.Count(); }
        }

        public virtual bool ContainsArray(int ArrayNumber, DirectionEnum Direction)
        {
            ArrayPrimaryKey tmp = (new ArrayPrimaryKey() { ArrayNumber = ArrayNumber, Direction = Direction });
            return storage.ContainsKey(tmp);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var a in this.Arrays)
                sb.AppendLine(a.ToString());
            return sb.ToString();
        }
    }
}
