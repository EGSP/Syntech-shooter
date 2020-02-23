using System.Collections.Generic;
using UnityEngine;

namespace DamageEffects
{
    public class AcidBehaviour : DamageBehaviour
    {
        public AcidBehaviour(
            float _Duration,
            float _Damage,
            float _SpeedModifier,
            float _ModifierTime)
        {
            // AcidEffect.ID
            ID = 103;
            Duration = _Duration;
            Damage = _Damage;
            SpeedModifier = _SpeedModifier;
            ModifierTime = _ModifierTime;
        }
        
        // Это все настройки эффекта
        private float Duration { get; }
        private float Damage { get; }
        private float SpeedModifier { get; }
        private float ModifierTime { get; }

        public override DamageBehaviour Clone()
        {
            return new AcidBehaviour(Duration, Damage, SpeedModifier, ModifierTime);
        }

        public override void Merge(DamageBehaviour behaviour)
        {
            return;
        }

        public override void OnImpactLife(List<LifeComponent> components)
        {
            // Добавляем эффект возгорания на все компоненты
            for (int i = 0; i < components.Count; i++)
            {
                components[i].AddEffect(new AcidEffect(Duration, Damage, SpeedModifier, ModifierTime));
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
            return null;
        }
    }
}
