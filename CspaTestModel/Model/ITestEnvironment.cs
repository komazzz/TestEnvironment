using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CspaTestModel.Model.Protections;

namespace CspaTestModel.Model
{
    public interface ITestEnvironment:IDisposable
    {
        IProcessDataProvider DataProvider { get; set; }
        /// <summary>
        /// Перечень задвижек на ТУ
        /// </summary>
        List<IVlv> VlvList { get; set; }
        
        /// <summary>
        /// Перечень НПСок на ТУ
        /// </summary>
        List<INps> NpsList { get; set; }

        /// <summary>
        /// Объект технологического участка
        /// </summary>
        ITu Tu { get; set; }

        /// <summary>
        /// Все защиты 
        /// </summary>
        List<IProtection> Protections { get; set; }

        /// <summary>
        /// Перечень защит по потере связи с НПС 
        /// </summary>
      //  List<IConnectionLostProtection> ConnectionLostProtections { get; set; }

    }




}
