using System;
namespace CspaTestModel.Model
{
    public interface IVlv
    {
        int Index { get; }
        string Name { get; }
        
        IState<VlvInputProtectionState> InputProtectionState { get; }
        IState<VlvPositionEnum> Position { get; }
        IState<bool> IsMasked { get; }

        void SetPosition(VlvPositionEnum Position);
        VlvPositionEnum GetPosition();
        
        
        void SetMaskOn();
        void SetMaskOff();

        bool GetMaskState { get; }
        
    }

    public interface IState<T>
    {
        T StateValue {get;set;}
        DateTime TimeStamp {get;}

        bool NewerThan(IState<T> State);
    }

    public enum VlvPositionEnum 
    {   Opened=1, 
        Closing=2, 
        Closed=3, 
        Opening=4,
        Failure=5, 
        Unknown=6  
    }





}
