using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHEIBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/HEIBehaviour", order = 1)]
public class HEIBehaviourPreset : DamageBehaviourPreset
{
    [SerializeField] private float Damage;
    [SerializeField] private float Radius;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new HEIBehaviour(Damage, Radius);
    }
}
