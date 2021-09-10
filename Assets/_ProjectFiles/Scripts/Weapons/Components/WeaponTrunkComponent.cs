using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrunkComponent : MonoBehaviour
{
    public WeaponTrunkPreset Preset;

    public void Awake()
    {
        SpreadXFactor = Preset.SpreadXFactor;
        SpreadYFactor = Preset.SpreadYFactor;
    }

    public float SpreadXFactor { get; private set; }
    public float SpreadYFactor { get; private set; }
}
