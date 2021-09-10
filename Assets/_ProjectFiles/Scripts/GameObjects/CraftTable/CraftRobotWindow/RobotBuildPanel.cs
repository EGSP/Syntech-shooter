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
    [SerializeField] private Color BarDefaultColor;
    [SerializeField] private Color BarFilledColor;
    // Количество необходимых и текущих деталей
    [SerializeField] private TMP_Text DetailsText;

    /// <summary>
    /// Вызывается при клике на кнопку
    /// </summary>
    public event Action OnBuildButtonClicked = delegate { };

    /// <summary>
    /// Вызывается при изменении отображения деталей. Возвращает opacity
    /// </summary>
    public event Action<RobotBuilderData> OnDetailsChange = delegate { };

    private void Awake()
    {
        BuildButton.OnPointerClickEvent += BuildButtonClicked;
    }

    /// <summary>
    /// Устанавливает значения для отображения
    /// </summary>
    public void SetDetails(RobotBuilderData data)
    {
        OnDetailsChange(data);
        if(data.ReadyToBuild)
        {
            BuildBar.color = BarFilledColor;
        }
        else
        {
            BuildBar.color = BarDefaultColor;
        }

        DetailsText.text = $"{data.Current}/{data.CompanionBundle.details}";
        
        var color = BuildBar.color;
        color.a = data.BuildOpacity;
        BuildBar.color = color;
    }

    private void BuildButtonClicked(PointerEventData eventData)
    {
        OnBuildButtonClicked();
    }
}
