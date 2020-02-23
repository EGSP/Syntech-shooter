using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeComponent : LifeComponent
{
    public LayerMask MedkitsLayer;
    [Tooltip("Поднятие аптечек")]
    public float OverlapRadius;
    public Vector3 CentreOffset;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        var colliders = Physics.OverlapSphere(transform.position + CentreOffset, OverlapRadius, MedkitsLayer, QueryTriggerInteraction.Collide);    

        for(int i = 0; i < colliders.Length; i++)
        {
            var medkit = colliders[i].GetComponent<Medkit>();

            if (medkit != null)
                AddEffect(medkit.Use());
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + CentreOffset, OverlapRadius);
    }
}
