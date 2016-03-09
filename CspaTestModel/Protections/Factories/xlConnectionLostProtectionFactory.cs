using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model.Protections.Factories;
using CspaTestModel.Factories;
using System.Xml.Linq;
using CspaTestModel.Model.Protections;
using CspaTestModel.Protections;
using CspaTestModel.Model;

namespace CspaTestModel.Protections.Factories
{
    public class xlConnectionLostProtectionFactory:IConnectionLostProtectionFactory
    {
        public IProcessDataProvider ProcessDataProvider { get; set; }
        public ArrayStorage ArrayStorage { get; set; }
        public ItemAddressBaseEnum AddressBase { get; set; }

        protected virtual string GetItemAddress(ArrayElement Element)
        {
            if (AddressBase == ItemAddressBaseEnum.Opc)
                return Element.OpcVarName;
            else
                return Element.PlcVarName;
        }
        public IConnectionLostProtection Create(XElement ConLostElement)
        {
            string Name = ConLostElement.Attribute("Name").Value;
            uint Index = Convert.ToUInt16(ConLostElement.Attribute("Index").Value);
            ConnectionLostProtection retVal = new ConnectionLostProtection();
            retVal.Name = Name;
            retVal.ProcessDataProvider = ProcessDataProvider;
            ArrayElement tmpAE = null;

            //5 массив - Команды деблокировки
            tmpAE = ArrayStorage.GetArray(5, DirectionEnum.ToCspa)[((int)Index - 1) + 7];
            retVal.DeblockCmdTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            #region 10 массив
            //ВУС
            tmpAE = ArrayStorage.GetArray(10, DirectionEnum.FromCspa)[((int)Index - 1) * 5 + 82];
            retVal.ThreatTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            //ЗС
            tmpAE = ArrayStorage.GetArray(10, DirectionEnum.FromCspa)[((int)Index - 1) * 5 + 83];
            retVal.ActiveTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            //ЗИ
            tmpAE = ArrayStorage.GetArray(10, DirectionEnum.FromCspa)[((int)Index - 1) * 5 + 84];
            retVal.IgnoredTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            //УДБ
            tmpAE = ArrayStorage.GetArray(10, DirectionEnum.FromCspa)[((int)Index - 1) * 5 + 85];
            retVal.DeblockConditionTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            //ДБ
            tmpAE = ArrayStorage.GetArray(10, DirectionEnum.FromCspa)[((int)Index - 1) * 5 + 86];
            retVal.DeblockedTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool)); 
            #endregion

            //34 массив - Счетчик связи НПС --> ЦСПА
            tmpAE = ArrayStorage.GetArray(34, DirectionEnum.ToCspa)[(int)Index];
            retVal.CounterToCspaTag = new ProcessItem(GetItemAddress(tmpAE), tmpAE.Array.Type);

            //35 массив - Счетчик связи ЦСПА <-- НПС
            tmpAE = ArrayStorage.GetArray(35, DirectionEnum.FromCspa)[(int)Index];
            retVal.CounterFromCspaTag = new ProcessItem(GetItemAddress(tmpAE), tmpAE.Array.Type); 

            //37 массив - Команды маскирования/демаскирования по выходу защиты
            tmpAE = ArrayStorage.GetArray(37, DirectionEnum.ToCspa)[((int)Index - 1) * 2 + 15];
            retVal.SetMaskOnTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            tmpAE = ArrayStorage.GetArray(37, DirectionEnum.ToCspa)[(int)Index + 15];
            retVal.SetMaskOffTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool)); 

            //38 массив - Признаки маскирования выходов защит
            tmpAE = ArrayStorage.GetArray(38, DirectionEnum.FromCspa)[((int)Index-1) + 7];
            retVal.MaskedTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));

            //2 массив Уставки защиты
            tmpAE = ArrayStorage.GetArray(2, DirectionEnum.FromCspa)[((int)Index - 1) + 14];
            retVal.TimerPresetTagInCSPA = new ProcessItem(GetItemAddress(tmpAE), typeof(Single));

            tmpAE = ArrayStorage.GetArray(2, DirectionEnum.ToCspa)[((int)Index - 1) + 14];
            retVal.TimerPresetTagToCSPA = new ProcessItem(GetItemAddress(tmpAE), typeof(Single));

            tmpAE = ArrayStorage.GetArray(2, DirectionEnum.FromCspa)[((int)Index - 1) + 18];
            retVal.ConDiagTimerPresetTagInCSPA = new ProcessItem(GetItemAddress(tmpAE), typeof(Single));

            tmpAE = ArrayStorage.GetArray(2, DirectionEnum.ToCspa)[((int)Index - 1) + 18];
            retVal.ConDiagTimerPresetTagToCSPA = new ProcessItem(GetItemAddress(tmpAE), typeof(Single));

            //Команда на запису уставок во второй массив
            tmpAE = ArrayStorage.GetArray(27, DirectionEnum.ToCspa)[1];
            retVal.SetTimerPresetTag = new ProcessItem(GetItemAddress(tmpAE), typeof(bool));


            return retVal;
        }

    }
}
