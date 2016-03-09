using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;
using CspaTestModel.Model.Factories;
using System.IO;
using System.Xml.Linq;
using System.Data.OleDb;

namespace CspaTestModel.Factories
{
    public enum ItemAddressBaseEnum{Plc,Opc};
    public class xlNpsFactory:INpsFactory
    {
        public IProcessDataProvider ProcessDataProvider { get; set; }
        public ArrayStorage ArrayStorage { get; set; }
        public ItemAddressBaseEnum AddressBase { get; set; }

        private xlMnaFactory MnaFactory = new xlMnaFactory();

        /// <summary>
        /// Создает объект НПС
        /// </summary>
        /// <param name="NpsElement"></param>
        /// <returns></returns>
        protected virtual Nps CreateNps(XElement NpsElement)
        {
            string Name = NpsElement.Attribute("Name").Value;
            uint Index = Convert.ToUInt16(NpsElement.Attribute("Index").Value);
            Nps retVal = new Nps(Name, Index);
            retVal.isLast = Convert.ToBoolean((NpsElement.Attribute("isLast").Value));
            
            //Обрабатываем задвиги отсекающие НПС
            int tmpIndex = 0;
            string tmpStr = null;
            ProcessItem pi;

            foreach (var vlv in NpsElement.Element("CutVLVs").Elements("VLV")) 
            {
                string tmp = vlv.Attribute("Index").Value;
                tmpIndex = Convert.ToInt16(tmp);

                tmpStr = GetItemAddress(ArrayStorage.GetArray(3, DirectionEnum.ToCspa)[tmpIndex]);
                pi = new ProcessItem(tmpStr, typeof(Int16));

                //Код состояния закрытой задвижки
                retVal.cutValues.Add(new ProcessItemValue(pi) {Value = 3 });

                //Код состояния открытой задвижки
                retVal.unCutValues.Add(new ProcessItemValue(pi) { Value = 1 });
            }

            if (!retVal.isLast)
                retVal.MnaList = GetListMna(NpsElement.Element("MNAs"));

            return retVal;
        }

        /// <summary>
        /// Заполняет данными свойства объекта НПС
        /// </summary>
        /// <param name="nps"></param>
        protected virtual void FillData(ref Nps nps)
        {
            if (ProcessDataProvider==null)
                throw new ArgumentNullException("ProcessDataProvider");

            nps.ProcessDataProvider = this.ProcessDataProvider;
            
            //Тег признака отсечения НПС
            if(!nps.isLast)
            {
                int index = (Int16)nps.SchemaIndex - 1 + 8;
                string tmpStr = GetItemAddress(ArrayStorage.GetArray(25, DirectionEnum.FromCspa)[index]);
                nps.isNpsCutTag = new ProcessItem(tmpStr,typeof(bool));
            }
        }

        protected virtual List<IMna> GetListMna(XElement x)
        {
            if (x.Name.LocalName != "MNAs")
                throw new ArgumentException("Узел отличен от MNAs");

            if (MnaFactory == null)
                throw new ArgumentNullException("MnaFactory");

            MnaFactory.ArrayStorage = this.ArrayStorage;
            MnaFactory.ProcessDataProvider = this.ProcessDataProvider;
            MnaFactory.AddressBase = this.AddressBase;

            List<IMna> retVal = new List<IMna>();
            IMna tmpMna = null;

            foreach (var xlMna in x.Elements("MNA"))
            {
                try
                {
                    tmpMna = MnaFactory.Create(xlMna);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Unable to create MNA from xml node" + xlMna.Name.LocalName, e);
                }
                retVal.Add(tmpMna);
            }
            return retVal;
        }

        /// <summary>
        /// Выдает корректный адрес тега либо адрес opc в СДКУ либо адрес opc в Гейтвэе контроллера
        /// </summary>
        /// <param name="Element"></param>
        /// <returns></returns>
        protected virtual string GetItemAddress(ArrayElement Element)
        {
            if (AddressBase == ItemAddressBaseEnum.Opc)
                return Element.OpcVarName;
            else
                return Element.PlcVarName;
        }

        public void Dispose()
        {
            if (ProcessDataProvider != null)
                ProcessDataProvider.Dispose();
        }

        public INps Create(XElement xElement)
        {
            if (xElement == null)
                throw new ArgumentNullException("xElement", "Cannot find xml node for Nps object");

            Nps retVal = CreateNps(xElement);
            FillData(ref retVal);
            return retVal as INps;
        }
    }


    }




