using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CspaTestModel.Model.Protections;
using CspaTestModel.Model;
using System.Diagnostics;

namespace CspaTestModel.Protections
{
    public class ConnectionLostProtection:IConnectionLostProtection
    {
        public string Name { get; set; }
        public IProcessDataProvider ProcessDataProvider { get; set; }
        
        public IProcessItem CounterFromCspaTag { get; set; }
        public IProcessItem CounterToCspaTag { get; set; }

        //Теои защиты
        public IProcessItem ThreatTag { get; set; }
        public IProcessItem ActiveTag { get; set; }
        public IProcessItem IgnoredTag { get; set; }
        public IProcessItem DeblockedTag { get; set; }
        public IProcessItem DeblockConditionTag { get; set; }
        public IProcessItem DeblockCmdTag { get; set; }

        //Теги маски
        public IProcessItem MaskedTag { get; set; }
        public IProcessItem SetMaskOnTag { get; set; }
        public IProcessItem SetMaskOffTag { get; set; }
        public IProcessItem SetMaskOnConfirmTag { get; set; }
        public IProcessItem SetMaskOffConfirmTag { get; set; }

        //2-ой массив -Уставка таймера защиты
        public IProcessItem TimerPresetTagInCSPA { get; set; }
        public IProcessItem TimerPresetTagToCSPA { get; set; }

        //2-ой массив -Уставка таймера диагностики связи
        public IProcessItem ConDiagTimerPresetTagInCSPA { get; set; }
        public IProcessItem ConDiagTimerPresetTagToCSPA { get; set; }

        //Тег команды установки параметров во второй массив
        public IProcessItem SetTimerPresetTag { get; set; }



        #region IConnectionLostProtection Implementation
        //Уставка тйамера ЦСПА диагностирующего потерю связи
        public uint ConnectionDiagTimerPreset
        {
            get 
            {
                IProcessItemValue piv = ProcessDataProvider.ReadItem(ConDiagTimerPresetTagInCSPA);
                return Convert.ToUInt32(piv.Value); 
            }
            set 
            {
                IProcessItemValue piv = new ProcessItemValue(ConDiagTimerPresetTagToCSPA) {Value = (Single)value };
                ProcessDataProvider.WriteItem(piv);
                piv = new ProcessItemValue(SetTimerPresetTag) { Value = true };
                ProcessDataProvider.WriteItem(piv);
            }
        }

        //Инфрастукртура для таска
        private CancellationTokenSource cTokenSource { get; set; }
        //private CancellationToken cToken { get; set; }
        private Task taskEmulation { get; set; }
        

        public void StartCounterEmulation()
        {
            if (CounterFromCspaTag == null)
                throw new ArgumentNullException("CounterFromCspaTag");

            if (CounterToCspaTag == null)
                throw new ArgumentNullException("CounterToCspaTag");

            if (EmulationStatus == ConnectionEmulationStatusEnum.NotStarted)
            {
                cTokenSource = new CancellationTokenSource();
                CancellationToken cToken = cTokenSource.Token;
                taskEmulation = Task.Factory.StartNew(()=>EmulateCounter(cToken), cToken);
            }
            RequestAndWaitForEmulationState(ConnectionEmulationStatusEnum.InProgress);
            CheckTaskStatus();
        }

        public void PauseCounterEmulation()
        {
            CheckTaskStatus();
            RequestAndWaitForEmulationState(ConnectionEmulationStatusEnum.Paused);
        }

        public void ResumeCounterEmulation()
        {
            CheckTaskStatus();
            RequestAndWaitForEmulationState(ConnectionEmulationStatusEnum.InProgress);
        }

        public void StopCounterEmulation()
        {
            if (cTokenSource != null)
            {
                cTokenSource.Cancel();
            }
            SetEmulStatus(ConnectionEmulationStatusEnum.NotStarted);
        }


        ConnectionEmulationStatusEnum _conEmulStatus = ConnectionEmulationStatusEnum.NotStarted;
        public ConnectionEmulationStatusEnum EmulationStatus { get { return _conEmulStatus; } }
        private ConnectionEmulationStatusEnum RequestedEmulationStatus { get; set; }
        //Устанавливает новый статус эмуляции счетчиков связи

        private void SetEmulStatus(ConnectionEmulationStatusEnum NewStatus)
        {
            _conEmulStatus = NewStatus;
        }

        //проверяеи сиаиус таска выполняющего эмуляцию
        private void CheckTaskStatus()
        {
            try
            {
                if (taskEmulation.IsFaulted)
                    taskEmulation.Wait();
            }
            catch (AggregateException ae)
            {
                throw new ArgumentException("Unable to perform counter emulation: " + ae.Message, ae);
            }
        }

