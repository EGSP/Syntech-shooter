using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RayCaster : MonoBehaviour
{
    public float Distance;
    public LayerMask mask;
    public Vector3 Dir;
    public Vector3 hitPos;
    public Vector3 hitNormal;

    public Vector3 forPL;

    // Сумма плоскости и направления
    public Vector3 sumFS;

    
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position,transform.up*-1,out hit, Distance, mask))
        {
            
            hitPos = hit.point;
            hitNormal = hit.normal;

            // Вектор всегда будет устремлён по склону
            Vector3 surfParall = hit.point - transform.position - hit.normal * Vector3.Dot(hit.point-transform.position,hit.normal );
            //print(Vector3.Dot((surfParall + hit.point).normalized, hit.normal));

            // Forward projection
            Vector3 forpl = Vector3.ProjectOnPlane(transform.forward,hit.normal);
            // -1 В гору, 0 - прямо, 1 - спуск 
            // print(Vector3.Dot(transform.forward, hit.normal));

            // Сумма векторов
            sumFS = (forpl + surfParall).normalized*Extensions.Middle(forpl.magnitude,surfParall.magnitude) + hit.point;

            // Мировая позиция
            forPL = forpl+hit.point;
            Dir = surfParall+hit.point;

            
        }
        
    }

    


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Ray
        Gizmos.DrawLine(transform.position,hitPos);
        Gizmos.color = Color.magenta; // Surface with normal
        Gizmos.DrawLine(hitPos, Dir);
        Gizmos.DrawLine(hitPos, hitPos + hitNormal);
        Gizmos.color = Color.blue; // Forward
        Gizmos.DrawLine(hitPos, forPL);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(hitPos, sumFS);
    }
}
