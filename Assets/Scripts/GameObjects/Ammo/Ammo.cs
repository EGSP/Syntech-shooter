using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    // Засунуть это в пресет
    // Идентификатор боеприпаса
    public string BulletID;

    // Количество боеприпасов
    public int Count;

    // Был ли использован компонент
    private bool Used;
    
    /// <summary>
    /// Использование компонента боеприпасов
    /// </summary>
    /// <returns></returns>
    public AmmoData GetAmmoData()
    {
        // Возвращение пустого боезапаса
        if (Used == true)
            return new AmmoData(BulletID, 0);

        Used = true;

        return new AmmoData(BulletID, Count);
    }
    
    private void Update()
    {
        if (Used)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

}