        //эсмулятор счетчика связи
        private void EmulateCounter(Object Token1)
        {
            if (Token1 == null)
                throw new ArgumentException("Cancellation Token is null");
            CancellationToken Token = (CancellationToken)Token1 ;
            Thread.CurrentThread.Name = Name+"counter";
            IProcessItemValue pivInput = null;
            ProcessItemValue pivOutput = new ProcessItemValue(CounterToCspaTag);

            uint pollPeriod = 500;//(ConnectionDiagTimerPreset < 2000) ? ConnectionDiagTimerPreset/2:1000;
            //pollPeriod = pollPeriod < 500 ? 500 : pollPeriod;
            while (true)
            {
                
                if (this.RequestedEmulationStatus != ConnectionEmulationStatusEnum.Paused)
                {
                    try
                    {
                        pivInput = ProcessDataProvider.ReadItem(CounterFromCspaTag);
                        pivOutput.Value = Convert.ToInt16(pivInput.Value) + 1;
                        ProcessDataProvider.WriteItem(pivOutput);

                        Thread.Sleep((int)pollPeriod);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                    Thread.Sleep(100);

                if (EmulationStatus != RequestedEmulationStatus)
                    SetEmulStatus(RequestedEmulationStatus);
                
                if (Token.IsCancellationRequested)
                    throw new TaskCanceledException();
            }
        }

        private Stopwatch stw = new Stopwatch();
        private object locker = new object();
        private void RequestAndWaitForEmulationState(ConnectionEmulationStatusEnum RequestedState)
        {
            lock (locker)
            {
                RequestedEmulationStatus = RequestedState;
                stw.Restart();
                while(EmulationStatus != RequestedState)
                {
                    Thread.Sleep(100);
                    if (stw.ElapsedMilliseconds > 2000)
                    {
                        stw.Stop();
                        throw new TimeoutException("Эмулятор не смог перейти из состояния: " + GetStateName(EmulationStatus) + "в запрошенное состояние: " + GetStateName(RequestedEmulationStatus));
                    }
                }
            }
            
        }

        private string GetStateName(ConnectionEmulationStatusEnum State)
        {
            switch(State)
            {
                case ConnectionEmulationStatusEnum.InProgress: return "ВЫПОЛНЕНИЕ";
                case ConnectionEmulationStatusEnum.NotStarted: return "НЕ СТАРТОВАЛ";
                case ConnectionEmulationStatusEnum.Paused: return "ПРИОСТАНОВЛЕН";
                default: return "ХЗ";
            }
        }
        #endregion

        #region IProtection Implementation
        public uint TimePreset
        {
            get 
            {
                IProcessItemValue piv = ProcessDataProvider.ReadItem(TimerPresetTagInCSPA);
                return Convert.ToUInt32(piv.Value); 
            }
            set 
            {
                IProcessItemValue piv = new ProcessItemValue(TimerPresetTagToCSPA) {Value = (Single)value };
                ProcessDataProvider.WriteItem(piv);
                piv = new ProcessItemValue(SetTimerPresetTag) { Value = true };
                ProcessDataProvider.WriteItem(piv);
            }
        }

        public Guid Id { get; set; }

        public bool isThreat
        {
            get { return TagEqualsTo(ThreatTag,true); }
        }

        public bool isActive
        {
            get { return TagEqualsTo(ActiveTag, true); }
        }

        public bool isIgnored
        {
            get { return TagEqualsTo(IgnoredTag, true); }
        }

        public bool DeblockCondition
        {
            get { return TagEqualsTo(DeblockConditionTag, true); }
        }

        public bool isDeblocked
        {
            get { return TagEqualsTo(DeblockedTag, true); }
        }

        public bool isMasked
        {
            get { return TagEqualsTo(MaskedTag, true); }
        }


        public void Deblock()
        {
            ProcessItemValue piv = new ProcessItemValue(DeblockCmdTag) { Value = true };
            ProcessDataProvider.WriteItem(piv);
        }

        public void SetMask()
        {
            ProcessItemValue piv = new ProcessItemValue(SetMaskOnTag) { Value = true };
            ProcessDataProvider.WriteItem(piv);
        }

        public void ResetMask()
        {
            ProcessItemValue piv = new ProcessItemValue(SetMaskOffTag) { Value = true };
            ProcessDataProvider.WriteItem(piv);
        }

        public void ActivateThreat()
        {
            PauseCounterEmulation();
            Thread.Sleep(100);
        }

        public void Activate()
        {
            int sleepTime = (int)TimePreset * 1000;
            switch (EmulationStatus)
            {
                case ConnectionEmulationStatusEnum.InProgress:
                    this.PauseCounterEmulation();
                    Thread.Sleep(sleepTime);
                    break;
                case ConnectionEmulationStatusEnum.NotStarted:
                    break;
                case ConnectionEmulationStatusEnum.Paused: 
                    break;
                default: break;
            }
        }

        public void InActivate()
        {
            switch(EmulationStatus)
            {
                case ConnectionEmulationStatusEnum.InProgress:
                    break;
                case ConnectionEmulationStatusEnum.NotStarted: 
                    this.StartCounterEmulation();
                    Thread.Sleep(100);
                    break;
                case ConnectionEmulationStatusEnum.Paused: 
                    this.ResumeCounterEmulation();
                   Thread.Sleep(100);
                    break;
                default: break;
            }
        } 

        public void Dispose()
        {
            if (taskEmulation!=null)
            {
                cTokenSource.Cancel();
                try
                {
                    taskEmulation.Wait();
                }
                catch (AggregateException ae)
                {
                    taskEmulation = null;
                }
                finally
                {
                    cTokenSource.Dispose();
                }
            }
        }
        #endregion

        private bool TagEqualsTo(IProcessItem Tag, object ExpectedValue)
        {
            IProcessItemValue val = this.ProcessDataProvider.ReadItem(Tag);
            return val.Value.Equals(ExpectedValue);
        }
    }
}
