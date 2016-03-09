using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface ITu
    {
        void SetTuOff();
        void SetTuOn();
        bool isOn { get; }
    }
}
