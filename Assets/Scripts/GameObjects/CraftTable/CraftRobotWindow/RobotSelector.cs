using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System;

using AIB.AIBehaviours;

using TMPro;

public class RobotSelector : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private CraftRobotWindow CraftRobotWindow;
    
    [Header("UIElements")]
    [SerializeField] private RectTransform RobotIconsParent;
    // Стандартный цвет
    [SerializeField] private Color DefaultColor;
    // Цвет при выделении и отсутствии робота у игрока
    [SerializeField] private Color BuildColor;
    // Цвет при выделении и наличии робота у игрока
    [SerializeField] private Color OwnColor;
    

    [Header("Prefabs")]
    [SerializeField] private RobotSelectorIcon selectorIconPrefab;

    /// <summary>
    /// Словарь созданных иконок
    /// </summary>
    private Dictionary<string,RobotSelectorIcon> RobotIcons;
    
    /// <summary>
    /// Вызывается при выборе робота из списка. Передает ID
    /// </summary>
    public event Action<string> OnRobotChoosed = delegate { };

    // Последняя выбранная иконка
    private RobotSelectorIcon lastIcon;

    private void Awake()
    {
        var builder = CraftRobotWindow.RobotBuilder;

        RobotIcons = new Dictionary<string, RobotSelectorIcon>();
        
        // Генерируем иконки для каждого робота
        for(int i=0;i< builder.Data.Count; i++)
        {
            var robotIcon = Instantiate(selectorIconPrefab);
            robotIcon.transform.SetParent(RobotIconsParent,false);
            robotIcon.gameObject.SetActive(true);

            robotIcon.OnClickedEvent += ChooseRobot;

            robotIcon.RobotID = builder.Data[i].RobotPrefab.ID;
            robotIcon.Icon.sprite = builder.Data[i].CompanionBundle.companionIcon;
            robotIcon.ShadowColor = DefaultColor;

            RobotIcons.Add(builder.Data[i].RobotPrefab.ID, robotIcon);
        }
    }

    /// <summary>
    /// Выделить робота в селекторе
    /// </summary>
    /// <param name="id">Идентификатор робота</param>
    public void SelectRobot(string id, bool toBuild)
    {
        if (RobotIcons.ContainsKey(id))
        {
            if (lastIcon != null)
                lastIcon.ShadowColor = DefaultColor;

            var icon = RobotIcons[id];

            icon.OnSelect();
            icon.ShadowColor = toBuild ? BuildColor : OwnColor;

            lastIcon = icon;
        }
    }

    /// <summary>
    /// Выбрать робота и передать всему окну
    /// </summary>
    /// <param name="id">Идентификатор робота</param>
    public void ChooseRobot(string id)
    {
        OnRobotChoosed(id);
    }
    
}
