using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponComponent))]
public class WeaponSettingsComponent : MonoBehaviour
{
    /// <summary>
    /// Оружие к которому закреплены эти настройки
    /// </summary>
    public WeaponComponent WeaponComponent { get; private set; }

    /// <summary>
    /// Начальная позиция в руках
    /// </summary>
    [Header("DefaultPosition")]
    public Vector3 DefaultPosition;

    /// <summary>
    /// Начальное вращение в руках. В углах эйлера
    /// </summary>
    public Vector3 DefaultRotation;

    private void Awake()
    {
        WeaponComponent = GetComponent<WeaponComponent>();

        if (WeaponComponent == null)
            throw new System.Exception($"WeaponComponent не был найден на игровом объекте {gameObject.name}");
    }
}
