#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PenetrationTestController : MonoBehaviour
{
    [Range(0,5)]
    // Толщина пробития стандартного покрытия (Penetration Tresshold == 1)
    [SerializeField] private float PenetrationThikness;

    [SerializeField] private LayerMask LayerMask;

    // Коллайдер используемый для всех вычислений пробития 
    [SerializeField] private SphereCollider PenetrationCollider;

    public List<GameObject> hitted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnDrawGizmosSelected()
    {
        if (PenetrationCollider == null)
            return;

        Gizmos.color = Color.green;

        var hits = Physics.RaycastAll(transform.position, transform.forward, 50, LayerMask).OrderBy(x=>x.distance).ToArray();

        if (hits.Length == 0)
            return;

        hitted.Clear();
        for(int i = 0; i < hits.Length; i++)
        {
            hitted.Add(hits[i].collider.gameObject);
        }

        Vector3 endpoint = hits[0].point;
        List<Vector3> impactPoints = new List<Vector3>(hits.Length);
        // Проходимся по всем препятствиям
        for(int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];

            // Точка в которую точно попали
            impactPoints.Add(hit.point);
            endpoint = hit.point;

            // Try penetrate
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

        Gizmos.DrawLine(transform.position, endpoint);

        



        
    }
}
