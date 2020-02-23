using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class UIAmmoElement : MonoBehaviour
{
    [SerializeField] private TMP_Text Count;

    [SerializeField] private Image Icon;

    private void Start()
    {
        ResetToDefault();
    }

    /// <summary>
    /// Установка изображения боеприпаса. Не должно быть null
    /// </summary>
    /// <param name="icon">Иконка боеприпаса</param>
    public void SetImage(Sprite icon)
    {
        Icon.sprite = icon;
    }

    /// <summary>
    /// Установка стандартного изображения
    /// </summary>
    public void SetDefaultImage()
    {
        Icon.sprite = null;
    }

    public void SetData(AmmoData ammoData)
    {
        Count.text = ammoData.Count.ToString();
    }

    public void ResetToDefault()
    {
        SetDefaultImage();
        Count.text = "";
    }

}
