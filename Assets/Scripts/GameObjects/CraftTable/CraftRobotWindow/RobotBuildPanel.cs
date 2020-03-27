using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using System;

using TMPro;

public class RobotBuildPanel : MonoBehaviour
{
    [Header("Buttons")]
    // Кнопка постройки робота
    [SerializeField] private EventElement BuildButton;

    [Header("UIElements")]
    // Шкала постройки
    [SerializeField] private Image BuildBar;
    // Количество необходимых и текущих деталей
    [SerializeField] private TMP_Text DetailsText;

    public event Action OnBuildButtonClicked = delegate { };

    private void Awake()
    {
        BuildButton.OnPointerClickEvent += BuildButtonClicked;
    }

    /// <summary>
    /// Устанавливает значения для отображения
    /// </summary>
    /// <param name="current">Текущее количество деталей</param>
    /// <param name="details">Необходимое количество деталей</param>
    public void SetDetails(int current, int details)
    {
        DetailsText.text = $"{current}/{details}";
        
        var color = BuildBar.color;
        color.a = (float)current / (float)details;
        BuildBar.color = color;
    }

    private void BuildButtonClicked(PointerEventData eventData)
    {
        OnBuildButtonClicked();
    }
}
