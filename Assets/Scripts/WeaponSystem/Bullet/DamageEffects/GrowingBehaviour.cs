using System.Collections.Generic;
using UnityEngine;

namespace DamageEffects
{
    public class GrowingBehaviour : DamageBehaviour
    {
        public GrowingBehaviour(float _BaseDamage, float _DeltaDamage)
        {
            BaseDamage = new DamageData();
            BaseDamage.armourModifier = 1f;
            BaseDamage.baseDamage = _BaseDamage;

            DeltaDamage = _DeltaDamage;
        }

        // Базовый урон эффекта
        private DamageData BaseDamage { get; }
        // Нарастающий урон за каждую пораженную цель
        private float DeltaDamage { get; }

        public override DamageBehaviour Clone()
        {
            return new GrowingBehaviour(BaseDamage.baseDamage, DeltaDamage);
        }

        public override void Merge(DamageBehaviour behaviour)
        {
            return;
        }

        public override void OnImpactLife(List<LifeComponent> components)
        {
            int modificator = 0; 
            // Урон растет с каждой новой целью
            for(int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                component.Hurt(new DamageData() {
                    baseDamage = BaseDamage.baseDamage + (DeltaDamage * modificator),
                    armourModifier = BaseDamage.armourModifier });

                modificator++;
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
