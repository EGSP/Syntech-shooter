using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

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

        // Установка модификаторов урона
        for(int i = 0; i < Preset.DamageBehaviourPresets.Count; i++)
        {
            AddDamageBehaviour(Preset.DamageBehaviourPresets[i].GetDamageBehaviour());
        }
    }

    // Идентификатор пули
    public string BulletID { get; private set; }
    // Модификатор изменения времени перезарядки
    public float ReloadFactor { get; private set; }

    /// <summary>
    /// Вместимость боеприпасов в магазине
    /// </summary>
    public int Capacity { get; private set; }
    
    /// <summary>
    /// Количество боеприпасов в магазине
    /// </summary>
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

    /// <summary>
    /// Вызывается при изменении количества боеприпасов в магазине
    /// </summary>
    public event Action<int> OnCountChanged = delegate { };

    public event Action<IObservable> OnForceUnsubcribe = delegate { };

    /// <summary>
    /// Список модификаторов урона
    /// </summary>
    public List<DamageBehaviour> DamageBehaviours { get; private set; }

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

    /// <summary>
    /// Добавление модификатора урона
    /// </summary>
    /// <param name="damageBehaviour">Новый модификатор урона</param>
    public void AddDamageBehaviour(DamageBehaviour damageBehaviour)
    {
        var coincidence = DamageBehaviours.FindIndex(x => x.ID == damageBehaviour.ID);

        // Если не было найдено совпадений
        if(coincidence == -1)
        {
            DamageBehaviours.Add(damageBehaviour);
        }
        else
        {
            // Происходит замена на новый модификатор этого же типа
            DamageBehaviours[coincidence] = damageBehaviour;
        }
    }

    /// <summary>
    /// Удаление из списка модификатора урона
    /// </summary>
    /// <param name="behaviourID">Идентификатор модификатора урона</param>
    public void RemoveDamageBehaviour(int behaviourID)
    {
        var coincidence = DamageBehaviours.FindIndex(x => x.ID == behaviourID);

        if (coincidence > -1)
        {
            DamageBehaviours.RemoveAt(coincidence);
        }
    }
}
