using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

/// <summary>
/// Класс настроек поведения мира отдельной сцены
/// </summary>
public class WorldSettings : MonoBehaviour
{
    public static WorldSettings Instance;

    /// <summary>
    /// Сила гравитации
    /// </summary>
    public float Gravity { get => gravity; private set => gravity = value; }
    [SerializeField] private float gravity;

    /// <summary>
    /// Направление вектора силы гравитации в пространстве
    /// </summary>
    public Vector3 GravityDirection { get => gravityDirection; private set => gravityDirection = value; }
    [SerializeField] private Vector3 gravityDirection;

    /// <summary>
    /// Максимальная длинна вектора скорости объектов
    /// </summary>
    public float MaxVelocityMagnitude { get => maxVelocityMagnitude; private set => maxVelocityMagnitude = value; }
    [SerializeField] private float maxVelocityMagnitude;

    /// <summary>
    /// Вектор силы гравитации
    /// </summary>
    public Vector3 GravityVector { get => GravityDirection * Gravity; }



    private void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Two or more WorldSettings in Scene");

        Instance = this;
    }
    
}

