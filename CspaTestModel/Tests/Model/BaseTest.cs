using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CspaTestModel.Tests.Model
{
    /// <summary>
    /// Базовый класс для всех тестов
    /// </summary>
    public abstract class BaseTest : ITest
    {
        //public TestEnvironment TestEnvironment { get; set; }

        public string Name { get; set; }

        int _stepSleepTime = 200;
        public int StepSleepTime 
        {
            get { return _stepSleepTime; } 
            set
            {
                if ((value >= 100) && (value < 2000))
                {
                    _stepSleepTime = value;
                }
            }
        }

        public ITestResult ExecuteTest()
        {
            IEnumerable<string> errors = null;
            List<Exception> exceptions = null;
            DateTime startTime = DateTime.Now;
            TestResult retVal = null;
            var stopwWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                
                DoTest();
            }
            catch (AssertFailedException e)
            {
                errors = new List<string>() { e.Message };
            }
            catch (Exception e)
            {
                exceptions = new List<Exception>();
                exceptions.Add(e);
            }
            finally
            {
                retVal = new TestResult(startTime, DateTime.Now, stopwWatch.ElapsedMilliseconds, errors, exceptions);
            }
            return retVal;
        }

        protected void Wait()
        {
            System.Threading.Thread.Sleep(StepSleepTime);
        }
        protected abstract void DoTest();

    }
}
