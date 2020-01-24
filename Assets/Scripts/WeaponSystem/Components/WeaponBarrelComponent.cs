using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBarrelComponent : MonoBehaviour
{
    public WeaponBarrelPreset Preset;
    public void Awake()
    {
        DamageFactor = Preset.DamageFactor;
        AdditionalBullets = Preset.AdditionalBullets;
    }

    public float DamageFactor { get; private set; }
    // Количество дополнительных пуль за выстрел
    public float AdditionalBullets { get; private set; }
}
