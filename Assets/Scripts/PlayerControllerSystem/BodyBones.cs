using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BodyBones
{
    // Дистанция от центра до земли
    [Tooltip("Дистанция от центра тела до земли")]
    public float CentreToGround;

    public Transform Ass;
    public Transform Head;

    // Высота при состояниях
    public float AssWalk;
    public float AssCrouch;

    /// <summary>
    /// Позиция текущего положения тела
    /// </summary>
    [NonSerialized] public float AssPosition;
    
    /// <summary>
    /// Проверка на наличие всех компонентов
    /// </summary>
    public bool CheckNull()
    {
        if (Ass == null || Head == null)
            return false;

        return true;
    }
}

