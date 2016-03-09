using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CspaTestEnvironment;
using CspaTestModel.Model.Factories;
using CspaTestModel.Model;
using CspaTestModel.Model.Protections;
using CspaTestModel.Tests;
using CspaTestModel.Tests.Model;
using CspaTestModel;

using System.Diagnostics;


namespace CspaTestModelUnitTests
{
    /// <summary>
    /// Summary description for TestEnvironmentFactory
    /// </summary>
    [TestClass]
    public class TstEnvironment
    {
        public ITestEnvironment TE { get; set; }
        [TestMethod]
        public void EnvironmentCreationTest()
        {
             ITestEnvironment te = null;
            try
            {
                xlTestEnvironmentFactory tef = new xlTestEnvironmentFactory();
                 
                tef.ModelSchemaFilePath = @"..\..\..\TestXml\ModelSchema\ModelSchema.xml";
                tef.xlStorageFilePath = @"..\..\..\TestXml\ModelSchema\xlFileStorage.xlsm";
                tef.PlcVarPrefix = "CoDeSys.OPC.DA.";
                tef.TestEnvironmentBase = EnvironmentBaseEnum.BaseOnPlcValues;

                te = tef.Create();
                TE = te;
                te.DataProvider.Connect();

                Debug.Print(te.NpsList.Count.ToString());
                
            }

            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                if(te!=null)
                    te.Dispose();
            }
        }

       // [TestMethod]
        public void tstConnectionLostProtection()
        {
            using (var TestEnvironment = CreateTestEnvironment())
            {
                var test = new ConnectionLostProtectionTest();
                test.Name = "SampleTest";
                test.StepSleepTime = 200;
                test.TestEnvironment = TestEnvironment;
                test.ProtectionToTest = TestEnvironment.ConnectionLostProtections.First();
                try
                {
                   ITestResult res =  test.ExecuteTest();
                   if  (res.HasExceptions)
                       throw res.Exceptions.First();
                   if (res.IsFailed)
                       Assert.Fail(res.Errors.FirstOrDefault());
                }
                catch (Exception e)
                {
                    throw new AssertFailedException("Exception throwed: " +e.Message , e);
                }

            }
        }
  
        ITestEnvironment CreateTestEnvironment()
        {
            xlTestEnvironmentFactory tef = new xlTestEnvironmentFactory();

            tef.ModelSchemaFilePath = @"..\..\..\TestXml\ModelSchema\ModelSchema.xml";
            tef.xlStorageFilePath = @"..\..\..\TestXml\ModelSchema\xlFileStorage.xlsm";
            tef.PlcVarPrefix = "CoDeSys.OPC.DA.";
            tef.TestEnvironmentBase = EnvironmentBaseEnum.BaseOnPlcValues;

            ITestEnvironment te = null;
            te = tef.Create();
            return te;
        }
    }
}
