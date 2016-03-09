using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessDataProviders.OpcDa;
using CspaTestModel.Model;
using CspaTestModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ProcessDataProvidersUT
{
    [TestClass]
    public class utProcessItemGeneric:BaseOpcDaProcessProviderTest
    {

        [TestMethod]
        public void ReadGoodItems()
        {
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("GoodTags.Int16", DataProvider);
                    int result = pi.ReadValue();
                    Assert.IsTrue(result == pi.Value);
                }
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void ReadGoodRegisteredItems()
        {
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    bool eventCalled = false;
                    ProcessItem<String> pi = new ProcessItem<String>("GoodTags.String", DataProvider);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true; };
                    String result = pi.ReadValue();

                    Assert.IsTrue(eventCalled);
                    Assert.IsTrue(result == pi.Value);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void ReadBadItems()
        {
            bool ExceptionCaught = false;
            bool eventCalled = false;
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("ReadError.Int16", DataProvider);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true; };
                    int result = pi.ReadValue();
                }
            }
            catch(ProcessItemErrorException)
            {
                ExceptionCaught = true;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(ExceptionCaught);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void RegisterBadItems()
        {
            bool eventCalled = false;
            
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("ReadError.Int16", DataProvider);
                    DataProvider.Register(pi);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true;};
                    Assert.IsTrue(pi.ErrorException == null);
                    DataProvider.Connect();
                    Thread.Sleep(1000);
                    Assert.IsTrue(pi.ErrorException != null);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(eventCalled);
            
        }

        [TestMethod]
        public void ReadRegisteredBadItems()
        {
            bool ExceptionCaught = false;
            bool eventCalled = false;
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("ReadError.Int16", DataProvider);
                    DataProvider.Register(pi);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true; };
                    int result = pi.ReadValue();
                }
            }
            catch (ProcessItemErrorException)
            {
                ExceptionCaught = true;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(ExceptionCaught);
            Assert.IsTrue(eventCalled);
        }



        [TestMethod]
        public void WriteGoodItems()
        {
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("GoodTags.Int16", DataProvider);
                    short valueToWrite = (short)Randomizer.Next(Int16.MaxValue);
                    pi.WriteValue(valueToWrite);
                    Thread.Sleep(300);
                    short readValue = pi.ReadValue();

                    Assert.IsTrue(valueToWrite == readValue);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void WriteGoodRegisteredItems()
        {
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    bool eventCalled = false;
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("GoodTags.Int16", DataProvider);
                    DataProvider.Register(pi);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true; };

                    short valueToWrite = (short)Randomizer.Next(Int16.MaxValue);
                    pi.WriteValue(valueToWrite);
                    Thread.Sleep(300);
                    Assert.IsTrue(valueToWrite == pi.Value);
                    Assert.IsTrue(eventCalled);

                    short readValue = pi.ReadValue();
                    Assert.IsTrue(valueToWrite == readValue);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void WriteBadItems()
        {
            bool ExceptionCaught = false;
            bool eventCalled = false;
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("WriteError.Int16", DataProvider);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true; };
                    short valueToWrite = (short)Randomizer.Next(Int16.MaxValue);
                    pi.WriteValue(valueToWrite);
                }
            }
            catch (ProcessItemErrorException)
            {
                ExceptionCaught = true;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(ExceptionCaught);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void WriteRegisteredBadItems()
        {
            bool ExceptionCaught = false;
            bool eventCalled = false;
            try
            {
                using (IProcessDataProvider DataProvider = GetEmptyProvider())
                {
                    ProcessItem<Int16> pi = new ProcessItem<Int16>("WriteError.Int16", DataProvider);
                    pi.ProcessItemChanged += (obj, args) => { eventCalled = true; };
                    
                    short valueToWrite = (short)Randomizer.Next(Int16.MaxValue);
                    pi.WriteValue(valueToWrite);
                }
            }
            catch (ProcessItemErrorException)
            {
                ExceptionCaught = true;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(ExceptionCaught);
            Assert.IsTrue(eventCalled);
        }

    }
}
