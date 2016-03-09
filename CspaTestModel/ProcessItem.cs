using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model;
using System.Runtime.Serialization;



namespace CspaTestModel
{
    [DataContract(IsReference=true)]
    [KnownType("GetTypes")]
    public class ProcessItem<T> : IProcessItem<T>
    {
        public static Type[] GetTypes()
        {
            return new Type[] { typeof( ProcessItem<T>)};
        }
        [DataMember]
        public string Address { get; private set; }

       
        [DataMember]
        public IProcessDataProvider ProcessDataProvider { get; private set; }
        
        public DateTime TimeStamp { get; private set; }
        public int Quality { get; private set; }
        public T Value
        {
            get
            {
                return _value;
            }
            private set
            {
                _value = value;
            }
        }
        public ProcessItemErrorException ErrorException { get; private set; }

        public event Action<object,ProcessItemChangedEventArgs<T>> ProcessItemChanged;

        private T _value = default(T);
        private object locker = new object();

        public ProcessItem(string ItemAddress, IProcessDataProvider Provider)
        {
            this.Address = ItemAddress;
            this.ProcessDataProvider = Provider;
        }

        public T ReadValue()
        {
            IProcessItemValue piv = ProcessDataProvider.ReadItem(Address);
            UpdateState(piv);
            ThrowIfErrorOccured();

            return Value;
        }

        public void WriteValue(T Value)
        {
            try
            {
                ProcessDataProvider.WriteItem(Address, Value);
            }
            catch (ProcessDataProviderException e )
            {
                var str = String.Format("При выполнении записи произшло исключение с сигналом {0}: {1}", Address, e.Message);
                ErrorException = new ProcessItemErrorException(str,e);
                ErrorOccured = true;
                OnProcessItemChanged();
                ThrowIfErrorOccured();
            }
        }

        private bool ErrorOccured { get; set; }

        public void UpdateState(IProcessItemValue ProcessItemValue)
        {
            lock (locker)
            {
                var piv = ProcessItemValue;
                //Обновляем только новыми значениями
                if ((this.Address == piv.ItemAddress) && (this.TimeStamp <= ProcessItemValue.TimeStamp))
                {
                    //причина обновления - смена качества, значения, ошибки
                    if ((this.Quality != piv.Quality) ||
                            (!this.Value.Equals(piv.Value)) ||
                            (piv.ErrorOccured)
                            )
                    {
                        this.Quality = ProcessItemValue.Quality;
                        this.TimeStamp = ProcessItemValue.TimeStamp;
                        if(piv.Value!=null)
                            this.Value = (T)ProcessItemValue.Value;

                        if (ProcessItemValue.ErrorOccured)
                        {
                            var str = String.Format("Произшло исключение с сигналом {0}: {1}", Address, ProcessItemValue.ErrorDescription);
                            ErrorException = new ProcessItemErrorException(str);
                        }
                        OnProcessItemChanged();
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            var temp = obj as ProcessItem<T>;
            if (temp == null)
                return false;

            return this.Address.Equals(temp.Address);
        }

        public override int GetHashCode()
        {
            return this.Address.GetHashCode();
        }

        private void ThrowIfErrorOccured()
        {
            if (ErrorException != null)
            {
                var errStr = String.Format("С сигналом {0} произошла ошибка", Address);
                throw new ProcessItemErrorException(errStr, ErrorException);
            }
        }

        private void OnProcessItemChanged()
        {
            var handler = this.ProcessItemChanged;
            if (handler!=null)
                {
                    var arg = new ProcessItemChangedEventArgs<T>(this);
                    handler(this,arg);
                }
        }


        public Type Type
        {
            get { return typeof(T); }
        }
    }

    [Serializable]
    public class ProcessItemErrorException : Exception
    {
        public ProcessItemErrorException() : base() { }

        public ProcessItemErrorException(string message) : base(message) { }

        public ProcessItemErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
