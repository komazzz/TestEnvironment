using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;

namespace CspaTestModel.Tests.Model
{
    public interface ITest
    {
       // ITestEnvironment TestEnvironment { get; set; }
        string Name { get; set; }
        int StepSleepTime { get; set; }

        ITestResult ExecuteTest();
    }

 
}
