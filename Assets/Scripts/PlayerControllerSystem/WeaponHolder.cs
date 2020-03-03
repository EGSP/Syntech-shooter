using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryComponent))]
public class WeaponHolder : MonoBehaviour, IObservable
{
    /// <summary>
    /// Максимальное количество слотов под оружие
    /// </summary>
    [SerializeField] private int MaxWeapons = 3;
    
    /// <summary>
    /// Список оружия
    /// </summary>
    private List<WeaponComponent> Weapons;

    /// <summary>
    /// Текущий индекс оружия в списке
    /// </summary>
    private int currentWeaponIndex;

    /// <summary>
    /// Оружие, которое нельзя выбрасывать или заменять
    /// </summary>
    private int lockedWeaponIndex;

    /// <summary>
    /// Инвентарь игрока
    /// </summary>
    private InventoryComponent playerInventory;

    /// <summary>
    /// Возвращает добавленное оружие
    /// </summary>
    public event Action<WeaponComponent> OnWeaponAdded = delegate { };

    /// <summary>
    /// Возвращает новое оружие
    /// </summary>
    public event Action<WeaponComponent> OnWeaponChanged = delegate { };

    public event Action<IObservable> OnForceUnsubcribe = delegate { };

    void Awake()
    {
        Weapons = new List<WeaponComponent>(MaxWeapons);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GetComponent<InventoryComponent>();
        if (playerInventory == null)
            throw new System.Exception("Инвентарь игрока не найден в WeaponHolder");
        
        

        lockedWeaponIndex = 0;
        
    }
    

    /// <summary>
    /// Добавление нового оружия
    /// </summary>
    /// <param name="newWeapon">Новое оружие</param>
    /// <param name="swaped">Сменило ли новое оружие старое</param>
    /// <returns></returns>
    public WeaponComponent AddWeapon(WeaponComponent newWeapon, out bool swaped)
    {
        swaped = false;
        // Подбор первого оружия
        if(Weapons.Count == 0)
        {
            swaped = true;


            lockedWeaponIndex = 0;
            Weapons.Add(newWeapon);

            OnWeaponAdded(newWeapon);
            OnWeaponChanged(newWeapon);

            return newWeapon;
        }

        // Если нет места
        if (Weapons.Count == MaxWeapons)
        {
            // Если текущее оружие неизменяемо
            if (currentWeaponIndex == lockedWeaponIndex)
                return Weapons[currentWeaponIndex];

            // Ссылка на старое оружие хранится в контроллере игрока
            // Замена оружия
            swaped = true;


            Weapons[currentWeaponIndex] = newWeapon;
            OnWeaponChanged(newWeapon);

            return newWeapon;
        }

        Weapons.Add(newWeapon);
        OnWeaponAdded(newWeapon);

        return Weapons[currentWeaponIndex];
    }

    /// <summary>
    /// Получение ссылки на оружие. Возвращает null, если нет оружия
    /// </summary>
    /// <param name="index">Индекс в списке оружия</param>
    /// <param name="changed">Сменилось ли оружие на новое</param>
    /// <returns></returns>
    public WeaponComponent ChangeWeapon(int index, out bool changed)
    {
        changed = false;

        if(Weapons.Count == 0)
        {
            // Нет оружия
            return null;
        }

        if(index > Weapons.Count-1)
        {
            // Если слоты пустые
            return null;
        }

        if(currentWeaponIndex == index)
        {
            // Не сменилось
            return Weapons[currentWeaponIndex];
        }
        else
        {
            // Сменилось
            currentWeaponIndex = index;
            changed = true;

            OnWeaponChanged(Weapons[currentWeaponIndex]);


            return Weapons[index];
        }
    }

    public WeaponComponent GetCurrentWeapon()
    {
        if (Weapons.Count == 0)
            return null;

        return Weapons[currentWeaponIndex];
    }
}
