using System;
using ProcessDataProviders.OpcDa;
using CspaTestModel.Model;
using CspaTestModel;
using System.Collections.Generic;

namespace ProcessDataProvidersUT
{
    public abstract class BaseOpcDaProcessProviderTest
    {
        protected readonly string[] TestingItemTypes = new string[] { "Bool", "Int16", "String" };

        protected readonly Random Randomizer = new Random();

        protected enum TagType { Good, ReadError, WriteError }

        protected virtual OpcDaProcessDataProvider GetEmptyProvider()
        {
            var provider = new OpcDaProcessDataProvider();
            provider.OpcName = "Matrikon.OPC.Simulation.1";
            provider.Host = "localhost";
            provider.UpdateRate = 200;
            return provider;
        }

        protected virtual List<IProcessItemDefinition> GetListOfTags(TagType TagType, IProcessDataProvider ProcessDataProvider)
        {
            string tagGroup = null;
            switch (TagType)
            {
                case TagType.Good: tagGroup = "GoodTags";
                    break;
                case TagType.ReadError: tagGroup = "ReadError";
                    break;
                case TagType.WriteError: tagGroup = "WriteError";
                    break;
            }

            List<IProcessItemDefinition> retVal = new List<IProcessItemDefinition>();
            foreach (var tagType in TestingItemTypes)
            {
                string tagAddress = String.Format("{0}.{1}", tagGroup, tagType);
                retVal.Add(GetItemDef(tagType, tagAddress, ProcessDataProvider));
            }
            return retVal;
        }

        protected virtual IProcessItemDefinition GetItemDef(String ItemType, String ItemAddress, IProcessDataProvider ProcessDataProvider)
        {
            switch (ItemType)
            {
                case "Bool": return new ProcessItem<Boolean>(ItemAddress, ProcessDataProvider);
                case "Int16": return new ProcessItem<Int16>(ItemAddress, ProcessDataProvider);
                case "String": return new ProcessItem<String>(ItemAddress, ProcessDataProvider);
                default: return new ProcessItem<String>(ItemAddress, ProcessDataProvider);
            }
        }

        protected virtual object GenerateItemValue(string ItemType)
        {
            switch (ItemType)
            {
                case "Bool": return true;
                case "Int16": return (Int16)Randomizer.Next(Int16.MaxValue);
                case "String": return String.Format("String{0}", Randomizer.Next(Int16.MaxValue));
                default: throw new ArgumentException("Неизвестный тип данных");
            }
        }
    }
}
