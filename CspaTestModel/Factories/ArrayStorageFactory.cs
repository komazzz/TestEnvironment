using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;


namespace CspaTestModel.Factories
{
    public class ArrayStorageFactory
    {
        //Расположение файла хранилища содержащего ИО
        public string xlFilePath { get; set; }

        string _plcVarPrefix = String.Empty;
        public string PlcVarPrefix
        {
            get { return _plcVarPrefix; }
            set 
            {
                if (value != null)
                    _plcVarPrefix = ProcessPrefix(value);
                else
                    _plcVarPrefix = String.Empty;
            }
        }

        string _opcVarPrefix = String.Empty;
        public string OpcVarPrefix
        {
            get { return _opcVarPrefix; }
            set
            {
                if (value != null)
                    _opcVarPrefix = ProcessPrefix(value);
                else
                    _opcVarPrefix = String.Empty;
            }
        }


        private string ProcessPrefix(string InputPrefix)
        {
            string retVal = InputPrefix.Trim();
            if (retVal.EndsWith("."))
                _plcVarPrefix = retVal;
            else
                _plcVarPrefix = retVal + ".";
            return retVal;
        }
        public ArrayStorage Create()
        {
            //Проверяем наличие файла
            if (!File.Exists(xlFilePath))
                throw new ArgumentNullException(xlFilePath);

            if (String.IsNullOrWhiteSpace(PlcVarPrefix))
                throw new ArgumentNullException(PlcVarPrefix);

            //Запрашиваем данные из файла эксель
            OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder();
            csb.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=\"Excel 12.0;HDR=YES\"";
            csb.DataSource = this.xlFilePath;
            ArrayStorage RetVal = new ArrayStorage();

            using (OleDbConnection con = new OleDbConnection(csb.ConnectionString))
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandText = "Select * From[Gateway$]";
                cmd.Connection = con;

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {

                    ArrayElement tmpElement = null;
                    Array curArray = null;
                    int rowCount = 1;
                    int curArrayNumber;
                    int lastArrayNumber = 0;

                    string curDirection = null;
                    string lastDirection = null;

                    int curElementOrderNumber = 1;
                    try
                    {
                        while (reader.Read())
                        {

                            object tmp = reader.GetValue(3);
                            
                            //Защита от нулов
                            if (tmp.GetType() == typeof(System.DBNull))
                                break;

                            curArrayNumber = Convert.ToInt16(reader.GetValue(3));
                            curDirection = reader.GetString(4).Trim().ToUpper();

                            //провереям на начало нового массива
                            if ((curArrayNumber != lastArrayNumber) || (curDirection != lastDirection))
                            {
                                //Добавляем в текущий объект массива в хранилище
                                if (curArray != null)
                                    RetVal.Add(curArray);

                                //Создаем новый объект массив
                                curArray = new Array();
                                curArray.ArrayNumber = (uint)curArrayNumber;
                                curArray.Direction = GetDirection(curDirection);
                                curArray.Type = GetElementType(reader.GetString(5));
                                curArray.Name = String.Format("Массив-{0}", curArrayNumber);

                                //Сохраняем переменные
                                lastArrayNumber = curArrayNumber;
                                lastDirection = curDirection;
                                curElementOrderNumber = 1;
                            }

                            //Создаем новый элемент 
                            tmpElement = new ArrayElement();
                            tmpElement.Array = curArray;
                            tmpElement.PlcVarName = reader.GetString(6).Trim();//PlcVarPrefix + reader.GetString(2).Trim().ToLower();
                            tmpElement.Description = reader.GetString(1).Trim();
                            tmpElement.OrderNumber = (uint)curElementOrderNumber;

                            curArray.Elements.Add(tmpElement);
                            curElementOrderNumber++;
                            rowCount++;
                        }
                        //Добавляем последний объект массива в хранилище
                        RetVal.Add(curArray);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException(String.Format("Ошибка проеобразования строки {0} файла Excel.", rowCount), e);
                    }
                }

                return RetVal;
            }


        }

        private DirectionEnum GetDirection(string str)
        {
            switch (str)
            {
                case "I": return DirectionEnum.ToCspa;
                case "O": return DirectionEnum.FromCspa;
                default: return DirectionEnum.Unknown;
            }
        }

        private Type GetElementType(string Type)
        {
            switch (Type.ToUpper())
            {
                case "BOOL": return typeof(Boolean);
                case "INT": return typeof(Int16);
                case "REAL": return typeof(float);
                case "UINT": return typeof(UInt16);
                default: return typeof(object);
            }
        }
    }
}
