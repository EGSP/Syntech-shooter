using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using System;

/// <summary>
/// Класс реализующий события EventTrigger
/// </summary>
public class EventElement : EventTrigger
{
    /// <summary>
    /// Вызывается при клике на кнопку
    /// </summary>
    public event Action<PointerEventData> OnPointerClickEvent = delegate { };

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        OnPointerClickEvent(eventData);
    }
}
