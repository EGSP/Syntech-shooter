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

            RobotIcons.Add(builder.Data[i].RobotPrefab.ID, robotIcon);
        }
    }

    /// <summary>
    /// Выделить робота в селекторе
    /// </summary>
    /// <param name="id">Идентификатор робота</param>
    public void SelectRobot(string id)
    {
        if (RobotIcons.ContainsKey(id))
        {
            var icon = RobotIcons[id];

            icon.OnSelect();
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
