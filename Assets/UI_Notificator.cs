using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MUI;

public class UI_Notificator : MonoBehaviour
{
    [SerializeField] private int MaxNots;
    [SerializeField] private float FadeSpeed;
    [SerializeField] private float FadeDelay;
    [Space(10)]
    [SerializeField] private UI_NotifyPanel NotPrefab;
    [SerializeField] private Transform NotList;
    [SerializeField] private Color[] MessageColors;

    private Queue<UI_NotifyPanel> InactiveNots;
    private Queue<UI_NotifyPanel> ActiveNots;

    private void Awake()
    {
        InactiveNots = new Queue<UI_NotifyPanel>();
        ActiveNots = new Queue<UI_NotifyPanel>();

        for(int i = 0; i < MaxNots; i++)
        {
            var prefab = Instantiate(NotPrefab);
            prefab.transform.SetParent(NotList, false);
            prefab.gameObject.SetActive(false);
            prefab.Opacity = 0;
            
            InactiveNots.Enqueue(prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;

        int dequeue = 0;
        foreach (var n in ActiveNots)
        {
            if (n.fadeDelay < 0)
            {
                // Увеличиваем прозрачность
                n.Opacity -= deltaTime * FadeSpeed;

                // Убираем в неактивные элементы
                if (n.Opacity <= 0)
                {
                    dequeue++;
                }
            }
            else
            {
                n.fadeDelay -= deltaTime;
            }
        }

        // Очистка отработавших уведомлений
        for(int i = 0; i < dequeue; i++)
        {
            var not = ActiveNots.Dequeue();

            not.gameObject.SetActive(false);

            InactiveNots.Enqueue(not);
        }
    }

    /// <summary>
    /// Показывает уведомление с текстом сообщения
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    public void ShowNotify(string message, MessageType messageType = MessageType.Information)
    {
        UI_NotifyPanel not;
        if (InactiveNots.Count > 0)
        {
            not = InactiveNots.Dequeue();
        }
        else
        {
            not = ActiveNots.Dequeue();
        }

        if (not != null)
        {
            not.gameObject.SetActive(true);
            not.transform.SetSiblingIndex(0);
            not.Opacity = 1;
            not.fadeDelay = FadeDelay;
            not.BackgroundColor = MessageColors[(int)messageType];
            not.Message = message;

            ActiveNots.Enqueue(not);
        }
    }
}

namespace MUI
{
    public enum MessageType
    {
        OK = 0,
        Error = 1,
        Warning = 2,
        Information = 3
    }
}








