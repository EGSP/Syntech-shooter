using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UI_NotifyPanel : MonoBehaviour
{
    [SerializeField] private Image Background; 
    [SerializeField] private TMP_Text Text;

    /// <summary>
    /// Текущее время жизни
    /// </summary>
    public float fadeDelay { get; set; }
    
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Место в иерархии
    /// </summary>
    public int HierarchyIndex
    {
        set => transform.SetSiblingIndex(value);
    }

    /// <summary>
    /// Сообщение уведомления
    /// </summary>
    public string Message
    {
        set => Text.text = value;
    }
    
    /// <summary>
    /// Прозрачность уведомления от 0 до 1
    /// </summary>
    public float Opacity
    {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }

    /// <summary>
    /// Цвет заднего изображения
    /// </summary>
    public Color BackgroundColor
    {
        set => Background.color = value;
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
   
}
