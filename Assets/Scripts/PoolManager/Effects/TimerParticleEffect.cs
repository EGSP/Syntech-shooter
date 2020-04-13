using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace EffectObjects
{
    /// <summary>
    /// Не рекомендуется использовать
    /// </summary>
    public class TimerParticleEffect : ParticleEffect
    {
        /// <summary>
        /// Время проигрывания эффекта, после которого он выключается
        /// </summary>
        [SerializeField] private float PlayTime;

        private TimerCallbacker timerCallbacker;

        public override void Initialize()
        {
            base.Initialize();

            timerCallbacker = new TimerCallbacker(PlayTime);
            timerCallbacker.OnEmmitionEndCallback += StopEffect;
        }
        

        public override void PlayEffect()
        {
            base.PlayEffect();

            timerCallbacker.Reset();
            EffectManager.Instance.OnUpdate += timerCallbacker.Update;
        }

        public override void ResetEffect()
        {
            base.ResetEffect();

            timerCallbacker.Reset();
            EffectManager.Instance.OnUpdate += timerCallbacker.Update;
        }

        public override void StopEffect()
        {
            base.StopEffect();

            EffectManager.Instance.OnUpdate -= timerCallbacker.Update;
        }

    }
}
