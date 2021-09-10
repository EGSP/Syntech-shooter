using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InventorySystem: IObservable
{
    public InventorySystem()
    {
        Inventory = new Dictionary<InventoryItemType, List<IInventoryItem>>();

        Inventory.Add(InventoryItemType.Detail, new List<IInventoryItem>());
        Inventory.Add(InventoryItemType.SpareParts, new List<IInventoryItem>());
        Inventory.Add(InventoryItemType.Ammo, new List<IInventoryItem>());
        Inventory.Add(InventoryItemType.WeaponModules, new List<IInventoryItem>());
    }

    /// <summary>
    /// Максимальное количесвто предметов в списке
    /// </summary>
    private int[] MaxSlots = { 1, 1, 16, 16 }; 

    /// <summary>
    /// Возвращает добавленный боеприпас
    /// </summary>
    public event Action<IInventoryItem> OnAmmoAdded = delegate { };

    /// <summary>
    /// Возвращает добавленный боеприпас
    /// </summary>
    public event Action<IInventoryItem> OnDetailAdded = delegate { };

    /// <summary>
    /// Возвращает добавленный боеприпас
    /// </summary>
    public event Action<IInventoryItem> OnSparePartsAdded = delegate { };

    /// <summary>
    /// Возвращает добавленный боеприпас
    /// </summary>
    public event Action<IInventoryItem> OnWeaponModulesAdded = delegate { };

    public event Action<IObservable> OnForceUnsubcribe = delegate { };

    // Слоты инвентаря
    public Dictionary<InventoryItemType, List<IInventoryItem>> Inventory { get; private set; }
    
    /// <summary>
    /// Получения списка объектов с заданным типом (InventoryItemType)
    /// </summary>
    /// <param name="itemType">Тип объектов в получаемом списке</param>
    public List<IInventoryItem> GetListOfInventoryItem(InventoryItemType itemType)
    {
        return Inventory[itemType];
    }

    /// <summary>
    /// Добавление предмета в инвентарь. Все, что не вместилось будет уничтожено
    /// </summary>
    /// <param name="itemType">Тип предмета</param>
    /// <param name="item">Предмет</param>
    public void AddItem(InventoryItemType itemType, IInventoryItem item)
    {
        switch (itemType)
        {
            case InventoryItemType.Detail:
                if (Inventory[InventoryItemType.Detail].Count > MaxSlots[0])
                    break;

                Inventory[InventoryItemType.Detail].Add(item);
                OnDetailAdded(item);
                break;

            case InventoryItemType.SpareParts:
                if (Inventory[InventoryItemType.SpareParts].Count > MaxSlots[1])
                    break;

                Inventory[InventoryItemType.SpareParts].Add(item);
                OnSparePartsAdded(item);
                break;

            case InventoryItemType.Ammo:
                if (Inventory[InventoryItemType.Ammo].Count > MaxSlots[2])
                    break;

                Inventory[InventoryItemType.Ammo].Add(item);
                OnAmmoAdded(item);
                break;

            case InventoryItemType.WeaponModules:
                if (Inventory[InventoryItemType.WeaponModules].Count > MaxSlots[3])
                    break;

                Inventory[InventoryItemType.WeaponModules].Add(item);
                OnWeaponModulesAdded(item);
                break;
        }
    }
}

