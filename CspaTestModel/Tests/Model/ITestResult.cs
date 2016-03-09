using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Tests.Model
{
    /// <summary>
    /// Описывает результат выполнения теста
    /// </summary>
    public interface ITestResult
    {
        /// <summary>
        /// признак того что тест провалился
        /// </summary>
         bool IsFailed { get; }

        /// <summary>
        /// признак наличия исключений при прохождении теста
        /// </summary>
         bool HasExceptions {get;}

        /// <summary>
        /// время начала теста
        /// </summary>
         DateTime StartTime { get; }

        /// <summary>
        /// время окончания теста
        /// </summary>
         DateTime EndTime { get; }

        /// <summary>
        /// продолжительность теста
        /// </summary>
         long Duration { get; }

        /// <summary>
        /// Ошибки возникшие при выполнении теста
        /// </summary>
         IEnumerable<String> Errors { get; }

        /// <summary>
        /// исключения возникшие при выполнении теста
        /// </summary>
         IEnumerable<Exception> Exceptions { get; }
    }   
}
