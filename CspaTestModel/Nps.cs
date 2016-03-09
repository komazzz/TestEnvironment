using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;

namespace CspaTestModel
{
    public class Nps:INps
    {
        public Nps(string Name, uint Index)
        {
            _name = Name;
            _schemaIndex = Index;
            cutValues = new List<ProcessItemValue>();
            unCutValues = new List<ProcessItemValue>();
        }
        string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        uint _schemaIndex;
        public uint SchemaIndex
        {
            get { return _schemaIndex; }
        }

        public void Cut()
        {
            if (ProcessDataProvider == null)
                throw new ArgumentNullException("ProcessDataProvider", "Nps class of : " + this.Name);

            foreach (ProcessItemValue v in cutValues)
                ProcessDataProvider.WriteItem(v);
        }

        public void UnCut()
        {
            if (ProcessDataProvider == null)
                throw new ArgumentNullException("ProcessDataProvider", "Nps class of : " + this.Name);

            foreach (ProcessItemValue v in unCutValues)
                ProcessDataProvider.WriteItem(v);
        }

        public bool isCut
        {
            get 
            {
                if (ProcessDataProvider == null)
                    throw new ArgumentNullException("ProcessDataProvider", "Nps class of : " + this.Name); 
                IProcessItemValue piv = ProcessDataProvider.ReadItem(isNpsCutTag);
                return Convert.ToBoolean(piv.Value);
            }
        }
        public bool isLast { get; set; }

        public IProcessDataProvider ProcessDataProvider { get; set; }

        public List<ProcessItemValue> cutValues { get; set; }
        public List<ProcessItemValue> unCutValues { get; set; }

        public ProcessItem isNpsCutTag { get; set; }

        public List<IMna> MnaList { get; set; }

    }
}
