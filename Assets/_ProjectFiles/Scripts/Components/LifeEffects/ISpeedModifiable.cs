using System.Collections.Generic;

namespace LifeEffects
{
    public interface ISpeedModifiable
    {
        float SpeedModifier { get; }
        float ModifierTime { get; }
        /// <summary>
        /// Добавление модификаторов скорости
        /// </summary>
        /// <param name="_SpeedModifier"> Модификатор скорости </param>
        void EnqueueModifier(float _SpeedModifier, float _ModifierTime);
    }
}
