using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFireBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/FireBehaviour", order = 1)]
public class FireBehaviourPreset : DamageBehaviourPreset
{
    // Радиус распрстранения эффекта
    [SerializeField] private float Radius;
    // Урон наносимый эффектом
    [SerializeField] private float Damage;

    [SerializeField] private float EmmitTime;
    [SerializeField] private float Duration;

    // Продолжительность жизни эффекта
    [SerializeField] private float EffectEmmitTime;
    // Периодичность эффекта
    [SerializeField] private float EffectDurTime;

    // Является ли данный объект родителем
    // Объекты созданные родителем не могут распространять огонь
    [SerializeField] private bool IsParent;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new FireBehaviour(EmmitTime, Duration, EffectEmmitTime, EffectDurTime, Radius, Damage, IsParent);
    }
}
