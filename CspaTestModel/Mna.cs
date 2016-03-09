using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;

namespace CspaTestModel
{
    public class Mna:IMna
    {
        public IProcessItem MnaStateTag { get; set; }
        public IProcessItem MnaStateQualityTag { get; set; }
        public IProcessDataProvider DataProvider { get; set; }

        public Mna(string Name, uint SchemaIndex)
        {
            this._name = Name;
            this._schemaIndex = SchemaIndex;
        }

        private string _name;
        public string Name { get { return _name; } }

        private uint _schemaIndex;
        public uint SchemaIndex { get { return _schemaIndex; } }

        public void SetOn()
        {
            IProcessItemValue piv = new ProcessItemValue(MnaStateTag){Value = false};
            DataProvider.WriteItem(piv);
        }

        public void SetOff()
        {
            IProcessItemValue piv = new ProcessItemValue(MnaStateTag) { Value = true };
            DataProvider.WriteItem(piv);
        }

        public MnaStateEnum State
        {
            get { 
                var q = DataProvider.ReadItem(MnaStateQualityTag);
                if ((int)q.Value < 192)
                    return MnaStateEnum.Unknown;

                var s = DataProvider.ReadItem(MnaStateTag);
                if ((bool)s.Value == true)
                    return MnaStateEnum.On;
                else
                    return MnaStateEnum.Off;
            }
        }

        public void SetUnknown()
        {
            IProcessItemValue piv = new ProcessItemValue(MnaStateTag) { Value = false };
            DataProvider.WriteItem(piv);
        }

        public void SetExcluded()
        {
            throw new NotImplementedException();
        }

        public void ResetExcluded()
        {
            throw new NotImplementedException();
        }

        public bool isExcluded
        {
            get { throw new NotImplementedException(); }
        }
    }
}
