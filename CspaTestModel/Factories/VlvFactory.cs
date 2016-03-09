using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CspaTestModel.Model;
using CspaTestModel.Model.Factories;

namespace CspaTestModel.Factories
{
    public class VlvFactory : IVlvFactory
    {
        IProcessDataProvider ProcessDataProvider { get; set; }
        public IVlv Create(XElement VlvNode)
        {
          
            string tmpName = VlvNode.Attribute("Name").Value;
            string tmpStr = VlvNode.Attribute("Index").Value;
            int tmpIndex = Convert.ToInt32(tmpStr);
            VLV retVal = new VLV(tmpIndex, tmpName);

            retVal.PositionTag = GetProcessItem(VlvNode, "3", "PositionTag");

            retVal.ThreatNotMaskedClosedTag = GetProcessItem(VlvNode, "11", "ThreatNotMaskedClosedTag");
            retVal.CutNotMaskedClosedTag = GetProcessItem(VlvNode, "11", "CutNotMaskedClosedTag");
            retVal.ThreatMaskedClosedTag = GetProcessItem(VlvNode, "11", "ThreatMaskedClosedTag");
            retVal.CutMaskedClosedTag = GetProcessItem(VlvNode, "11", "CutMaskedClosedTag");
            retVal.ThreatNotMaskedClosingTag = GetProcessItem(VlvNode, "11", "ThreatNotMaskedClosingTag");
            retVal.CutNotMaskedClosingTag = GetProcessItem(VlvNode, "11", "CutNotMaskedClosingTag");
            retVal.ThreatMaskedClosingTag = GetProcessItem(VlvNode, "11", "ThreatMaskedClosingTag");
            retVal.CutMaskedClosingTag = GetProcessItem(VlvNode, "11", "CutMaskedClosingTag");
            retVal.ThreatUnknownTag = GetProcessItem(VlvNode, "11", "ThreatUnknownTag");
            retVal.OkTag = GetProcessItem(VlvNode, "11", "OkTag");

            retVal.MaskStateTag = GetProcessItem(VlvNode, "9", "MaskStateTag");

            retVal.SetMaskOnTag = GetProcessItem(VlvNode, "9", "SetMaskOnTag");
            retVal.SetMaskOffTag = GetProcessItem(VlvNode, "9", "SetMaskOffTag");
            retVal.SetMaskOnConfirmTag = GetProcessItem(VlvNode, "108", "SetMaskOnConfirmTag");
            retVal.SetMaskOffConfirmTag = GetProcessItem(VlvNode, "108", "SetMaskOffConfirmTag");

            retVal.ProcessDataProvider = this.ProcessDataProvider;
            return retVal;
        }

        private Type GetSupportedType(string strType)
        {
            if (String.IsNullOrWhiteSpace(strType))
                throw new ArgumentNullException();
            string str = strType.Trim().ToLower();
            switch(str)
            {
                case "bool": return typeof(bool);
                case "real": return typeof(float);
                case "int": return typeof(int);
                default: throw new ArgumentException("Не могу преобразовать тип: " + str);
            }
        }

        private ProcessItem GetProcessItem(XElement x, string ArrayNumber, string ItemElementName)
        {
            //Определяем тип тега из типа массива
            string strType = x.Elements("ProcessTags").Elements(ItemElementName).First().Attribute("Type").Value.Trim();
            //Мутим System.Type из строки
            Type tmpType = GetSupportedType(strType);

            //Определяем адрес тега
            string strTagAddress = x.Elements("ProcessTags").Elements(ItemElementName).First().Value.Trim();
            return new ProcessItem(strTagAddress, tmpType);
        }

    }
}
