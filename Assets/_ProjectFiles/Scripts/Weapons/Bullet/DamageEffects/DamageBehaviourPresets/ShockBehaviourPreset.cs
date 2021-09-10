using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShockBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/ShockBehaviour", order = 1)]
public class ShockBehaviourPreset : DamageBehaviourPreset
{
    [SerializeField] private float ShockTime;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new ShockBehaviour(ShockTime);
    }
}
