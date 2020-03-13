using System.Collections.Generic;
using UnityEngine;

namespace DamageEffects
{
    public class FireBehaviour : DurationDamageBehaviour
    {
        public FireBehaviour(
            float _EmmitTime, float _Duration,
            float _EffectEmmitTime, float _EffectDurTime,
            float _Radius, float _Damage, bool _IsParent = true)
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
        

        // Продолжительность жизни эффекта
        private float EffectEmmitTime;
        // Периодичность эффекта
        private float EffectDurTime;

        // Является ли данный объект родителем
        // Объекты созданные родителем не могут распространять огонь
        private bool IsParent;
        

        public override void OnImpactAll(List<RaycastHit> points)
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

        public override void OnPenetrate(List<Vector3> outputPoints, Vector3 direction)
        {
            return;
        }

        public override DamageBehaviour Update(float deltaTime)
        {
            return base.Update(deltaTime);
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
            var colls = Physics.OverlapSphere(Carrier.transform.position, Radius, LifeLayerMask);

            Debug.Log(colls.Length);
            for (int i = 0; i < colls.Length; i++)
            {
                var coll = colls[i];
                Debug.Log(coll.name);

                var lifeComponent = coll.GetComponent<LifeComponent>();

                if (lifeComponent != null)
                {
                    lifeComponent.AddEffect(new FireEffect(EffectEmmitTime, EffectDurTime, Damage));
                }
            }
        }
    }
}
