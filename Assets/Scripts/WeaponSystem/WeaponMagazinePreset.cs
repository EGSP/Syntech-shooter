using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewWeaponMagazinePreset", menuName = "ScriptableObjects/WeaponMagazinePreset", order = 1)]
public class WeaponMagazinePreset: ScriptableObject
{
    public string BulletID;
    [Space]
    [Header("Constant")]
    // Общая вместимость магазина
    public int AbsoluteCapacity;
    // Вместимость боеприпасов, которые будут находится в очереди на выстрел
    public int ActiveCapacity;
    [Range(0.1f,5)]
    public float ReloadFactor;
    
    
}
