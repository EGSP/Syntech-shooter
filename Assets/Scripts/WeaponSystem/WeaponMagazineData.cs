using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponMagazineData
{
    public WeaponMagazineData(WeaponMagazinePreset _Preset)
    {
        BulletID = _Preset.BulletID;

        AbsoluteCapacity = _Preset.AbsoluteCapacity;
        ActiveCapacity = _Preset.ActiveCapacity;

        reloadFactor = 1/_Preset.ReloadFactor;

        ActiveCount = ActiveCapacity;
    }

    // Идентификатор пули
    public readonly string BulletID;

    // Общая вместимость магазина (запасник)
    public readonly int AbsoluteCapacity;
    // Вместимость боеприпасов, которые будут находится в очереди на выстрел
    public readonly int ActiveCapacity;

    // Модификатор изменения времени перезарядки
    private readonly float reloadFactor;

    // Текущее количество боеприпасов в запасе магазина
    private int AbsoluteCount;
    // Текущее количество боеприпасов в очереди на выстрел
    public int ActiveCount { get; set; }

    /// <summary>
    /// Наличие боеприпасов в очереди на выстрел
    /// </summary>
    public bool Availability 
    {
        get => (ActiveCount > 0) ? true : false;
    }

    /// <summary>
    /// Возвращает количество мест под новые боеприпасы
    /// </summary>
    public int SpaceCapacity
    {
        get => AbsoluteCapacity - AbsoluteCount;
    }

    /// <summary>
    /// Перезарядка магазина при помощи запасника
    /// </summary>
    /// <returns>Возвращает успех операции</returns>
    public bool Reload()
    {
        int needAmmo = ActiveCapacity - ActiveCount;
        int takenAmmo = Mathf.Min(needAmmo, AbsoluteCount);

        ActiveCount += takenAmmo;
        AbsoluteCount -= takenAmmo;

        // Если было что-то заряжено
        if(takenAmmo != 0)
        {
            return true;
        }

        // Ничего не было заряжено, но патроны остались
        if(ActiveCount != 0)
        {
            return true;
        }

        // Пустой магазин
        return false;
    }
    
    /// <summary>
    /// Пополнение запасов. Пополняется только запасник магазина
    /// </summary>
    public void Fill(int value)
    {
        AbsoluteCount += value;

        if (AbsoluteCount > AbsoluteCapacity)
        {
            throw new System.Exception("Переполнение боеприпасов! Используйте проверку SpaceCapacity");
        }
    }
    
}
