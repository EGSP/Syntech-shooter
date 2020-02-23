using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeEffects
{
    public interface IShockable
    {
        /// <summary>
        /// Работает ли в данный момент замыкание
        /// </summary>
        bool IsShocked { get; }

        /// <summary>
        /// Активация замыкания
        /// </summary>
        void Shock(float _ShockTime);
    }
}
