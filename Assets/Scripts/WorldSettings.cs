using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class WorldSettings
{
    /// <summary>
    /// Сила гравитации
    /// </summary>
    public static float Gravity { get; set; }
    /// <summary>
    /// Направление силы гравитации в пространстве
    /// </summary>
    public static Vector3 GravityDirection { get; set; }




    public static void Setup()
    {
        GravityDirection = new Vector3(0, -1, 0);
        Gravity = 6f;
    }
}

