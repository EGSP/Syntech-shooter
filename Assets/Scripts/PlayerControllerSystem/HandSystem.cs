#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandSystem
{
    [Header("Transform")]
    public Transform Hands;
    public Transform WeaponParent;
    [Space]
    [Header("Sensivity and Smoothness")]
    [SerializeField] private float handSensivityFactor;
    [Range(0, 1)]
    [SerializeField] private float HandsLerp;
    [Range(0, 0.1f)]
    [SerializeField] private float HandsCentrallizeLerp; // Скорость доворота рук до нулевого положения
    
    [Tooltip("X - Left, Z - Right, Y - Top, W - Bottom")]
    [SerializeField] private Vector4 ClampHandsRotation;
    [Tooltip("Позиция рук относительно камеры и угла поврота")]
    [SerializeField] private Vector3 HandsOffsetAboutAngle; // Позиция рук относительно камеры
    [Range(0, 1)]
    [SerializeField] private float HandsOffsetLerp; // Плавность смещения рук
    [Space]
    [Range(0, 1)]
    [SerializeField] private float WeaponRecoilResistance; // Коэфицент сопротивления отдаче 
    
    private Quaternion newHandsRotation;
    
    private Vector2 handsRotation; // значение углов поворота рук
    
    private Vector3 startHandsPosition;
    private Vector3 newHandsPosition;

    // Start is called before the first frame update
    public bool Start()
    {
        startHandsPosition = Hands.localPosition;
        newHandsPosition = startHandsPosition;

        return true;
    }

    // Update is called once per frame
    public HandSystemOutput Update(HandSystemInput IN)
    {
        // Угол поворота отдачи со знаком противополжным повороту по Оси N
        var recoilX = IN.weaponRecoil * (1 - WeaponRecoilResistance) * (-1) * Time.deltaTime;
        var recoilY = IN.weaponRecoil * (1 - WeaponRecoilResistance)*(-1) * Time.deltaTime;

        Vector2 AbsRotationXY = new Vector2(Mathf.Abs(IN.rotationX), Mathf.Abs(IN.rotationY));

        // Поворот рук относительно 
        var handsCentralizeFactorXY = Vector2.Max(Vector2.zero, new Vector2(HandsCentrallizeLerp, HandsCentrallizeLerp) - AbsRotationXY); // Коэфицент возврата к рукам
        handsRotation += new Vector2(
            IN.rotationX + recoilX,
            IN.rotationY ) * handSensivityFactor;

        handsRotation.x = Mathf.Lerp(Mathf.Clamp(handsRotation.x, ClampHandsRotation.w, ClampHandsRotation.y), 0, handsCentralizeFactorXY.x); // Ограничение поврота
        handsRotation.y = Mathf.Lerp(Mathf.Clamp(handsRotation.y, ClampHandsRotation.x, ClampHandsRotation.z), 0, handsCentralizeFactorXY.y);

        newHandsRotation = IN.cameraRotation * Quaternion.Euler(handsRotation.x, handsRotation.y, 0); // Новое вращение рук
        Hands.rotation = Quaternion.Lerp(Hands.rotation, newHandsRotation, HandsLerp); // Применяем вращение

        var factorX = Mathf.Sin(handsRotation.y * Mathf.Deg2Rad); // Целенаправленно изменены местами x и y, так как FactorN - это коэфиценты координат
        var factorY = Mathf.Sin(handsRotation.x * Mathf.Deg2Rad);

        // Позиция рук относительно поворота
        var handsTruePos = Hands.localRotation * startHandsPosition; // Позиция относительно поворота
        newHandsPosition = handsTruePos - new Vector3(HandsOffsetAboutAngle.x * factorX, HandsOffsetAboutAngle.y * factorY, HandsOffsetAboutAngle.z * factorY);
        Hands.localPosition = Vector3.Lerp(Hands.localPosition, newHandsPosition, HandsOffsetLerp);

        return new HandSystemOutput();
    }
}


public class HandSystemInput
{
    public float rotationY;
    public float rotationX;

    public Quaternion cameraRotation;

    public float weaponRecoil;
}

public class HandSystemOutput
{
}
