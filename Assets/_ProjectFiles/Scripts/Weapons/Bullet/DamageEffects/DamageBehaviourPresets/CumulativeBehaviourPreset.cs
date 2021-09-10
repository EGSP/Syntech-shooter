using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewCumulativeBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/CumulativeBehaviour", order = 1)]
public class CumulativeBehaviourPreset : DamageBehaviourPreset
{
    [SerializeField] private float Damage;
    [SerializeField] private float Radius;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new CumulativeBehaviour(Damage, Radius);
    }
}
