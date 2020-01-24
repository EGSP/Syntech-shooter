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
        /// Носитель эффекта
        /// </summary>
        protected LifeComponent Carrier;

        public virtual void Start(LifeComponent _Carrier)
        {
            Carrier = _Carrier;
        }

        public abstract DamageBehaviour Update(float deltaTime);

        public abstract void OnImpactWall(RaycastHit[] points);

        public abstract void OnImpactLife(List<LifeComponent> components);

        public abstract void OnPenetrate(RaycastHit[] points, Vector3 direction, float stepLength);

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
    

    public class FireBehaviour : DurationDamageBehaviour
    {
        public FireBehaviour(
            float _EmmitTime, float _Duration,
            float _EffectEmmitTime, float _EffectDurTime,
            float _Radius, float _Damage, bool _IsParent)
            : base(_EmmitTime, _Duration)
        {
            // ID == FireEffect.ID
            ID = 100;

            EffectEmmitTime = _EffectEmmitTime;
            EffectDurTime = _EffectDurTime;

            Radius = _Radius;
            Damage = _Damage;

            IsParent = _IsParent;
        }

        // Радиус распрстранения эффекта
        private float Radius;
        // Урон наносимый эффектом
        private float Damage;

        // Визуальный эффект огня
        private EffectObject VisualEffect;

        // Продолжительность жизни эффекта
        private float EffectEmmitTime;
        // Периодичность эффекта
        private float EffectDurTime;

        // Является ли данный объект родителем
        // Объекты созданные родителем не могут распространять огонь
        private bool IsParent;
        

        public override void OnImpactWall(RaycastHit[] points)
        {
            return;
        }
        
        public override void OnImpactLife(List<LifeComponent> components)
        {
            // Добавляем эффект возгорания на все компоненты
            for(int i = 0; i < components.Count; i++)
            {
                components[i].AddEffect(new FireEffect(EffectEmmitTime, EffectDurTime, Damage));
            }
        }

        public override void OnPenetrate(RaycastHit[] points, Vector3 direction, float stepLength)
        {
            return;
        }

        public override DamageBehaviour Update(float deltaTime)
        {
            return base.Update(deltaTime);
        }

        public override void Start(LifeComponent _Carrier)
        {
            base.Start(_Carrier);
        }

        public override DamageBehaviour Clone()
        {
            return new FireBehaviour(EmmitTime, Duration, EffectEmmitTime, EffectDurTime, Radius, Damage, IsParent);
        }

        protected override void OnDuration()
        {
            if (IsParent == false)
                return;
            
            // Находим LifeComponent объекты и распространяем на них огонь
            var colls = Physics.OverlapSphere(Carrier.transform.position, Radius, layerMask: 13);

            for (int i = 0; i < colls.Length; i++)
            {
                var coll = colls[i];

                var lifeComponent = coll.GetComponent<LifeComponent>();

                if (lifeComponent != null)
                {
                    lifeComponent.AddEffect(new FireEffect(EffectEmmitTime, EffectDurTime, Damage));
                }
            }
        }
    }
}
