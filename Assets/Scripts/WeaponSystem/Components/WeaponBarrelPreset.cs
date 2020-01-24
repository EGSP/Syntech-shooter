using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponBarrelPreset", menuName = "ScriptableObjects/WeaponBarrelPreset", order = 1)]
public class WeaponBarrelPreset : ScriptableObject
{
    public float DamageFactor;
    // Количество дополнительных пуль за выстрел
    public float AdditionalBullets;
}
