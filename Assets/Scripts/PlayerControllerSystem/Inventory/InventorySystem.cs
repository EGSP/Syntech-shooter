using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InventorySystem
{
    public InventorySystem()
    {
        Inventory = new Dictionary<InventoryItemType, List<IInventoryItem>>();

        Inventory.Add(InventoryItemType.Detail, new List<IInventoryItem>());
        Inventory.Add(InventoryItemType.SpareParts, new List<IInventoryItem>());
        Inventory.Add(InventoryItemType.Ammo, new List<IInventoryItem>());
        Inventory.Add(InventoryItemType.WeaponModules, new List<IInventoryItem>());
    }

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
}

