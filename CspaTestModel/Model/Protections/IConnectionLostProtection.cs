using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model.Protections
{
    public enum ConnectionEmulationStatusEnum { NotStarted, InProgress, Paused}

    public interface IConnectionLostProtection:IProtection
    {

        void StartCounterEmulation();
        void PauseCounterEmulation();
        void ResumeCounterEmulation();
        void StopCounterEmulation();
        ConnectionEmulationStatusEnum EmulationStatus { get; }
        uint ConnectionDiagTimerPreset { get; set; }


 
    }
}
