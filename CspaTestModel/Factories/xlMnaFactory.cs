using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model.Factories;
using CspaTestModel.Model;

namespace CspaTestModel.Factories
{
    public class xlMnaFactory
    {
        public IProcessDataProvider ProcessDataProvider { get; set; }
        public ArrayStorage ArrayStorage { get; set; }
        public ItemAddressBaseEnum AddressBase { get; set; }

        public Mna Create(XElement MnaNode)
        {
            string Name = MnaNode.Attribute("Name").Value;
            uint Index = Convert.ToUInt16(MnaNode.Attribute("Index").Value);
            Mna retVal = new Mna(Name, Index);
            retVal.DataProvider = ProcessDataProvider;

            ArrayElement tempAE = ArrayStorage.GetArray(22, DirectionEnum.ToCspa)[(int)Index];
            retVal.MnaStateTag = new ProcessItem(GetItemAddress(tempAE), typeof(bool));

            tempAE = ArrayStorage.GetArray(23, DirectionEnum.ToCspa)[(int)Index];
            retVal.MnaStateQualityTag = new ProcessItem(GetItemAddress(tempAE), typeof(bool));

            return retVal;
        }


        protected virtual string GetItemAddress(ArrayElement Element)
        {
            if (AddressBase == ItemAddressBaseEnum.Opc)
                return Element.OpcVarName;
            else
                return Element.PlcVarName;
        }

    }
}
