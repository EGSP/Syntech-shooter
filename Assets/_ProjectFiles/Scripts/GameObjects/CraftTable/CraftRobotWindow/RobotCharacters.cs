using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AIB.AIBehaviours;

public class RobotCharacters : MonoBehaviour
{
    /// <summary>
    /// Родительский элемент в котором находятся все настройки для отрисовки
    /// </summary>
    private RobotOverview RobotOverview;
    
    /// <summary>
    /// Робот, характеристики которого нужно отрисовать
    /// </summary>
    public SignalAIBehaviour Robot
    {
        protected get => robot;
        set
        {
            robot = value;

            ShowCharacters();
        }
    }
    private SignalAIBehaviour robot;
    
    
    public virtual void ShowCharacters()
    {
        RobotOverview.AddCharacter("ARMOUR", Robot.LifeComponent.ActiveArmour.MaxBattery, 0.5f);
        RobotOverview.AddCharacter("HEALTH", Robot.LifeComponent.MaxHealth, 1);

    }


    /// <summary>
    /// Устанавливет родителський элемент интерфейса
    /// </summary>
    /// <param name="robotOverview">Не может быть null</param>
    public void SetOverview(RobotOverview robotOverview)
    {
        RobotOverview = robotOverview;
    }
}
