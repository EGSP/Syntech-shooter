using System.Collections;
using System.Collections.Generic;
using DamageEffects;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlasmaBehaviourPreset", menuName = "ScriptableObjects/DamageBehaviours/PlasmaBehaviour", order = 1)]
public class PlasmaBehaviourPreset : DamageBehaviourPreset
{
    // Радиус распрстранения эффекта
    [SerializeField] private float Radius;
    // Урон наносимый эффектом
    [SerializeField] private float Damage;

    // Продолжительность жизни эффекта
    [SerializeField] private float EffectEmmitTime;
    // Периодичность эффекта
    [SerializeField] private float EffectDurTime;

    public override DamageBehaviour GetDamageBehaviour()
    {
        return new PlasmaBehaviour(EffectEmmitTime, EffectDurTime, Damage);
    }
}
