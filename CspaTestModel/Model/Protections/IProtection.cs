using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CspaTestModel.Model.Protections
{
    /// <summary>
    /// Интерфейс объединяет все что присуще всем защитам
    /// </summary>
    /// 
    public interface IProtection<T> : IProtection
    {
        IList<T> Inputs { get; }
    }


    public interface IProtection    {
        string Name { get; set; }
        Guid Id { get; set; }

        /// <summary>
        /// Признак выполненного условия срабатывания защиты
        /// </summary>
        IState<bool> IsThreat { get; }

        /// <summary>
        /// Признак срабатывания защиты
        /// </summary>
        IState<bool> IsActive { get; }

        /// <summary>
        /// Признак игнорирования защиты
        /// </summary>
        IState<bool> IsIgnored { get; }

        /// <summary>
        /// Условие деблокирования защиты
        /// </summary>
        IState<bool> DeblockCondition { get; }

        /// <summary>
        /// Команда на деблокирование защиты
        /// </summary>
        void Deblock();

        /// <summary>
        /// Признак того что защита находится в деблокированном состоянии
        /// </summary>
        IState<bool> IsDeblocked { get; }

        /// <summary>
        /// Признак наличия маски на выходе защиты
        /// </summary>
        IState<bool> IsMasked { get; }

        /// <summary>
        /// Команда Установить маску на выход защиты
        /// </summary>
        void SetMask();

        /// <summary>
        /// Команда сбросить маску на выход защиты
        /// </summary>
        void ResetMask();

        /// <summary>
        /// Выполняет условия срабатывания для данной защиты
        /// </summary>
        void ActivateThreat();


        /// <summary>
        /// Переводит объекты ТУ в состояние при котором происходит немедленное срабатывание защиты (с изменением уставок)
        /// </summary>
        void Activate();


        /// <summary>
        /// Переводит объекты ТУ в состояние при котором происходит исчезновение умловий срабатывания защиты
        /// </summary>
        void InActivate();


        /// <summary>
        /// Уставка времени на срабатывание защиты при выполненных условиях срабатывания
        /// </summary>
        int TimePreset { get; set; }

    }

    public interface IProtectionInput<T>
    {
        string Name { get; set; }

        T Object { get; }

        bool IsThreat { get; }

        bool IsActive { get; }

        bool IsIgnored { get; }

        bool IsDeblocked { get; }

        bool IsMasked { get; }

        /// <summary>
        /// Команда Установить маску на выход защиты
        /// </summary>
        void SetMask();

        /// <summary>
        /// Команда сбросить маску на выход защиты
        /// </summary>
        void ResetMask();

        /// <summary>
        /// Выполняет условия срабатывания для данной защиты
        /// </summary>
        void ActivateThreat();

        void InActivateThreat();
    }
}
