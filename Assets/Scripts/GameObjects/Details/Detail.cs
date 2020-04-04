using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detail : LootPhysicBody
{
    /// <summary>
    /// Количество получаемых деталей
    /// </summary>
    public int Count { get => count; protected set => count = value; }
    [SerializeField] private int count;

    // Был ли использован компонент
    private bool Used;

    /// <summary>
    /// Получение экземпляра детали
    /// </summary>
    public DetailData GetDetailData()
    {
        // Возвращение пустой детали
        if (Used == true)
            return new DetailData(0);

        return new DetailData(Count);
    }

    protected override void Update()
    {
        base.Update();

        if (Used)
        {
            InsertToPool();
        }
    }
}
