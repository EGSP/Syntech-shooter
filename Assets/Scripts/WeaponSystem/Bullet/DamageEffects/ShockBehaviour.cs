using System.Collections.Generic;
using UnityEngine;

namespace DamageEffects
{
    public class ShockBehaviour : DamageBehaviour
    {
        public ShockBehaviour(float _ShockTime)
        {
            // ShockEffect.ID
            ID = 102;
            ShockTime = _ShockTime;
        }

        private float ShockTime { get; }

        public override DamageBehaviour Clone()
        {
            return new ShockBehaviour(ShockTime);
        }

        public override void Merge(DamageBehaviour behaviour)
        {
            return;
        }

        public override void OnImpactLife(List<LifeComponent> components)
        {
            // Добавляем эффект шока на все компоненты
            for (int i = 0; i < components.Count; i++)
            {
                components[i].AddEffect(new ShockEffect(_ShockTime: ShockTime));
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
