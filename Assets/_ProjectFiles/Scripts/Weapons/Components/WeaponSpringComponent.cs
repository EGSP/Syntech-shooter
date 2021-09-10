using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpringComponent : MonoBehaviour
{
    public WeaponSpringPreset Preset;
    public void Awake()
    {
        FireRateFactor = Preset.FireRateFactor;
    }

    // Множитель скорострельности
    public float FireRateFactor { get; private set; }
}
