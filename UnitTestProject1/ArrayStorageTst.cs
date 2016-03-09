using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using CspaTestModel.Factories;

namespace CspaTestModelUnitTests
{
    /// <summary>
    /// Summary description for ArrayStorageTst
    /// </summary>
    [TestClass]
    public class ArrayStorageTst
    {
        [TestMethod]
        public void ArrayStorageCreationTest()
        {
            try
            {
                string xlFilePath = @"..\..\..\TestXml\ModelSchema\xlFileStorage.xlsm";
                ArrayStorageFactory asf = new ArrayStorageFactory();
                asf.xlFilePath = xlFilePath;
                asf.PlcVarPrefix = "CoDeSys.OPC.DA.";

                var t = Stopwatch.StartNew();
                var arrayStorage = asf.Create();
                t.Stop();

                Debug.Print(arrayStorage.ToString() + "\n" + t.ElapsedMilliseconds + "ms");

            }
            catch(Exception e)
            {
                throw new InternalTestFailureException(e.Message, e);
            }
        }
    }
}
