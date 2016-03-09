using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CspaTestModel.Model.Protections;
using CspaTestModel.Model;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CspaTestModel.Protections
{
    [DataContract]
    public class VlvClosingProtection : BaseProtection<IVlvCombination>
    {
        
        VlvClosingProtection(IEnumerable<IVlvCombination> Inputs) : base(Inputs) { }
    }


    
    [DataContract]
    [KnownType("GetTypes")]
    public class BaseProtection<InputsT>:IProtection<InputsT>
    {
        public static Type[] GetTypes()
        {
            return new Type[] { typeof(BaseProtection<InputsT>) };
        }
        public BaseProtection(IEnumerable<InputsT> Inputs)
        {
            this.Inputs = Inputs.ToList();
        }
        //Теои защиты
        [DataMember]
        public IProcessItem<bool> ThreatTag { get; set; }

        [DataMember]
        public IProcessItem<bool> ActiveTag { get; set; }

        [DataMember]
        public IProcessItem<bool> IgnoredTag { get; set; }

        [DataMember]
        public IProcessItem<bool> DeblockedTag { get; set; }

        [DataMember]
        public IProcessItem<bool> DeblockConditionTag { get; set; }

        [DataMember]
        public IProcessItem<bool> DeblockCmdTag { get; set; }

        //Теги маски
        [DataMember]
        public IProcessItem<bool> MaskedTag { get; set; }

        [DataMember]
        public IProcessItem<bool> SetMaskOnTag { get; set; }

        [DataMember]
        public IProcessItem<bool> SetMaskOffTag { get; set; }

        [DataMember]
        public IProcessItem<bool> SetMaskOnConfirmTag { get; set; }

        [DataMember]
        public IProcessItem<bool> SetMaskOffConfirmTag { get; set; }


        protected IState<bool> GetCurentItemState(IProcessItem<bool> ProcessItem)
        {
            bool val = ProcessItem.ReadValue();
            return new State<bool>(val, ProcessItem.TimeStamp);
        }

        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public Guid Id { get; set; }

        public IState<bool> IsThreat
        {
            get { return GetCurentItemState(ThreatTag); }
        }

        public IState<bool> IsActive
        {
            get { return GetCurentItemState(ActiveTag); }
        }

        public IState<bool> IsIgnored
        {
            get { return GetCurentItemState(IgnoredTag);  }
        }

        public IState<bool> DeblockCondition
        {
            get { return GetCurentItemState(DeblockConditionTag); }
        }

        public void Deblock()
        {
            DeblockCmdTag.WriteValue(true);
        }

        public IState<bool> IsDeblocked
        {
            get { return GetCurentItemState(DeblockedTag); }
        }

        public IState<bool> IsMasked
        {
            get { return GetCurentItemState(MaskedTag); }
        }

        public void SetMask()
        {
            SetMaskOnTag.WriteValue(true);
        }

        public void ResetMask()
        {
            SetMaskOffTag.WriteValue(true);
        }

        public virtual void ActivateThreat()
        {
            throw new NotImplementedException();
        }

        public virtual void Activate()
        {
            throw new NotImplementedException();
        }

        public virtual void InActivate()
        {
            throw new NotImplementedException();
        }

        public int TimePreset { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        [DataMember]
        public IList<InputsT> Inputs{get;private set;}
    }

}
