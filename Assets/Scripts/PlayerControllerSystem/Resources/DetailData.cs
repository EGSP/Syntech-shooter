using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DetailData : IInventoryItem
{
    public DetailData(int count)
    {
        Count = count;
    }

    public InventoryItemType ItemType { get; set; }

    /// <summary>
    /// Количество деталей
    /// </summary>
    public int Count { get; private set; }



    public bool ItemSendMessage(string message)
    {
        return false;
    }
    
    /// <summary>
    /// Добавление деталей
    /// </summary>
    /// <param name="count">Количество добавляемых деталей</param>
    public void AddDetail(int count)
    {
        Count += count;
    }

    public void Merge(DetailData detail)
    {
        Count += detail.Count;
    }
}
