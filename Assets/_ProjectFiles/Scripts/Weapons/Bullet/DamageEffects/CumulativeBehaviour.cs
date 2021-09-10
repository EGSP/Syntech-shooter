using System.Collections.Generic;
using UnityEngine;

namespace DamageEffects
{
    public class CumulativeBehaviour : DamageBehaviour
    {
        public CumulativeBehaviour(float _Damage, float _Radius)
        {
            ID = 105;
            Damage = new DamageData();
            Damage.armourModifier = 0.4f;
            Damage.baseDamage = _Damage;

            Radius = _Radius;
        }

        public DamageData Damage { get; private set; }
        public float Radius { get; private set; }

        public override DamageBehaviour Clone()
        {
            return new CumulativeBehaviour(Damage.baseDamage, Radius);
        }

        public override void Merge(DamageBehaviour behaviour)
        {
            return;
        }

        public override void OnImpactAll(List<RaycastHit> points)
        {
            return;
        }

        public override void OnImpactLife(List<LifeComponent> components)
        {
            return;
        }

        public override void OnPenetrate(List<Vector3> outputPoints, Vector3 direction)
        {
            var layerMask = GameManager.Instance.LifeLayerMask;
            var checkMask = GameManager.Instance.ShootRayMask;

            for (int i = 0; i < outputPoints.Count; i++)
            {
                var hit = outputPoints[i];
                var colls = Physics.OverlapSphere(hit, Radius, layerMask);

                // Проходимся по всем в радусе поражения
                for (int c = 0; c < colls.Length; c++)
                {
                    var coll = colls[c];

                    var life = coll.GetComponent<LifeComponent>();
                    if (life != null)
                    {
                        RaycastHit rayHit;
                        if (Physics.Raycast(hit, coll.transform.position - hit, out rayHit, Radius, checkMask))
                        {
                            // Если нет объектов между точкой взрыва и целью
                            if (rayHit.collider == coll)
                            {
                                life.Hurt(Damage);
                            }
                        }
                    }
                }

                // Спавним эффект на месте взрыва (TimerParticle)
                var VFX = EffectManager.Instance.Take(ID.ToString());

                VFX.PlayEffect(hit, direction);
            }
        }

        public override DamageBehaviour Update(float deltaTime)
        {
            return null;
        }
    }
}
