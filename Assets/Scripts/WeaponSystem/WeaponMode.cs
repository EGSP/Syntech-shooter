using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewWeaponMode", menuName = "ScriptableObjects/WeaponMode", order = 1)]
public class WeaponMode : ScriptableObject
{
    public string Name; // Название режима стрельбы
    public string MagazineID; // Идентификатор для подбора магазина
    [Space]
    public float ReloadTime; // Время перезарядки
    public float FireRate; // Скорострельность
    public float RecoilForce; // Сила отдачи
    [Range(0, 90)]
    public float SpreadY, SpreadX; // Максимальный разброс 

    [Range(0, 5)]
    // Длинна шага пробития
    public float PenetrationStepLength;
    public int BulletPerShoot; // Количество снарядов за выстрел
    public float BulletFlyDistance; // Максимальная дистанция полёта
}
