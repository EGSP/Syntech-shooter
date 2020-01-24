using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BodyBones
{
    // Дистанция от центра до земли
    [Tooltip("Дистанция от центра тела до земли")]
    public float CentreToGround;

    // Вектор смещения центра тела
    public Vector3 BodyCenterOffset;
}

