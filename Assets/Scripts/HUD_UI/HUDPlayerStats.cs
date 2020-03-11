using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using TMPro;

public class HUDPlayerStats : MonoBehaviour, IPlayerObserver
{
    /// <summary>
    /// Текст здоровья
    /// </summary>
    [SerializeField] private TMP_Text Health;
    /// <summary>
    /// Текст брони
    /// </summary>
    [SerializeField] private TMP_Text Armour;

    // Из Image получаем матриал
    [Space(10)]
    [SerializeField] private Image HealthImage; 
    [SerializeField] private Image ArmourImage;

    private Material healthMaterial;
    private Material armourMaterial;

    private void Awake()
    {
        healthMaterial = HealthImage.material;
        armourMaterial = ArmourImage.material;

        // Присваивание в Awake, т.к. при старте вызывается первое событие
        UIController.Instance.OnPlayerChanged += ChangePlayerController;

        UIController.Instance.OnPlayerNull += PlayerControllerNull;
    }
    


    /// <summary>
    /// Изменение визуальных показателей здоровья
    /// </summary>
    /// <param name="value">Текущее значение</param>
    /// <param name="maxValue">Максимальное значение</param>
    public void SetHealthValue(float value, float maxValue)
    {
        healthMaterial.SetFloat("_HealthOpacity", value / maxValue);

        Health.text = value.ToString(0);
    }

    /// <summary>
    /// Изменение визуальных показателей брони
    /// </summary>
    /// <param name="value">Текущее значение</param>
    /// <param name="maxValue">Максимальное значение</param>
    public void SetArmourValue(float value, float maxValue)
    {
        armourMaterial.SetFloat("_HealthOpacity", value / maxValue);

        Armour.text = value.ToString(0);
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        var observable = playerControllerComponent as IObservable;
        observable.OnForceUnsubcribe += Unsubscribe;

        playerControllerComponent.PlayerLifeComponent.OnHealthChanged += SetHealthValue;
        playerControllerComponent.PlayerLifeComponent.ActiveArmour.OnArmourChanged += SetArmourValue;

        SetHealthValue(playerControllerComponent.PlayerLifeComponent.Health, playerControllerComponent.PlayerLifeComponent.MaxHealth);
        SetArmourValue(playerControllerComponent.PlayerLifeComponent.ActiveArmour.Battery, playerControllerComponent.PlayerLifeComponent.ActiveArmour.MaxBattery);

    }

    public void PlayerControllerNull()
    {
        throw new System.NotImplementedException();
    }

    public void Unsubscribe(IObservable observable)
    {
        var pc = observable as PlayerControllerComponent;

        pc.PlayerLifeComponent.OnHealthChanged -= SetHealthValue;
        pc.PlayerLifeComponent.ActiveArmour.OnArmourChanged -= SetArmourValue;
    }
}
