using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CspaTestModel.Model;
using CspaTestModel;
using System.IO;
using System.Text;

namespace CspaEnvironment
{
    public class ProcessItemManager
    {
        Dictionary<string, IProcessItemDefinition> definitionStorage = new Dictionary<string, IProcessItemDefinition>();
        Dictionary<string, IProcessItem<Boolean>> boolStorage = new Dictionary<string, IProcessItem<bool>>();
        Dictionary<string, IProcessItem<Int32>> intStorage = new Dictionary<string, IProcessItem<Int32>>();
        Dictionary<string, IProcessItem<float>> floatStorage = new Dictionary<string, IProcessItem<Single>>();
        Dictionary<string, IProcessItem<Double>> doubleStorage = new Dictionary<string, IProcessItem<Double>>();
        Dictionary<string, IProcessItem<String>> stringStorage = new Dictionary<string, IProcessItem<String>>();

        readonly Type[] _supportedTypes = new Type[] {   typeof(bool), 
                                                                typeof(Int32), 
                                                                typeof(float), 
                                                                typeof(double), 
                                                                typeof(string) 
                                                            };
        public IEnumerable<Type> SupportedTypes { get { return _supportedTypes; } }

        public IEnumerable<IProcessItem<Boolean>> BoolProcessItems { get { return boolStorage.Values; } }
        public IEnumerable<IProcessItem<Int32>> IntProcessItems { get { return intStorage.Values; } }
        public IEnumerable<IProcessItem<Single>> FloatProcessItems { get { return floatStorage.Values; } }
        public IEnumerable<IProcessItem<Double>> DoubleProcessItems { get { return doubleStorage.Values; } }
        public IEnumerable<IProcessItem<String>> StringProcessItems { get { return stringStorage.Values; } }

        public void CreateProcessItem(String Address, Type Type, IProcessDataProvider DataProvider)
        {
            if (DataProvider == null)
                throw new ArgumentException("Dataprovider == null");

            if (!SupportedTypes.Contains(Type))
                throw new ArgumentException("Тип не поддерживается" + Address);

            string normAddress = NormalizeAddress(Address);
            if (definitionStorage.ContainsKey(normAddress))
                throw new ArgumentException("Объект уже существует" + Address);

            CreateTypedProcessItem(normAddress, Type, DataProvider);
        }

        private string NormalizeAddress(String Address)
        {
            return Address.Trim().ToUpperInvariant();
        }

        private void CreateTypedProcessItem(String Address, Type Type, IProcessDataProvider DataProvider)
        {
            if (Type == typeof(Boolean))
            {
                var pi = new ProcessItem<Boolean>(Address, DataProvider);
                boolStorage.Add(pi.Address, pi);
                definitionStorage.Add(pi.Address, pi);
            }
            if (Type == typeof(Int32))
            {
                var pi = new ProcessItem<Int32>(Address, DataProvider);
                intStorage.Add(pi.Address, pi);
                definitionStorage.Add(pi.Address, pi);
            }
            if (Type == typeof(Single))
            {
                var pi = new ProcessItem<Single>(Address, DataProvider);
                floatStorage.Add(pi.Address, pi);
                definitionStorage.Add(pi.Address, pi);
            }
            if (Type == typeof(Double))
            {
                var pi = new ProcessItem<Double>(Address, DataProvider);
                doubleStorage.Add(pi.Address, pi);
                definitionStorage.Add(pi.Address, pi);
            }
            if (Type == typeof(String))
            {
                var pi = new ProcessItem<String>(Address, DataProvider);
                stringStorage.Add(pi.Address, pi);
                definitionStorage.Add(pi.Address, pi);
            }

            throw new ArgumentException("Неизвестный тип. Не могу создать элемент");
        }


        public IProcessItem<Boolean> GetBoolProcessItem(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            var retval = boolStorage[normAddress];
            return retval;
        }

        public IProcessItem<Int32> GetIntProcessItem(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            var retval = intStorage[normAddress];
            return retval;
        }

        public IProcessItem<float> GetFloatProcessItem(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            var retval = floatStorage[normAddress];
            return retval;
        }

        public IProcessItem<Double> GetDoubleProcessItem(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            var retval = doubleStorage[normAddress];
            return retval;
        }

        public IProcessItem<String> GetStringProcessItem(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            var retval = stringStorage[normAddress];
            return retval;
        }

        public IProcessItemDefinition GetProcessItemDefinition(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            var retval = definitionStorage[normAddress];
            return retval;
        }

        public void ImportXml(String FilePath, IProcessDataProvider DataProvider)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                XDocument xdoc = XDocument.Load(fs);
                var errorStr = new StringBuilder();
                var piList = xdoc.Descendants("Item");
                
                foreach(XElement pi in piList)
                {
                    try
                    {
                        var addr = pi.Element("Address").Value;
                        Type type = ConvertType(pi.Element("Type").Value);
                        CreateProcessItem(addr, type, DataProvider);
                    }
                    catch(Exception e)
                    {
                        errorStr.AppendFormat("{0} - {1}\n", pi.Element("Address").Value, e.Message);
                    }
                }

                if (errorStr.Length > 0)
                    throw new ArgumentException(errorStr.ToString());
            }

        }

        public bool ContainsItem(String Address)
        {
            var normAddress = NormalizeAddress(Address);
            return definitionStorage.ContainsKey(normAddress);
        }
        private Type ConvertType(String TypeName)
        {
            var normTypeName = TypeName.Trim().ToLower();

            switch(normTypeName)
            {
                case "INT": return typeof(Int32);
                case "BOOL": return typeof(Boolean);
                case "FLOAT": return typeof(Single);
                case "REAL": return typeof(Single);
                case "DOUBLE": return typeof(Double);
                case "STRING": return typeof(String);
                default: throw new ArgumentException("Не могу извлечь тип из строки:" + normTypeName);
            }
        }

    }
}
