using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CspaTestModel.Model.Protections;
using CspaTestModel.Model;
using OpcDAL2;
using CspaTestModel.Protections;
using CspaTestModel;

namespace CspaTestModelUnitTests
{
    [TestClass]
    public class utConLost
    {
       // [TestMethod]
        public void CounterEmaulatorTest()
        {
            //Создаем dataProvider
            using (OpcDaContext pdp = new OpcDaContext())
            {
                pdp.NodeAddress = "serversdku1";
                pdp.ProgId = "Infinity.OPCServer";
                pdp.Connect();

                //Создаем объект тестирования
                ConnectionLostProtection clp = new ConnectionLostProtection();
                clp.CounterFromCspaTag = new ProcessItem("xxx.i1", typeof(int));
                clp.CounterToCspaTag = new ProcessItem("xxx.i2", typeof(int));
                clp.ProcessDataProvider = pdp;
                clp.ConnectionDiagTimerPreset = 2000;
                clp.StartCounterEmulation();
                System.Threading.Thread.Sleep(5000);
               }

            }
        }
    }

