using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using AIB.AIBehaviours;

public class RobotStats : MonoBehaviour
{
    [Header("Bars")]
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image AbilityBar;
    [Header("Texts")]
    [SerializeField] private TMP_Text NameText;

    /// <summary>
    /// Робот используемый для отрисовки
    /// </summary>
    public SignalAIBehaviour Robot
    {
        private get => robot;
        set
        {
            if (robot != null)
                UnsubscribeFromRobot();

            robot = value;

            if(robot == null)
            {
                ShowNotify();
            }
            else
            {
                SubscibeToRobot();
                ShowStats();
            }
        }
    }
    private SignalAIBehaviour robot;
     

    private void OnRobotLifeChanged(float value, float maxValue)
    {
        var color = HealthBar.color;
        color.a = value / maxValue;
        HealthBar.color = color;
    }

    private void OnRobotAblityChanged(float opacity)
    {
        var color = AbilityBar.color;
        color.a = opacity;
        AbilityBar.color = color;
    }

    /// <summary>
    /// Показывает статы робота
    /// </summary>
    private void ShowStats()
    {
        NameText.text = Robot.RobotName;

        OnRobotLifeChanged(Robot.LifeComponent.Health, Robot.LifeComponent.MaxHealth);
        OnRobotAblityChanged(Robot.ChargeOpacity);
    }

    /// <summary>
    /// Показывает уведомление об отсутствии робота
    /// </summary>
    private void ShowNotify()
    {
        NameText.text = "Empty";

        OnRobotAblityChanged(0);
        OnRobotLifeChanged(0, 1);
    }

    private void SubscibeToRobot()
    {
        Robot.LifeComponent.OnHealthChanged += OnRobotLifeChanged;
        Robot.OnChargeCountChanged += OnRobotAblityChanged;
    }

    private void UnsubscribeFromRobot()
    {
        Robot.LifeComponent.OnHealthChanged -= OnRobotLifeChanged;
        Robot.OnChargeCountChanged -= OnRobotAblityChanged;
    }
}
