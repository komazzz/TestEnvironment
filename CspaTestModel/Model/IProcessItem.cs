using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface IProcessItem<T> : IProcessItemDefinition
    {
        IProcessDataProvider ProcessDataProvider { get; }
        T Value { get; }
        int Quality { get; }
        DateTime TimeStamp { get; }
        event Action<object, ProcessItemChangedEventArgs<T>> ProcessItemChanged;

        T ReadValue();
        void WriteValue(T Value);
    }

    public class ProcessItemChangedEventArgs<T> : EventArgs
    {
        public ProcessItemChangedEventArgs(IProcessItem<T> ProcessItem)
        {
            this.Item = ProcessItem;
        }
        public IProcessItem<T> Item { get; private set; }
    }


}
