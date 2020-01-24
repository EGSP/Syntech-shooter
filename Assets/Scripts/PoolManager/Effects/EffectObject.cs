using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectObject : MonoBehaviour
{
    // Свободен ли сейчас эффект
    public bool IsFree { get; set; }

    public abstract void Initialize();

    public abstract void PlayEffect();

    public abstract void StopEffect();

    public abstract void ResetEffect();
}
