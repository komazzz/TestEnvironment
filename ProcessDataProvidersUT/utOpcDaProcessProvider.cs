using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessDataProviders.OpcDa;
using CspaTestModel.Model;
using System.Diagnostics;

namespace ProcessDataProvidersUT
{
    [TestClass]
    public class utOpcDaProcessProvider : BaseOpcDaProcessProviderTest
    {
        [TestMethod]
        public void ConnectDisconnetEmptyProvider()
        {
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    OpcDaProcessDataProvider.Connect();
                    Thread.Sleep(1000);
                    OpcDaProcessDataProvider.Disconnect();
                }
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void ConnectWithGoodTagsRegistered()
        {
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    var GoodSignals = GetListOfTags(TagType.Good, OpcDaProcessDataProvider);
                    foreach (var tag in GoodSignals)
                        OpcDaProcessDataProvider.Register(tag);

                    OpcDaProcessDataProvider.Connect();
                    Thread.Sleep(1000);
                    OpcDaProcessDataProvider.Disconnect();
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void ConnectWithReadErrorTagsRegistered()
        {
            bool exceptionCatched = false;
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    var BadSignals = GetListOfTags(TagType.ReadError, OpcDaProcessDataProvider);
                    foreach (var tag in BadSignals)
                        OpcDaProcessDataProvider.Register(tag);

                    OpcDaProcessDataProvider.Connect();
                    Thread.Sleep(1000);
                    OpcDaProcessDataProvider.Disconnect();
                }
            }
            catch (ProcessDataProviderException e)
            {
                exceptionCatched = true;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.IsFalse(exceptionCatched);
        }

        [TestMethod]
        public void ReadGoodTags()
        {
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    OpcDaProcessDataProvider.Connect();

                    foreach (var item in TestingItemTypes)
                    {
                        var itemAddress = "GoodTags." + item;
                        Thread.Sleep(1000);
                        IProcessItemValue piv = OpcDaProcessDataProvider.ReadItem(itemAddress);

                        Assert.IsFalse(piv.ErrorOccured);
                        Assert.IsNull(piv.ErrorDescription);
                        Assert.IsTrue(piv.Quality >= 192);
                        Assert.IsTrue(piv.TimeStamp > DateTime.MinValue);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        public void ReadBadTags()
        {
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    OpcDaProcessDataProvider.Connect();

                    foreach(var item in TestingItemTypes)
                    {
                        var itemAddress = "ReadError." + item;
                        DateTime dtBeforeReadExec = DateTime.UtcNow;
                        Thread.Sleep(1000);
                        IProcessItemValue piv = OpcDaProcessDataProvider.ReadItem(itemAddress);

                        Assert.IsTrue(piv.ErrorOccured);
                        Assert.IsNotNull(piv.ErrorDescription);
                        Assert.IsTrue(piv.Quality == 0);
                        Assert.IsTrue(piv.TimeStamp > dtBeforeReadExec);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void WriteGoodTags()
        {
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    OpcDaProcessDataProvider.Connect();

                    foreach (var itemType in TestingItemTypes)
                    {
                        var itemAddress = "GoodTags." + itemType;
                        var itemValue = GenerateItemValue(itemType);
                        OpcDaProcessDataProvider.WriteItem(itemAddress, itemValue);
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void WriteBadTags()
        {
            bool[] ExceptionCatched = new bool[TestingItemTypes.Length];
            int i = 0;
            try
            {
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    OpcDaProcessDataProvider.Connect();

                    foreach (var itemType in TestingItemTypes)
                    {
                        var itemAddress = "WriteError." + itemType;
                        var itemValue = GenerateItemValue(itemType);
                        try
                        {
                            OpcDaProcessDataProvider.WriteItem(itemAddress, itemValue);
                        }
                        catch (ProcessDataProviderException e)
                        {
                            ExceptionCatched[i++] = true;
                        }
                        catch { }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Assert.Fail(e.Message);
            }

            foreach (var result in ExceptionCatched)
                Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReadWriteGoodTags()
        {
            try
            {
                DateTime timestampBeforeExecution = DateTime.UtcNow;
                Thread.Sleep(1000);
                using (var OpcDaProcessDataProvider = GetEmptyProvider())
                {
                    OpcDaProcessDataProvider.Connect();

                    foreach (var itemType in TestingItemTypes)
                    {
                        var itemAddress = "GoodTags." + itemType;
                        var itemValue = GenerateItemValue(itemType);
                        OpcDaProcessDataProvider.WriteItem(itemAddress, itemValue);

                        var piv = OpcDaProcessDataProvider.ReadItem(itemAddress);

                        Assert.AreEqual(itemValue,piv.Value);
                        Assert.IsTrue(piv.TimeStamp>timestampBeforeExecution);
                    }
                }
            }
            catch(AssertFailedException ex)
            {
                Assert.Fail();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Assert.Fail(e.Message);
            }
        }
    }


}
