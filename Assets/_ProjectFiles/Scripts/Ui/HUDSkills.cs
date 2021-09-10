using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HUDSkills : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private Image DashSkillIcon;

    void Awake()
    {
        // Присваивание в Awake, т.к. при старте вызывается первое событие
        UIController.Instance.OnPlayerChanged += ChangePlayerController;

        UIController.Instance.OnPlayerNull += PlayerControllerNull;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Изменение значения заливки в зависимости от перезарядки
    /// </summary>
    private void OnReloadValueChanged(float value, float maxValue)
    {
        var tempColor = DashSkillIcon.color;
        tempColor.a = 1 - value / maxValue; 
        DashSkillIcon.color = tempColor;
    }

    
    private void OnSkillStateChanged(DashSkill.SkillState value)
    {
        if(value == DashSkill.SkillState.Active)
        {
            var tempColor = DashSkillIcon.color;
            tempColor.a = 0;
            DashSkillIcon.color = tempColor;
            return;
        }

        if(value == DashSkill.SkillState.Done)
        {
            var tempColor = DashSkillIcon.color;
            tempColor.a = 1;
            DashSkillIcon.color = tempColor;
            return;
        }
        
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        var observable = playerControllerComponent as IObservable;
        observable.OnForceUnsubcribe += Unsubscribe;

        playerControllerComponent.DashSkill.OnReloadValueChanged += OnReloadValueChanged;
        playerControllerComponent.DashSkill.OnSkillStateChanged += OnSkillStateChanged;

        OnReloadValueChanged(playerControllerComponent.DashSkill.CurrentReloadTime, playerControllerComponent.DashSkill.ReloadTime);
        OnSkillStateChanged(playerControllerComponent.DashSkill.skillState);
    }

    public void PlayerControllerNull()
    {
        throw new System.NotImplementedException();
    }

    public void Unsubscribe(IObservable observable)
    {
        var pc = observable as PlayerControllerComponent;

        pc.DashSkill.OnReloadValueChanged -= OnReloadValueChanged;
        pc.DashSkill.OnSkillStateChanged -= OnSkillStateChanged;
    }
}
