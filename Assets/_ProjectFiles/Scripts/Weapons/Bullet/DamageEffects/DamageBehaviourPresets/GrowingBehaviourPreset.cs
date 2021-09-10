using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrowingBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/GrowingBehaviour", order = 1)]
public class GrowingBehaviourPreset : DamageBehaviourPreset
{
    // Базовый урон эффекта
    [SerializeField] private float BaseDamage;
    // Нарастающий урон за каждую пораженную цель
    [SerializeField] private float DeltaDamage;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new GrowingBehaviour(BaseDamage, DeltaDamage);
    }
}
