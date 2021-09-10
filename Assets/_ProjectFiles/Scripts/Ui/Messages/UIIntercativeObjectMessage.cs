using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using System;

public class UIIntercativeObjectMessage : MonoBehaviour, IPlayerObserver
{
    /// <summary>
    /// Текст сообщения
    /// </summary>
    [SerializeField] private TMP_Text MessageText;

   
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

    private void ShowMessage(string message)
    {
        MessageText.text = message;
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        var observable = playerControllerComponent as IObservable;
        observable.OnForceUnsubcribe += Unsubscribe;

        playerControllerComponent.WeaponHolder.OnMessageSend += ShowMessage;

        ShowMessage("");
    }

    public void PlayerControllerNull()
    {
        throw new System.NotImplementedException();
    }

    public void Unsubscribe(IObservable observable)
    {
        var pc = observable as PlayerControllerComponent;

        pc.WeaponHolder.OnMessageSend -= ShowMessage;

    }


}

public interface IMessageSender
{
    /// <summary>
    /// Вызывается при отправке объектом сообщения
    /// </summary>
    event Action<string> OnMessageSend;
    
}
