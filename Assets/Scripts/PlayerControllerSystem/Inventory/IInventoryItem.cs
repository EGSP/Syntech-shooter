using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IInventoryItem
{
    InventoryItemType ItemType { get; }

    bool ItemSendMessage(string message);
}

public enum InventoryItemType
{
    Detail,
    SpareParts,
    Ammo,
    WeaponModules
}