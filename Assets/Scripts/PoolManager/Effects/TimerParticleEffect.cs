using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace EffectObjects
{
    public class TimerParticleEffect : ParticleEffect
    {
        /// <summary>
        /// Время проигрывания эффекта
        /// </summary>
        [SerializeField] private float PlayTime;

        private TimerCallbacker RemoveTimer;

        public override void Initialize()
        {
            base.Initialize();

            RemoveTimer = new TimerCallbacker(PlayTime);

            RemoveTimer.OnEmmitionEndCallback += () =>
            {
                StopEffect();
            };
        }

        public void Update()
        {
            RemoveTimer.Update(Time.deltaTime);
        }

        public override void PlayEffect()
        {
            base.PlayEffect();

            RemoveTimer.Reset();
        }

    }
}
