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
   // [KnownType("GetTypes")]
    public class VLV:IVlv
    {
        private VLV() { }
        public VLV(int Index, string Name)
        {
            this.Index = Index;
            this.Name = Name;
        }

        [DataMember]
        public int Index {get;private set;}

        [DataMember]
        public string Name {get;private set;}

        public IState<VlvInputProtectionState> InputProtectionState
        {
            get { return _inputProtectionState; }
        }

        public IState<VlvPositionEnum> Position
        {
            get
            {
                return _position;
            }
        }

        public IState<bool> IsMasked
        {
            get { return _isMasked; }
        }

        protected IState<VlvInputProtectionState> _inputProtectionState;
        protected IState<VlvPositionEnum> _position;
        protected IState<bool> _isMasked;


        public void SetPosition(VlvPositionEnum position)
        {
            switch (position)
            {
                case VlvPositionEnum.Closed: PositionTag.WriteValue(3);
                    break;
                case VlvPositionEnum.Closing: PositionTag.WriteValue(2);
                    break;
                case VlvPositionEnum.Opening: PositionTag.WriteValue(4);
                    break;
                case VlvPositionEnum.Opened: PositionTag.WriteValue(1);
                    break;
                case VlvPositionEnum.Failure: PositionTag.WriteValue(5);
                    break;
                case VlvPositionEnum.Unknown: PositionQualityTag.WriteValue(0);
                    break;
            }
        }

        public VlvPositionEnum GetPosition()
        {
            int val = PositionTag.ReadValue();

            return GetPositionEnum(val);
        }


        public bool GetMaskState
        {
            get { return MaskStateTag.ReadValue(); }
        }
        
        public void SetMaskOn()
        {
            SetMaskOnTag.WriteValue(true);
        }

        public void SetMaskOff()
        {
            SetMaskOffTag.WriteValue(true);
        }

        public static Type[] GetTypes()
        {
            return new Type[] { typeof(VLV) };
        }

        protected void OnInputProtectionStateTagChanged(object s, ProcessItemChangedEventArgs<int> e)
        {
            var vlvInputProtectionState = GetInputProtectionStateEnum(e.Item.Value);
            _inputProtectionState = new State<VlvInputProtectionState>(vlvInputProtectionState, e.Item.TimeStamp);
        }

        protected void OnPositionTagChanged(object s, ProcessItemChangedEventArgs<int> e)
        {
            var vlvPosition = GetPositionEnum(e.Item.Value);
            _position = new State<VlvPositionEnum>(vlvPosition, e.Item.TimeStamp);
        }

        protected void OnMaskStateTagChanged(object s, ProcessItemChangedEventArgs<bool> e)
        {
            _isMasked = new State<bool>(e.Item.Value, e.Item.TimeStamp);
        }


        protected VlvInputProtectionState GetInputProtectionStateEnum(int EnumValue)
        {
            switch(EnumValue)
            {
                case 0: return VlvInputProtectionState.Ok;
                case 1: return VlvInputProtectionState.ThreatNotMaskedClosed;
                case 2: return VlvInputProtectionState.CutNotMaskedClosed;
                case 3: return VlvInputProtectionState.ThreatMaskedClosed;
                case 4: return VlvInputProtectionState.CutMaskedClosed;
                case 5: return VlvInputProtectionState.ThreatNotMaskedClosing;
                case 6: return VlvInputProtectionState.CutNotMaskedClosing;
                case 7: return VlvInputProtectionState.ThreatMaskedClosing;
                case 8: return VlvInputProtectionState.CutMaskedClosing;
                case 9: return VlvInputProtectionState.ThreatUnknown;
                case 10: return VlvInputProtectionState.LostControl;
                case 11: return VlvInputProtectionState.ThreatUnknownAndLostControl;
                default: throw new ArgumentOutOfRangeException("Не могу определить состояние задвижки  как входа защиты по считанному значению. Считанное значение:" + EnumValue);   
            }
        }

        protected VlvPositionEnum GetPositionEnum(int EnumValue)
        {
            switch (EnumValue)
            {
                case 1: return VlvPositionEnum.Opened;
                case 2: return VlvPositionEnum.Closing;
                case 3: return VlvPositionEnum.Closed;
                case 4: return VlvPositionEnum.Opening;
                case 5: return VlvPositionEnum.Failure;

                default: throw new ArgumentOutOfRangeException("Не могу определить состояние задвижки по считанному значению. Считанное значение:" + EnumValue);
            }
        }


        #region Теги задвижки
        [DataMember]
        private IProcessItem<int> _inputProtectionStateTag = null;
        public IProcessItem<int> InputProtectionStateTag
        {
            get { return _inputProtectionStateTag; }
            set
            {
                if (_inputProtectionStateTag == null)
                    _inputProtectionStateTag = value;
                if (_inputProtectionStateTag != value)
                {
                    _inputProtectionStateTag.ProcessItemChanged -= this.OnInputProtectionStateTagChanged;
                    _inputProtectionStateTag = value;
                    _inputProtectionStateTag.ProcessItemChanged += this.OnInputProtectionStateTagChanged;
                    _inputProtectionStateTag.ProcessDataProvider.Register(_inputProtectionStateTag);
                }
            }
        }

        [DataMember]
        private IProcessItem<int> _positionTag = null;
        public IProcessItem<int> PositionTag 
        { 
            get {return _positionTag;}
            set
            {
                if (_positionTag == null)
                    _positionTag = value;
                if(_positionTag!=value)
                {
                    _positionTag.ProcessItemChanged -= this.OnPositionTagChanged;
                    _positionTag = value;
                    _positionTag.ProcessItemChanged += this.OnPositionTagChanged;

                    _positionTag.ProcessDataProvider.Register(_positionTag);
                }
            }
        }

        [DataMember]
        private IProcessItem<bool> _maskStateTag = null;
        public IProcessItem<bool> MaskStateTag
        {
            get { return _maskStateTag; }
            set
            {
                if (_maskStateTag == null)
                    _maskStateTag = value;
                if (_maskStateTag != value)
                {
                    _maskStateTag.ProcessItemChanged -= this.OnMaskStateTagChanged;
                    _maskStateTag = value;
                    _maskStateTag.ProcessItemChanged += this.OnMaskStateTagChanged;

                    _maskStateTag.ProcessDataProvider.Register(_maskStateTag);
                }
            }
        }

        [DataMember]
        public IProcessItem<int> PositionQualityTag { get; set; }

        [DataMember]
        public IProcessItem<bool> SetMaskOnTag { get; set; }
        public IProcessItem<bool> SetMaskOffTag { get; set; }

        [DataMember]
        public IProcessItem<bool> SetMaskOnConfirmTag { get; set; }
        public IProcessItem<bool> SetMaskOffConfirmTag { get; set; }
        #endregion
    }

    
    public class State<T>:IState<T>
    {
        public State(T StateValue, DateTime Timestamp)
    {
        this.StateValue = StateValue;
        this.TimeStamp = Timestamp;
    }

        public DateTime TimeStamp { get; private set; }

        public bool NewerThan(IState<T> State)
        {
            return this.TimeStamp > State.TimeStamp;
        }

        public T StateValue { get; set; }
    }

    
    public enum VlvInputProtectionState
    {
        Ok = 0,
        ThreatNotMaskedClosed = 1,
        CutNotMaskedClosed = 2,
        ThreatMaskedClosed = 3,
        CutMaskedClosed =4,
        ThreatNotMaskedClosing=5,
        CutNotMaskedClosing=6,
        ThreatMaskedClosing=7,
        CutMaskedClosing=8,
        ThreatUnknown=9,
        LostControl = 10,
        ThreatUnknownAndLostControl=11
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(VlvCombination))]
    public class VlvCombination : IVlvCombination
    {
        public VlvCombination(int Index, IEnumerable<IVlv> Vlvs)
        {
            _index = Index;
            _vlvList.AddRange(Vlvs);
        }

        [DataMember]
        private int _index = 0;
        public int Index { get { return _index; } }

        [DataMember]
        private List<IVlv> _vlvList = new List<IVlv>();
        public IList<IVlv> VlvList { get { return _vlvList; } }
    }

}
