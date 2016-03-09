using System;
using System.Collections.Generic;
using CspaTestModel.Model;
using ProcessDataProviders.OpcDa;
using CspaTestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CspaEnvironment;
using CspaTestModel.Protections;
using System.Linq;
using ProcessDataProviders.OpcDa;


namespace ProcessDataProvidersUT.TestEnvironment
{
    [TestClass]
    public class utTestEnvironmentDefinition
    {
    

        [TestMethod]
        public void  TestSaveXml()
        {
            string filePath = Environment.CurrentDirectory + @"\testSave.xml";
            var envDef = GetTestEnvironmentDefinition();
            envDef.SaveDataToXml(filePath);
        }


        [TestMethod]
        public void TestLoadXml()
        {
            string filePath = Environment.CurrentDirectory + @"\testSave.xml";
            var envDef = GetTestEnvironmentDefinition();
            envDef.SaveDataToXml(filePath);

            var envDefLoaded = new CspaTestEnvironmentDefinition();
            envDefLoaded.LoadDataFromXml(filePath);
        }




        
        CspaTestEnvironmentDefinition GetTestEnvironmentDefinition()
        {
            var retVal = new CspaTestEnvironmentDefinition();
            retVal.DataProviders = new List<OpcDaProcessDataProvider>() { GetOpcDataProvider() };
            var dataProvider = retVal.DataProviders[0];

            retVal.BoolProcessItems = GetBoolProcessItems(dataProvider);
            retVal.IntProcessItems = GetIntProcessItems(dataProvider);
            retVal.StringProcessItems = GetStringProcessItems(dataProvider);

         //   retVal.VlvList = GetListVlv(retVal.ProcessItems);

          //  retVal.VlvCombinationList = GetVlvCombinationList(retVal.VlvList);

            return retVal;
        }

        OpcDaProcessDataProvider GetOpcDataProvider()
        {
            return new OpcDaProcessDataProvider()
                                    {   Domain="ddd", 
                                        Host="host", 
                                        OpcName="opcServ.DA",
                                        User="User", 
                                        Password="password"}; 

        }

        List<IProcessItem<bool>> GetBoolProcessItems(IProcessDataProvider DataProvider)
        {
            var retVal = new List<IProcessItem<bool>>();
            retVal.Add(new ProcessItem<bool>("tst.bool", DataProvider));
            return retVal;
        }

        List<IProcessItem<int>> GetIntProcessItems(IProcessDataProvider DataProvider)
        {
            var retVal = new List<IProcessItem<int>>();
            retVal.Add(new ProcessItem<int>("tst.int32", DataProvider));
            return retVal;
        }

        List<IProcessItem<string>> GetStringProcessItems(IProcessDataProvider DataProvider)
        {
            var retVal = new List<IProcessItem<string>>();
            retVal.Add(new ProcessItem<string>("tst.string", DataProvider));
            return retVal;
        }


        List<IVlv> GetListVlv(List<object> ProcessItems)
        {
            var retVal = new List<IVlv>();

            var piInt = (ProcessItem<int>)ProcessItems.Where(o=> o.GetType() == typeof(ProcessItem<int>)).First();
            var piBool = (ProcessItem<bool>)ProcessItems.Where(o => o.GetType() == typeof(ProcessItem<bool>)).First();
            retVal.Add(new VLV(1, "vlv1")
            {
                PositionTag = piInt,
                MaskStateTag = piBool,
            });
            retVal.Add(new VLV(2, "vlv2")
            {
                PositionTag = piInt,
                MaskStateTag = piBool,
            });

            retVal.Add(new VLV(3, "vlv3")
            {
                PositionTag = piInt,
                MaskStateTag = piBool,
            });
            return retVal;
        }

        List<IVlvCombination> GetVlvCombinationList(List<IVlv> VlvList)
        {
            var retVal = new List<IVlvCombination>();
            retVal.Add(new VlvCombination(1, new IVlv[] { VlvList[0] }));
            retVal.Add(new VlvCombination(2, new IVlv[] { VlvList[0], VlvList[1] }));
            retVal.Add(new VlvCombination(3, new IVlv[] { VlvList[0], VlvList[1], VlvList[2] }));

            return retVal;
        }

    }

     
}
