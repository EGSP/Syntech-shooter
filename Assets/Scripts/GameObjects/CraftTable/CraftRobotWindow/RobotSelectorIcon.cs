using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;

public class RobotSelectorIcon : MonoBehaviour
{
    /// <summary>
    /// Идентификатор робота
    /// </summary>
    public string RobotID { get; set; }

    /// <summary>
    /// Иконка отображения робота
    /// </summary>
    public Image Icon { get => icon; private set => icon = value; }
    [SerializeField] private Image icon;

    [SerializeField] private Image shadowImage;

    /// <summary>
    /// Цвет тени (подсветки)
    /// </summary>
    public Color ShadowColor
    {
        set => shadowImage.color = value;
    }
    
    /// <summary>
    /// Вызывается при клике на иконку
    /// </summary>
    public event Action<string> OnClickedEvent = delegate { };

    private EventElement eventElement;

    private void Awake()
    {
        eventElement = GetComponent<EventElement>();
        eventElement.OnPointerClickEvent += OnClicked;
    }

    private void OnClicked(PointerEventData eventData)
    {
        OnClickedEvent(RobotID);

        // Здесь анимашки
    }

    public void OnSelect()
    {
        // Здесь анимашки выбора
    }

    
}
