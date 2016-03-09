using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model
{
    public interface INps
    {
        /// <summary>
        /// Название НПС
        /// </summary>
         string Name { get;}

        /// <summary>
        /// Порядковый номер в модели нефтепровода. нужен для восставновления свойств из экселевского файла
        /// </summary>
         uint SchemaIndex { get; }

        /// <summary>
        /// Отсекает НПС от нефтепровода
        /// </summary>
         void Cut();

        /// <summary>
        /// Отменяет отсечение НПС от нефтепровода
        /// </summary>
         void UnCut();

        /// <summary>
        /// Признак отсечения НПС от нефтепровода
        /// </summary>
         bool isCut{get;}

        /// <summary>
        /// Признак того что НПС конечная на ТУ
        /// </summary>
         bool isLast { get; set; }
         IProcessDataProvider ProcessDataProvider { get; set; }

         List<IMna> MnaList { get; set; }

    }
}
