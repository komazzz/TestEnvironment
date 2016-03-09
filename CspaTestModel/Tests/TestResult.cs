using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Tests.Model;

namespace CspaTestModel.Tests
{
    /// <summary>
    /// Класс сообщающий результат тестирвоания
    /// </summary>
    public class TestResult : ITestResult
    {
        public TestResult(DateTime StartTime, DateTime EndTime, long Duration,
                            IEnumerable<string> Errors, IEnumerable<Exception> Exceptions)
        {
            _startTime = StartTime;
            _endTime = EndTime;
            _duration = Duration;
            _errors = Errors;
            _exceptions = Exceptions;
            if (_exceptions != null)
            {
                _isFailed = (_exceptions.Count() > 0);
                _hasExceptions = _isFailed;
            }
            else
            {
                if (_errors != null)
                {
                    _isFailed = (_errors.Count() > 0);
                    _hasExceptions = false;
                }
                else
                {
                    _isFailed = false; ;
                    _hasExceptions = false;
                }
            }
        }

        bool _isFailed = false;
        public bool IsFailed
        {
            get { return _isFailed; }
        }

        DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
        }

        DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
        }

        IEnumerable<string> _errors = null;
        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }

        bool _hasExceptions;
        public bool HasExceptions
        {
            get { return _hasExceptions; }
        }

        IEnumerable<Exception> _exceptions;
        public IEnumerable<Exception> Exceptions
        {
            get { return _exceptions; }
        }

        long _duration;
        public long Duration
        {
            get { return _duration; }
        }
    }
}
