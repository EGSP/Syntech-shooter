using System;
using System.Collections.Generic;
using UnityEngine;


public class APointView: MonoBehaviour
{
    public APoint point;
    public APoint jastarPoint;
    public LinkedAPoint linkedPoint;

    public SpriteRenderer sprite;

    public void Initialize()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Log()
    {
        Debug.Log(point.pos.ToString());
    }
}
