using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponMagazineData
{
    public WeaponMagazineData(WeaponMagazinePreset _Preset)
    {
        BulletID = _Preset.BulletID;
        Capacity = _Preset.Capacity;
        Count = Capacity;

        reloadFactor = _Preset.ReloadFactor;
    }

    // Идентификатор пули
    public readonly string BulletID;
    // Модификатор изменения времени перезарядки
    public readonly float reloadFactor;

    // Общая вместимость магазина
    public readonly int Capacity;
    // Текущее количество боеприпасов в магазине
    public int Count { get; set; }

    /// <summary>
    /// Недостающее количество боеприпасов
    /// </summary>
    public int NeedAmmo
    {
        get
        {
            return Capacity - Count;
        }
    }
    
    /// <summary>
    /// Наличие боеприпасов в очереди на выстрел
    /// </summary>
    public bool CountIsZero 
    {
        get => (Count > 0) ? false : true;
    }

    /// <summary>
    /// Добавление боеприпасов
    /// </summary>
    public void Fill(int value)
    {
        Count += value;
    }
}
