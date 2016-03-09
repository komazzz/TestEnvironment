using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Tests.Model;
using CspaTestModel.Model;
using CspaTestModel.Model.Protections;
using CspaTestModel.Protections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace CspaTestModel.Tests.Vlv
{
    public class ThreatStateTest : BaseTest
    {
        public IVlvCombination VlvCombination { get; set; }

        public IProtection<IVlvCombination> VlvProtection { get; set; }

        protected override void DoTest()
        {
            SetInitialState();
            Assert.AreEqual(false, VlvProtection.IsThreat.StateValue);

            IList<IVlv> vlvList = VlvCombination.VlvList;
            foreach (var vlv in vlvList)
            {
                Assert.AreEqual(false, VlvProtection.IsThreat.StateValue);
                vlv.SetPosition(VlvPositionEnum.Closing);
                Wait();
            }
            Wait();
            Assert.AreEqual(true, VlvProtection.IsThreat.StateValue);

            IVlv lastVlvInList =  VlvCombination.VlvList.Last(); 
            lastVlvInList.SetPosition(VlvPositionEnum.Opened);
            Wait();
            Assert.AreEqual(false, VlvProtection.IsThreat.StateValue);

            var vlvListWithoutLast = vlvList.Select(a =>{return (a != lastVlvInList) ? a:null;});
            foreach (var vlv in vlvListWithoutLast)
            {
                Assert.AreEqual(false, VlvProtection.IsThreat.StateValue);
                vlv.SetPosition(VlvPositionEnum.Closed);
                Wait();
            }
            Assert.AreEqual(false, VlvProtection.IsThreat.StateValue);
            lastVlvInList.SetPosition(VlvPositionEnum.Closing);
            Wait();
            Assert.AreEqual(true, VlvProtection.IsThreat.StateValue);

            foreach (var vlv in vlvList)
            {
                vlv.SetPosition(VlvPositionEnum.Opened);
                Wait();
                Assert.AreEqual(false, VlvProtection.IsThreat.StateValue);
            }
        }

        //Перед началом теста открываем задвижки
        protected virtual void SetInitialState()
        {
           // var vlvList = this.TestEnvironment.VlvList;
            //foreach (var vlv in vlvList)
            //    vlv.SetPosition(VlvPositionEnum.Opened);
        }
    }



}
