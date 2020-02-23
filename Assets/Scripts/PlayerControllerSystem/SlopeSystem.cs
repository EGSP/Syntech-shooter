#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlopeSystem
{

    [SerializeField] private Transform Ass;  // Таз
    [SerializeField] private Transform Head; // Голова - камера
    [Space]
    [SerializeField] private float AssAngle;  // Максимальный угол поворота таза
    [SerializeField] private float HeadAngle; // Максимальный угол поворота головы
    [Space]
    [SerializeField] private float SlopeDelta; // Скорость наклона

    private float Dir
    {
        get { return dir; }
        set
        {
            // Если ничего не введено
            if (value == 0)
                return;

            // Если введено то же значение
            if (value == dir)
            {
                dir = 0;
                return;
            }

            // Если введено зеркальное значение
            if (value != dir)
                dir = value;
        }
    }
    private float dir;
    // Наклоняет в сторону dir == left or right
    public SlopeSystemOutput Update(SlopeSystemInput IN)
    {
        int R = IN.inputRight ? -1 : 0;
        int L = IN.inputLeft ? 1 : 0;

        Dir = R + L;

        // Угол наклона по вертикали
        float headFactor = IN.cameraLocalEulerAnglesX;
        headFactor = Extensions.FixAngle(headFactor);

        float lerp = 1 - Mathf.Abs(headFactor) / 90;

        //Поворот таза
        Ass.transform.localRotation = Quaternion.Slerp(
            Ass.transform.localRotation,
            Quaternion.Euler(new Vector3(Ass.transform.localRotation.x, Ass.transform.localRotation.y, dir * AssAngle * lerp)),
            SlopeDelta * Time.deltaTime
            );

        //Поворот головы в противположную сторону
        Head.transform.localRotation = Quaternion.Slerp(
            Head.transform.localRotation,
            Quaternion.Euler(new Vector3(Head.transform.localEulerAngles.x, Head.transform.localEulerAngles.y, -1 * dir * HeadAngle * lerp)),
            SlopeDelta * Time.deltaTime
            );

        return new SlopeSystemOutput()
        {

        };
    }

    
}

public class SlopeSystemInput
{
    public bool inputLeft;
    public bool inputRight;
    public float cameraLocalEulerAnglesX;
}

public class SlopeSystemOutput
{

}
