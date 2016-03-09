using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CspaTestModel.Model
{
    [Serializable]
    public class ProcessDataProviderException : Exception
    {
        public ProcessDataProviderException() { }

        public ProcessDataProviderException(string message) : base(message) { }

        public ProcessDataProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
