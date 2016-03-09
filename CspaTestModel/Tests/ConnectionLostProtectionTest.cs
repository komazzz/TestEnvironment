using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;
using CspaTestModel.Tests.Model;
using CspaTestModel.Model.Protections;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace CspaTestModel.Tests
{
    /// <summary>
    /// Данный класс представляет тест для защиты по потере связи.
    /// </summary>
    public class ConnectionLostProtectionTest:BaseTest
    {
        public IConnectionLostProtection ProtectionToTest { get; set; }

        uint _protectionTimerPreset = 5000;
        public uint ProtectionTimerPreset 
        { 
            get {return _protectionTimerPreset;}
            set { _protectionTimerPreset = value;}
        }

        uint _diagnoseTimerPreset = 5000;
        public uint DiagnoseTimerPreset
        {
            get { return _diagnoseTimerPreset; }
            set { _diagnoseTimerPreset = value; }
        }

        Stopwatch stw = new Stopwatch();
        protected override void DoTest()
        {
            Thread.CurrentThread.Name = "TestThread";
            //Проверка компонетов теста
            CheckComponents();

            List<string> errorStrings = new List<string>();

        //УСТАНАВЛИВАЕМ НАЧАЛЬНОЕ ЗНАЧЕНИЕ
            try
            {
                //Устанавливаем начальное состояние теста
                SetInitialState();
                //Переводим ТУ во включенное состояние
                TestEnvironment.Tu.SetTuOn();
                Wait();
                Assert.AreEqual<bool>(true, TestEnvironment.Tu.isOn, "ТУ не запустился после команды на пуск");

                //Проверяем защиту на соответствие деблокированному состоянию
                //  !ВУС, !ЗС, !ЗИ, ДБК, !МСК
                Assert.AreEqual<bool>(false, ProtectionToTest.isThreat, "Защита угрожает при деблокированной защите.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала при деблокированной защите..");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована при деблокированной защите..");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита не деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита неожиданно замаскировалась при деблокированной защите..");
            }
            catch(Exception e)
            {
                throw new Exception("Ошибка при установке начального значения", e);
            }

        //ВЫПОЛНЯЕМ УСЛОВИЯ СРАБАТЫВАНИЯ И ЖДЕМ ВЫПОЛНЕНИЯ УСЛОВИЙ СРАБАТЫВАНИЯ
            try
            {
                ProtectionToTest.ActivateThreat();

                stw.Restart();
                //Ждем условия срабатывания и проверяем
                while (stw.ElapsedMilliseconds + 1000 < DiagnoseTimerPreset)
                {
                        // !ВУС, !ЗС, !ЗИ, ДБК, !МСК
                        Assert.AreEqual<bool>(false, ProtectionToTest.isThreat, "Защита угрожает.");
                        Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                        Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                        Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                        Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
                        Thread.Sleep(200);
                }
                
                Thread.Sleep(1300);
                Wait();
                stw.Stop();
                //Проверяем состояниие "Выполненно условие срабатывания" 
                // ВУС, !ЗС, !ЗИ, ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при ожидании выполненных условий срабатывания защиты", e);
            }

        //ДЕБЛОКИРУМСЯ, СТАВИМ МАСКУ И МЕНЯЕМ УСТАВКУ
            try
            {
                //Пытаемся деблокировать, сменить уставку и замаскировать
                ProtectionToTest.Deblock();
                ProtectionToTest.TimePreset = ProtectionTimerPreset / 2000;
                ProtectionToTest.SetMask();
                Wait();

                // ВУС, !ЗС, !ЗИ, ДБК, МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isMasked, "Защита НЕ замаскировалась.");
                
                Assert.AreEqual<uint>(ProtectionTimerPreset/1000, ProtectionToTest.TimePreset, "Изменилась уставка");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Выполненные условия срабатывания попытка Маскировать, деблокировать, сменить уставку", e);
            }

        //СНИМАЕМ УСТАНОВЛЕННУЮ МАСКУ ПРИ ВЫПОЛНЕННЫХ УСЛОВИЯХ СРАБАТЫВАНИЯ
            try
            {
                ProtectionToTest.ResetMask();

                // ВУС, !ЗС, !ЗИ, ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскирована.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Выполненные условия срабатывания попытка снять установленную маску", e);
            }

        //СНИМАЕМ УСЛОВИЯ СРАБАТЫВАНИЯ
            try
            {
                //Возобновляем эмуляцию счетчика связи для данной защиты
                Stopwatch stw = new Stopwatch();
                Debug.Print("Stw started");
                stw.Restart();
                ProtectionToTest.InActivate();

                //Проверяем защиту на соответствие деблокированному состоянию
                // !ВУС, !ЗС, !ЗИ, ДБК, !МСК
                stw.Stop();
                Debug.Print("Stw before assert: "+stw.ElapsedMilliseconds);

                Assert.AreEqual<bool>(false, ProtectionToTest.isThreat, "Защита угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита не деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
            }
            catch (Exception e)
            {
                stw.Stop();
                Debug.Print("Stw in catch. Elapsed time: " + stw.ElapsedMilliseconds);
                throw new Exception("Ошибка. Некорректное снятие условий срабатывания", e);
            }

            //ВЫПОЛНЯЕМ УСЛОВИЯ СРАБАТЫВАНИЯ И ЖДЕМ ВЫПОЛНЕНИЯ УСЛОВИЙ СРАБАТЫВАНИЯ
            try
            {
                ProtectionToTest.ActivateThreat();
                stw.Restart();
                //Ждем условия срабатывания и проверяем
                while (stw.ElapsedMilliseconds + 1000 < DiagnoseTimerPreset)
                {
                    // !ВУС, !ЗС, !ЗИ, ДБК, !МСК
                    Assert.AreEqual<bool>(false, ProtectionToTest.isThreat, "Защита угрожает.");
                    Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                    Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                    Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                    Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
                    Thread.Sleep(200);
                }
                //stw.Stop();
                Thread.Sleep(1000);
                Wait();

                //Проверяем состояниие "Выполненно условие срабатывания" 
                // ВУС, !ЗС, !ЗИ, ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при ожидании выполненных условий срабатывания защиты", e);
            }

            //ЖДЕМ СРАБАТЫВАНИЯ ЗАЩИТЫ
            try
            {
                //stw.Restart();
                //Ждем условия срабатывания и проверяем
                while (stw.ElapsedMilliseconds + 1000 < ProtectionTimerPreset + DiagnoseTimerPreset)
                {
                    // ВУС, !ЗС, !ЗИ, ДБК, !МСК
                    Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита НЕ угрожает.");
                    Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                    Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                    Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита НЕ деблокирована.");
                    Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
                    Thread.Sleep(200);
                }
                stw.Stop();
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                stw.Stop();
                throw new Exception("Ошибка при ожидании срабатывания защиты", e);
            }

            //Проверяем состояниие "Защита сработала" 
            try
            {
                // ВУС, ЗС, !ЗИ,!ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isActive, "Защита не сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isDeblocked, "Защита деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при ожидании срабатывания защиты", e);
            }

        //ЗАЩИТА СРАБОТАЛА ДЕБЛОКИРУМСЯ, СТАВИМ МАСКУ И МЕНЯЕМ УСТАВКУ
            try
            {
                //Пытаемся деблокировать, сменить уставку и замаскировать
                ProtectionToTest.Deblock();
                ProtectionToTest.TimePreset = ProtectionTimerPreset / 2000;
                ProtectionToTest.SetMask();
                Wait();

                // ВУС, ЗС, !ЗИ, !ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isActive, "Защита не сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isDeblocked, "Защита деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");

                Assert.AreEqual<uint>(ProtectionTimerPreset / 1000, ProtectionToTest.TimePreset, "Изменилась уставка");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Защита сработала попытка Маскировать, деблокировать, сменить уставку", e);
            }


        //ОСТАНАВЛИВАЕМ ТУ ЗАЩИТА СРАБОТАЛА ДЕБЛОКИРУЕМСЯ, МЕНЯЕМ УСТАВКУ
            try
            {
                //Останавливаем ТУ
                TestEnvironment.Tu.SetTuOff();
                Wait();
                Assert.AreEqual<bool>(false, TestEnvironment.Tu.isOn, "ТУ не не остановился после команды на остановку");

                //Пытаемся деблокировать защиту, поменять уставку.
                ProtectionToTest.Deblock();
                ProtectionToTest.TimePreset = ProtectionTimerPreset / 2000;
                Wait();
                // ВУС, ЗС, !ЗИ, !ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isActive, "Защита не сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isDeblocked, "Защита деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");

                Assert.AreEqual<uint>(ProtectionTimerPreset / 1000, ProtectionToTest.TimePreset, "Изменилась уставка");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Защита сработала попытка, ТУ остановлен - деблокировать, сменить уставку", e);
            }

        //ОСТАНВОЛЕННЫЙ ТУ, ЗАЩИТА СРАБОТАЛА, ВЫПОЛНЕНЫ УСЛОВИЯ СРАБАТЫВАНИЯ -  МАСКИРУЕМ
            try
            {
                //Маскируем
                ProtectionToTest.SetMask();
                Wait();
                // ВУС, !ЗС, ЗИ, ДБК, МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isIgnored, "Защита не игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита не деблокирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isMasked, "Маска не установилась.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Защита сработала попытка, ТУ остановлен - ставим маску", e);
            }


            //ОСТАНОВЛЕННЫЙ ТУ, ЗАЩИТА ИГНОРИРОВАНА,ВЫПОЛНЕНЫ УСЛОВИЯ СРАБАТЫВАНИЯ -  СНИМАЕМ МАСКУ 
            try
            {
                //Снимаем маску
                ProtectionToTest.ResetMask();
                Wait();
                // ВУС, ЗС, !ЗИ, !ДБК, !МСК
                Assert.AreEqual<bool>(true, ProtectionToTest.isThreat, "Защита не угрожает.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isActive, "Защита не сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isDeblocked, "Защита деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Защита игнорирована, ТУ остановлен - снимаем маску", e);
            }

            
        //ОСТАНОВЛЕННЫЙ ТУ, ЗАЩИТА СРАБОТАЛА - СНИМАЕМ УСЛОВИЯ СРАБАТЫВАНИЯ
            try
            {
                //Запускаем счетчик связи (Снимаем условие срабатывания)
                ProtectionToTest.InActivate();
                Wait();
                // !ВУС, ЗС, !ЗИ, !ДБК, !МСК
                Assert.AreEqual<bool>(false, ProtectionToTest.isThreat, "Защита угрожает.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isActive, "Защита не сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isDeblocked, "Защита деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");

            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Защита сработала, ТУ остановлен - снимаем условия срабатывания", e);
            }

        //ОСТАНОВЛЕННЫЙ ТУ, ЗАЩИТА СРАБОТАЛА - ДЕБЛОКИРУЕМ
            try
            {
                //Деблокируем
                ProtectionToTest.Deblock();
                Wait();
                // !ВУС, !ЗС, !ЗИ, ДБК, МСК
                Assert.AreEqual<bool>(false, ProtectionToTest.isThreat, "Защита угрожает.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isActive, "Защита сработала.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isIgnored, "Защита игнорирована.");
                Assert.AreEqual<bool>(true, ProtectionToTest.isDeblocked, "Защита не деблокирована.");
                Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Защита замаскировалась.");
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка. Защита сработала, ТУ остановлен - деблокируем", e);
            }
        }

        /// <summary>
        /// Проверка компонетов теста
        /// </summary>
        private void CheckComponents()
        {
            if (ProtectionToTest == null)
                throw new ArgumentException("ProtectionToTest", "попытка запустить тест с отсутствующей защитой");

            if (TestEnvironment == null)
                throw new ArgumentException("TestEnvironment", "попытка запустить тест с отсутствующим окружением");
        }

        /// <summary>
        /// Установка начального состояния для тестируемой защиты
        /// </summary>
        /// <param name="errorStrings"></param>
        private void SetInitialState()
        {

            //Переводим ТУ в отключенное состояние
            TestEnvironment.Tu.SetTuOff();
            //Ждем
            Wait();
            Assert.AreEqual<bool>(false, TestEnvironment.Tu.isOn, "ТУ не не остановился после команды на остановку");
            
            ////Запускаем Эмуляцию счетчиков связи из НПС в ЦСПА
            //{
            //    foreach (IConnectionLostProtection clp in TestEnvironment.ConnectionLostProtections)
            //        clp.StartCounterEmulation();
            //}
            //Thread.Sleep(2000);
            //Снимаем условия срабатывания всех защит и деблокируем
            foreach (IProtection p in TestEnvironment.Protections)
            {
                p.InActivate();
     //           Thread.Sleep(1000);
                p.Deblock();
                Wait();
                Assert.AreEqual<bool>(true, p.isDeblocked, String.Format("Защита {0} не деблокировалась", p.Name));
            }

            //Снимаем отсечение станций от нефтепровода
            foreach (INps nps in TestEnvironment.NpsList)
            {
                if (!nps.isLast)
                {
                    nps.UnCut();
                    Wait();
                    Assert.AreEqual<bool>(false, nps.isCut, String.Format("НПС {0} осталась отсеченной", nps.Name));
                }
            }

            //Устанавливаем уставку времени на срабатывание защиты
            {
                ProtectionToTest.TimePreset = ProtectionTimerPreset/1000;
                Wait();
                Assert.AreEqual<uint>(ProtectionTimerPreset/1000, ProtectionToTest.TimePreset, "Не записалась уставка таймера защиты");

                ProtectionToTest.ConnectionDiagTimerPreset = DiagnoseTimerPreset/1000;
                Wait();
                Assert.AreEqual<uint>(DiagnoseTimerPreset/1000, ProtectionToTest.ConnectionDiagTimerPreset, "Не записалась уставка таймера диагностики связи");
            }

            //Снимаем маску
            ProtectionToTest.ResetMask();
            Wait();
            Assert.AreEqual<bool>(false, ProtectionToTest.isMasked, "Маска не снялась с выхода защиты");
            
        }

    }



}
