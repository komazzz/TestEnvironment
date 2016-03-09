using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpcDAL2;
using CspaTestModel;
using CspaTestModel.Model;
using System.Threading;

namespace CspaTestModelUnitTests
{
    [TestClass]
    public class TestUnitTest
    {
        //Тест делает следующее:
        //1. Инкрементирует значение тега;
        //2. Ожидает;
        //3. Считывает значение тега;
        //4. Проверяет на четность считанное значение, если тег имеет четное значение то тест удался, иначе тест провален.
        [TestMethod]
        public void ТестовыйТест()
        {
                using (OpcDaContext daContext = new OpcDaContext())
                {
                    //Указываем Адрес Opc Сервера и подключаемся к нему
                    daContext.NodeAddress = "serversdku1";
                    daContext.ProgId = "Infinity.OPCServer";
                    daContext.Connect();
                    
                    //Инкрементируем тег
                    ProcessItem pi = new ProcessItem("Test.Int32", typeof(Int32));
                    IProcessItemValue piv = daContext.ReadItem(pi);
                    piv.Value = Convert.ToInt16(piv.Value) + 1;
                    daContext.WriteValue(piv);
                    Thread.Sleep(1000);

                    //Читаем тег
                    IProcessItemValue piv2 = daContext.ReadItem(piv.Item);

                    //Проверяем на четность
                    Int32 tmp = Convert.ToInt32(piv2.Value);
                    
                    //Если нечетный то ПРОВАЛ
                    if(tmp % 2 != 0)
                        Assert.Fail("Нечетное значение тега.");
                }
        }
    }
}
