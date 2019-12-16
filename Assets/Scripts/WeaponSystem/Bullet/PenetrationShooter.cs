using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PenetrationShooter : MonoBehaviour
{
    [Range(0, 5)]
    // Толщина пробития стандартного покрытия (Penetration Tresshold == 1)
    [SerializeField] private float PenetrationThikness;

    [SerializeField] private LayerMask LayerMask;

    // Коллайдер используемый для всех вычислений пробития 
    [SerializeField] private SphereCollider PenetrationCollider;

    public float Delay;
    private float delay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            delay = Delay;

            var hits = ComputePenetration();

            var bullet = PoolManager.Instance.Take("9MM") as BulletComponent;
            bullet.Push(hits.Last().point, hits.Last().normal);
            bullet.transform.position = transform.position;
            bullet.transform.LookAt(hits.Last().point);

            for (int i = 0; i < hits.Count-1; i++)
            {
                var hit = hits[i];
                var effect = EffectManager.Instance.Take("IS");
                effect.transform.position = hit.point;
                effect.transform.rotation = Quaternion.LookRotation(hit.normal);
                effect.PlayEffect();
            }
            
        }
    }

    public List<RaycastHit> ComputePenetration()
    {
        var hits = Physics.RaycastAll(transform.position, transform.forward, 50, LayerMask).OrderBy(x => x.distance).ToArray();

        if (hits.Length == 0)
            return new List<RaycastHit>();

        // Точки в которые точно попали
        List<RaycastHit> impactPoints = new List<RaycastHit>();

        // Проходимся по всем препятствиям
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            
            impactPoints.Add(hit);
            

            // Наличие компонента пробития, если его нет, то пробить нельзя
            var hitPenetrationComponent = hit.collider.GetComponent<PenetrationComponent>();
            if (hitPenetrationComponent == null)
                break;

            // Сопротивление пробитию
            var penetrationTresshold = hitPenetrationComponent.PenetrationTresshold;

            // Конечная сила пробития
            var penetrationForce = PenetrationThikness * penetrationTresshold;
            if (penetrationForce == 0)
                break;

            // Позиция выхода пробития
            var penetrationExit = hit.point + transform.forward * PenetrationThikness * penetrationTresshold;

            float distance;
            Vector3 direction;
            bool isOverlapped = Physics.ComputePenetration(
                PenetrationCollider, penetrationExit, transform.rotation,
                hit.collider, hit.transform.position, hit.transform.rotation,
                out direction, out distance);

            bool penetrationSuccess = !isOverlapped;

            // Если не удалось пробить
            if (!penetrationSuccess)
            {
                break;
            }
            else
            {
                // Если стена была пробита, но дальше только пустое пространство, то стена станет непробитой
                if (i + 1 == hits.Length)
                {
                    break;
                }
            }

        }

        return impactPoints;

    }


}