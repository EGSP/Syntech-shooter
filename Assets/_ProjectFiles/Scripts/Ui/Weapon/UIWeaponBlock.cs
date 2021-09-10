using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class UIWeaponBlock : EventTrigger
{
    /// <summary>
    /// Стандартный цвет 
    /// </summary>
    public Color DefaultColor;
    
    /// <summary>
    /// Цвет выделения
    /// </summary>
    public Color SelectColor;

    /// <summary>
    /// Название оружия
    /// </summary>
    public TMP_Text WeaponName;

    /// <summary>
    /// Номер блока оружия соответсвует очереди добавления
    /// </summary>
    public TMP_Text BlockNumber;

    /// <summary>
    /// Иконка оружия
    /// </summary>
    public Image WeaponIcon;

    /// <summary>
    /// Выделение блока
    /// </summary>
    public void Select()
    {
        BlockNumber.color = SelectColor;
    }

    /// <summary>
    /// Снятие выделения блока
    /// </summary>
    public void Deselect()
    {
        BlockNumber.color = DefaultColor;
    }
}
