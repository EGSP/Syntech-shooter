using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DamageEffects;
public class WeaponMagazineComponent : MonoBehaviour, IObservable
{
    public WeaponMagazinePreset Preset;
   
    public void Constructor()
    {
        BulletID = Preset.BulletID;
        Capacity = Preset.Capacity;
        Count = Capacity;

        ReloadFactor = Preset.ReloadFactor;

        DamageBehaviours = new DamageBehaviour[] {
            //new FireBehaviour(_EmmitTime: 3, _Duration: 1.5f,
            //_EffectEmmitTime: 3, _EffectDurTime: 1,
            //_Radius: 3, _Damage: 10, _IsParent: true)

            //new PlasmaBehaviour(_EffectEmmitTime: 3,_EffectDurTime: 0.15f,_Damage: 15)
            //new AcidBehaviour(_Duration:0.2f,_Damage:3,_SpeedModifier:0.3f,_ModifierTime:1)
            //new ShockBehaviour(3)
            //new GrowingBehaviour(5,20)
            //new HEIBehaviour(5,3)
            new CumulativeBehaviour(5,3)
        };
    }

    // Идентификатор пули
    public string BulletID { get; private set; }
    // Модификатор изменения времени перезарядки
    public float ReloadFactor { get; private set; }

    // Общая вместимость магазина
    public int Capacity { get; private set; }
    // Текущее количество боеприпасов в магазине
    public int Count
    {
        get => count;
        set
        {
            count = value;
            OnCountChanged(count);
        }
    }
    private int count;

    public event Action<int> OnCountChanged = delegate { };

    public event Action<IObservable> OnForceUnsubcribe = delegate { };

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

    /// <summary>
    /// Отсоединение компонента от магазина
    /// </summary>
    public void Unweld()
    {
        OnForceUnsubcribe(this);
    }
}
