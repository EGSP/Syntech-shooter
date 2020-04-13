using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DetailData : IInventoryItem, IDisposable
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

    /// <summary>
    /// Вызывается при изменении количества деталей
    /// </summary>
    public event Action<int> OnCountChanged = delegate { };

    public event Action OnForceUnsubscribe = delegate { };

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

        OnCountChanged(Count);
    }

    /// <summary>
    /// Уменьшение количества деталей
    /// </summary>
    /// <param name="count">Количество, на которое нужно уменьшить</param>
    public void ReduceDetail(int count)
    {
        Count -= count;

        if (Count < 0)
            Count = 0;

        OnCountChanged(Count);
    }

    /// <summary>
    /// Прибавление деталей к этому объекту
    /// </summary>
    /// <param name="detail">Прибавляемая деталь</param>
    public void Merge(DetailData detail)
    {
        AddDetail(detail.Count);
    }

    public void Dispose()
    {
        OnForceUnsubscribe();
    }
}
