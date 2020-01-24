using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DamageEffects;
public class WeaponMagazineComponent : MonoBehaviour
{
    public WeaponMagazinePreset Preset;
   
    public void Constructor()
    {
        BulletID = Preset.BulletID;
        Capacity = Preset.Capacity;
        Count = Capacity;

        ReloadFactor = Preset.ReloadFactor;

        DamageBehaviours = new DamageBehaviour[] {
            new FireBehaviour(_EmmitTime: 3, _Duration: 1.5f,
            _EffectEmmitTime: 3, _EffectDurTime: 1,
            _Radius: 3, _Damage: 10, _IsParent: true)
        };
    }

    // Идентификатор пули
    public string BulletID { get; private set; }
    // Модификатор изменения времени перезарядки
    public float ReloadFactor { get; private set; }

    // Общая вместимость магазина
    public int Capacity { get; private set; }
    // Текущее количество боеприпасов в магазине
    public int Count { get; set; }

    public DamageBehaviour[] DamageBehaviours;

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

    public void ResetCount()
    {
        Count = Capacity;
    }
}
