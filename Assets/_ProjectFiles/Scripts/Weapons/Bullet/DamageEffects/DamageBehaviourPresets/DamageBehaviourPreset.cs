using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DamageEffects;

public abstract class DamageBehaviourPreset : ScriptableObject
{
    /// <summary>
    /// Возвращает DamageBehaviour
    /// </summary>
    public abstract DamageBehaviour GetDamageBehaviour();
}
