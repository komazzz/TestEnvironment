using System;
using System.Collections.Generic;
using System.Linq;
using CspaTestModel.Model;
using System.Runtime.Serialization;
using System.IO;
using CspaTestModel.Protections;
using System.Xml;
using ProcessDataProviders.OpcDa;

namespace CspaEnvironment
{
    [DataContract]
   // [KnownType(typeof(ProcessDataProviders.OpcDa.OpcDaProcessDataProvider))]
    public class CspaTestEnvironmentDefinition
    {
        [DataMember(Order = 0)]
        public List<OpcDaProcessDataProvider> DataProviders { get; set; }

        [DataMember(Order = 1)]
        public List<IProcessItem<Boolean>> BoolProcessItems { get; set; }
        [DataMember(Order = 2)]
        public List<IProcessItem<Int32>> IntProcessItems { get; set; }
        [DataMember(Order = 3)]
        public List<IProcessItem<Single>> FloatProcessItems { get; set; }
        [DataMember(Order = 4)]
        public List<IProcessItem<Double>> DoubleProcessItems { get; set; }
        [DataMember(Order = 5)]
        public List<IProcessItem<String>> StringProcessItems { get; set; }


        [DataMember(Order = 6)]
        public List<IVlv> VlvList { get; set; }

        [DataMember(Order = 7)]
        public List<IVlvCombination> VlvCombinationList { get; set; }

        [DataMember(Order = 8)]
        public VlvClosingProtection VlvClosingProtection { get; set; }

        public void SaveDataToXml(String FilePath)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                fs.Flush();
                var dcs = new DataContractSerializer(typeof(CspaTestEnvironmentDefinition), null, int.MaxValue, false, false, null, new SharedTypeResolver());
                dcs.WriteObject(fs, this);
            }
        }

        public void LoadDataFromXml(String FilePath)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open))
            {
                var dcs = new DataContractSerializer(typeof(CspaTestEnvironmentDefinition), null,int.MaxValue, false,false,null,new SharedTypeResolver() );
                
                CspaTestEnvironmentDefinition tst = (CspaTestEnvironmentDefinition)dcs.ReadObject(fs);
                FillData(tst);
            }
        }

        private void FillData(CspaTestEnvironmentDefinition Def)
        {
            this.DataProviders = Def.DataProviders;

            this.BoolProcessItems = Def.BoolProcessItems;

            this.IntProcessItems = Def.IntProcessItems;

            this.FloatProcessItems = Def.FloatProcessItems;

            this.DoubleProcessItems = Def.DoubleProcessItems;

            this.StringProcessItems = Def.StringProcessItems;

            this.VlvList = Def.VlvList;
        }
    }


    public class SharedTypeResolver : DataContractResolver
    {

        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {

            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {

                XmlDictionary dictionary = new XmlDictionary();
                
                typeName = dictionary.Add(dataContractType.FullName);

                var asmName = dataContractType.Assembly.GetName();
                typeNamespace = dictionary.Add(asmName.Name);

            }
            return true;
        }



        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            Type retVal = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);

            if (retVal == null)
            {
                retVal = Type.GetType(typeName + "," + typeNamespace);

                if(retVal == null)
                {
                    string xmlAsmName = typeNamespace;
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm=>
                    {
                        var asmName = asm.GetName();
                        return asmName.Name == xmlAsmName;
                    });

                    if(assembly!=null)
                        retVal = assembly.GetType(typeName);
                }
            }

            return retVal;
        }

    }


}
