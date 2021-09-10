using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewAcidBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/AcidBehaviour", order = 1)]
public class AcidBehaviourPreset : DamageBehaviourPreset
{
    [SerializeField] private float Duration;
    [SerializeField] private float Damage;
    [SerializeField] private float SpeedModifier;
    [SerializeField] private float ModifierTime;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new AcidBehaviour(Duration, Damage, SpeedModifier, ModifierTime);
    }
}
