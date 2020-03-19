using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeComponent))]
public class DamageResisterComponent : MonoBehaviour
{
    /// <summary>
    /// Данные о сопротивляемости урону
    /// </summary>
    [SerializeField] private DamageResister ResisterData;

    private void Awake()
    {
        GetComponent<LifeComponent>().AddDamageTakePerk(ResisterData);
    }
}

[System.Serializable]
public class DamageResister: IDamageTakePerk
{
    
    [SerializeField] private DamageType resistType;
    [Range(0, 1)]
    [SerializeField] private float resistValue;
    [SerializeField] private int Priority;

    /// <summary>
    /// Тип урона на который действует сопротивление
    /// </summary>
    public DamageType ResistType
    {
        get => resistType;
        private set => resistType = value;
    }
    
    /// <summary>
    /// Степень сопротивления урону (0 - нет сопротивления, 1 - полное сопротивление)
    /// </summary>
    public float ResistValue
    {
        get => 1 - resistValue;
        private set => resistValue = value;
    }
    
    /// <summary>
    /// Приоритет резиста
    /// </summary>
    public int DamagePerkPriority
    {
        get => Priority;
        private set => Priority = value;
    }

    

    /// <summary>
    /// Изменяет урон в зависимости от значения сопротивления
    /// </summary>
    /// <param name="data"></param>
    public void PerkDamage(DamageData data)
    {
        data.baseDamage *= ResistValue;
    }
}
