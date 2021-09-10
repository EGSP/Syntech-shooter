using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class OpacityBarDrawer : MonoBehaviour
{
    /// <summary>
    /// Графический элемент, который будет изменяться
    /// </summary>
    public GameObject Bar;

    /// <summary>
    /// Материал содержащий параметр _HealthOpacity
    /// </summary>
    private Material BarMaterial;

    public void Awake()
    {
        
            
    }

    public void ChangeOpacity(float count, float maxCount)
    {
        var opacity = count / maxCount;

        BarMaterial.SetFloat("_HealthOpacity", opacity);
    }
}

///// <summary>
///// Интерфейс который позволяет другим компонентам отрисовывать данные
///// </summary>
//public interface INeedBarDraw
//{
//    /// <summary>
//    /// Подписка наблюдателя к событию. Наблюдатель ожидает, что подписка точно будет выполнена
//    /// </summary>
//    /// <param name="viewerMethod">Метод наблюдателя, который нужно подписать на событие</param>
//    void SubscribeViewer(Action<float, float> viewerMethod);

//    /// <summary>
//    /// Отписка наблюдателя от события. Наблюдатель обязан быть отписанным
//    /// </summary>
//    /// <param name="viewerMethod">Метод наблюдателя, который нужно отписать</param>
//    void UnsubscribeViewer(Action<float, float> viewerMethod);
//}
