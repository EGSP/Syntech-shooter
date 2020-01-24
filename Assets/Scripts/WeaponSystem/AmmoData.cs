using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class AmmoData: IInventoryItem
{
    public AmmoData(string _BulletID, int _Count)
    {
        ItemType = InventoryItemType.Ammo;

        BulletID = _BulletID;
        Count = _Count;
    }

    public InventoryItemType ItemType { get; private set; }

    // Идентификатор боеприпаса
    public readonly string BulletID;
    // Количество хранимых боеприпасов
    public int Count { get; private set; }

    // Хранятся ли боеприпасы
    public bool IsEmpty
    {
        get => (Count > 0) ? false : true;
    }

    public int Take(int takenAmmo)
    {
        Count -= takenAmmo;
        return takenAmmo;
    }

    /// <summary>
    /// Слияние двух AmmoData
    /// </summary>
    /// <param name="mergeAmmo">AmmoData из которого будут взяты все боеприпасы</param>
    public AmmoData Merge(AmmoData mergeAmmo)
    {
        Count+=mergeAmmo.Take(mergeAmmo.Count);
        return this;
    }

    /// <summary>
    /// Проверка идентификатора пули (IInventoryItem)
    /// </summary>
    public bool ItemSendMessage(string message)
    {
        if (BulletID == message)
            return true;

        return false;
    }
}
