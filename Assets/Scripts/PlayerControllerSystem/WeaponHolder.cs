using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryComponent))]
public class WeaponHolder : MonoBehaviour
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
    /// Интерфейс отрисовки оружия 
    /// </summary>
    [SerializeField] private HUDWeapon HUDWeapon;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GetComponent<InventoryComponent>();
        if (playerInventory == null)
            throw new System.Exception("Инвентарь игрока не найден в WeaponHolder");

        HUDWeapon.SetInventorySystem(playerInventory.InventorySystem);
        Weapons = new List<WeaponComponent>(MaxWeapons);

        lockedWeaponIndex = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Weapons.Count != 0)
            HUDWeapon.UpdateUI();
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

            HUDWeapon.SetWeapon(newWeapon);

            lockedWeaponIndex = 0;
            Weapons.Add(newWeapon);
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

            HUDWeapon.SetWeapon(newWeapon);

            Weapons[currentWeaponIndex] = newWeapon;
            return newWeapon;
        }

        Weapons.Add(newWeapon);
        return Weapons[currentWeaponIndex];
    }

    /// <summary>
    /// Получение ссылки на оружие
    /// </summary>
    /// <param name="index">Индекс в списке оружия</param>
    /// <param name="changed">Сменилось ли оружие на новое</param>
    /// <returns></returns>
    public WeaponComponent TakeWeapon(int index, out bool changed)
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

            HUDWeapon.SetWeapon(Weapons[index]);

            return Weapons[index];
        }
    }
}
