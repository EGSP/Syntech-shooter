#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSystem 
{
    [Header("UnityCamera")]
    public Camera UnityCamera;

    [Header("Transform")]
    public Transform Camera;
    
    [Header("Limiters and Offsets")]
    [SerializeField] private float ClampCameraVerticalRotation; // Ограничение поворота камеры по оси X[Space]
    [Range(0, 1)]
    [SerializeField] private float WeaponRecoilResistance; // Коэфицент сопротивления отдаче 

    [Header("Effects")]
    [SerializeField] private MinMax FovRange;

    private Quaternion newCameraRotation;
    private float cameraVerticalRotation; // Значение угла поворота камеры

    public bool Start()
    {
        newCameraRotation = Camera.rotation;
        return true;
    }

    public CameraSystemOutput Update(CameraSystemInput IN)
    {
        // Поворот камеры относительно тела
        cameraVerticalRotation += IN.rotationX+IN.weaponRecoil*(1-WeaponRecoilResistance)*Time.deltaTime*-1;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -ClampCameraVerticalRotation, ClampCameraVerticalRotation); // Ограничение поврота

        newCameraRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0); // Новое вращение камеры
        Camera.localRotation = newCameraRotation; // Применяем вращение

        // Изменение поля зрения
        UnityCamera.fieldOfView = Mathf.Lerp(FovRange.min, FovRange.max, IN.weaponRecoilOpacity);

        return new CameraSystemOutput()
        {
            rotation = Camera.rotation,
            localRotation = Camera.localRotation,
            localEulerAngles = Camera.localEulerAngles
            
        };
    }
}

public class CameraSystemInput
{
    public float rotationX;

    public float weaponRecoil;
    public float weaponRecoilOpacity;
}

public class CameraSystemOutput
{
    public Quaternion rotation;
    public Quaternion localRotation;
    public Vector3 localEulerAngles;
}


[System.Serializable]
public class MinMax
{
    public float min;
    public float max;
}