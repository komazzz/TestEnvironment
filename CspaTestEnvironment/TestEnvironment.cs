using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model.Protections;
using CspaTestModel.Model;
using CspaTestModel;
using System.Runtime.Serialization;
using System.IO;
using CspaTestModel.Protections;
using ProcessDataProviders.OpcDa;

namespace CspaEnvironment
{
    public class CspaTestEnvironment
    {
        public CspaTestEnvironment(CspaTestEnvironmentDefinition Definition)
        {

        }
        public ProcessDataProviderManager DataProviderManager { get; private set; }

        public ProcessItemManager ProcessItemManager { get; private set; }

        public VlvManager VlvManager { get; private set; }
        
        public CspaTestEnvironmentDefinition GetDefinition()
        {
            var retVal = new CspaTestEnvironmentDefinition();
            retVal.DataProviders = this.DataProviderManager.DataProviders.Select(a => (OpcDaProcessDataProvider)a).ToList();

            retVal.BoolProcessItems = this.ProcessItemManager.BoolProcessItems.ToList();
            retVal.IntProcessItems = this.ProcessItemManager.IntProcessItems.ToList();
            retVal.FloatProcessItems = this.ProcessItemManager.FloatProcessItems.ToList();
            retVal.DoubleProcessItems = this.ProcessItemManager.DoubleProcessItems.ToList();
            retVal.StringProcessItems = this.ProcessItemManager.StringProcessItems.ToList();

            retVal.VlvList = this.VlvManager.Vlvs.ToList();
            return retVal;
        }
    }




  








}
