using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewWeaponGripPreset", menuName = "ScriptableObjects/WeaponGripPreset", order = 1)]
public class WeaponGripPreset : ScriptableObject
{
    public float RecoilForceFactor;
}
