using System.Collections.Generic;
using UnityEngine;

namespace DamageEffects
{
    public class PlasmaBehaviour : DamageBehaviour
    {
        public PlasmaBehaviour(
            float _EffectEmmitTime,
            float _EffectDurTime,
            float _Damage)
            : base()
        {
            // ID == PlasmaEffect.ID
            ID = 101;
            ArmourPenetration = true;

            EffectEmmitTime = _EffectEmmitTime;
            EffectDurTime = _EffectDurTime;
            
            Damage = _Damage;
            
        }
        // Урон наносимый эффектом
        private float Damage;


        // Продолжительность жизни эффекта
        private float EffectEmmitTime;
        // Периодичность эффекта
        private float EffectDurTime;


        public override DamageBehaviour Clone()
        {
            return new PlasmaBehaviour(EffectEmmitTime, EffectDurTime, Damage);
        }

        public override void OnImpactLife(List<LifeComponent> components)
        {
            // Добавляем эффект возгорания на все компоненты
            for (int i = 0; i < components.Count; i++)
            {
                components[i].AddEffect(new PlasmaEffect(EffectEmmitTime, EffectDurTime, Damage));
            }
        }

        public override void OnImpactAll(List<RaycastHit> points)
        {
            return;
        }

        public override void OnPenetrate(List<Vector3> outputPoints, Vector3 direction)
        {
            return;
        }

        public override DamageBehaviour Update(float deltaTime)
        {
            // Сразу пропадает при включении
            return null;
        }

        public override void Merge(DamageBehaviour behaviour)
        {
            return;
        }
    }
}
