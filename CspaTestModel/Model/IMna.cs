using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface IMna
    {
        string Name { get; }
        void SetOn();
        void SetOff();
        MnaStateEnum State { get; }
        void SetUnknown();

        void SetExcluded();
        void ResetExcluded();
        bool isExcluded { get; }
    }

    public enum MnaStateEnum { On, Off, Unknown}
}
