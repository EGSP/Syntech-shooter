using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMagazineComponent : MonoBehaviour
{
    public WeaponMagazinePreset Preset;

    public WeaponMagazineData Take()
    {
        return new WeaponMagazineData(Preset);
    }
}
