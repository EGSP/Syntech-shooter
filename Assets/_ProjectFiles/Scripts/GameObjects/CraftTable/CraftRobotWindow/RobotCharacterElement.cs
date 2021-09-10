using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using TMPro;

public class RobotCharacterElement : MonoBehaviour
{
    [Header("Bar")]
    [SerializeField] Image Bar;
    [Header("Texts")]
    [SerializeField] TMP_Text CharacterValueText;
    [SerializeField] TMP_Text CharacterNameText;

    /// <summary>
    /// Значение заполнения. От 0 до 1
    /// </summary>
    public float Opacity
    {
        set
        {
            var color = Bar.color;
            color.a = value;
            Bar.color = color;
        }
    }

    /// <summary>
    /// Название характеристики
    /// </summary>
    public string Name
    {
        set
        {
            CharacterNameText.text = value;
        }
    }

    /// <summary>
    /// Значение характеристики
    /// </summary>
    public float Value
    {
        set
        {
            CharacterValueText.text = value.ToString(0);
        }
    }

}
