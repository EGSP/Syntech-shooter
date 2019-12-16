using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectObject : MonoBehaviour
{
    public abstract void Initialize();

    public abstract void PlayEffect();

    public abstract void StopEffect();

    public abstract void ResetEffect();
}
