using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewWeaponMagazinePreset", menuName = "ScriptableObjects/WeaponMagazinePreset", order = 1)]
public class WeaponMagazinePreset: ScriptableObject
{
    public string BulletID;
    [Space]
    // Вместимость магазина
    public int Capacity;
    /// <summary>
    /// Коэфицент времени перезарядки магазина
    /// </summary>
    public float ReloadFactor;
    
    
}
