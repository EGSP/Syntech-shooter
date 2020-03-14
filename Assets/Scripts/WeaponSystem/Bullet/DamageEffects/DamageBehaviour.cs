using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DamageEffects
{
    public abstract class DamageBehaviour
    {
        public int ID = -1;

        /// <summary>
        /// Проходимость сквозь активную броню
        /// </summary>
        public bool AddThroughArmour { get; protected set; }

        /// <summary>
        /// Носитель эффекта
        /// </summary>
        protected LifeComponent Carrier;

        /// <summary>
        /// Слой принадлежащий объектам LifeComponent
        /// </summary>
        protected LayerMask LifeLayerMask;

        public virtual void Start(LifeComponent _Carrier, LayerMask _LifeLayerMask)
        {
            Carrier = _Carrier;
            LifeLayerMask = _LifeLayerMask;

            Carrier.AddDamageBehaviour(this);
        }

        public abstract DamageBehaviour Update(float deltaTime);

        public abstract void OnImpactAll(List<RaycastHit> points);

        public abstract void OnImpactLife(List<LifeComponent> components);

        public abstract void OnPenetrate(List<Vector3> outputPoints, Vector3 direction);

        public abstract void Merge(DamageBehaviour behaviour);

        
        public abstract DamageBehaviour Clone();
    }

    // Поведение срабатывающее с определенной периодичностью
    public abstract class DurationDamageBehaviour: DamageBehaviour
    {
        public DurationDamageBehaviour(float _EmmitTime, float _Duration)
        {
            EmmitTime = _EmmitTime;
            emmitTime = EmmitTime;

            Duration = _Duration;
            duration = Duration;
        }

        protected float EmmitTime;
        private float emmitTime;

        protected float Duration;
        private float duration;

        public override DamageBehaviour Update(float deltaTime)
        {
            emmitTime -= deltaTime;

            duration -= deltaTime;
            if (duration < 0)
            {
                duration = Duration;

                OnDuration();
            }

            if (emmitTime < 0)
                return null;

            return this;
        }

        protected abstract void OnDuration();

        public override void Merge(DamageBehaviour behaviour)
        {
            var anotherBeh = behaviour as DurationDamageBehaviour;

            emmitTime = anotherBeh.EmmitTime;
            Duration = anotherBeh.Duration;
        }
    }
}
