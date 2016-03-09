using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface IVlvCombination
    {
        int Index { get; }
 //       string Name { get; }
        IList<IVlv> VlvList { get; }

        //bool isThreatNotMaskedClosed { get; }
        //bool isCutNotMaskedClosed { get; }
        //bool isThreatMaskedClosed { get; }
        //bool isCutMaskedClosed { get; }

        //bool isThreatNotMaskedClosing { get; }
        //bool isCutNotMaskedClosing { get; }
        //bool isThreatMaskedClosing { get; }
        //bool isCutMaskedClosing { get; }

        //bool isThreatUnknown { get; }
        //bool isOk { get; }


    }
}
