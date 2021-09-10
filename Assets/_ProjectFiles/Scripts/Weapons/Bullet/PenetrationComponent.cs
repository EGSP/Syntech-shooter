#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrationComponent : MonoBehaviour
{
    [SerializeField] private PenetrationPreset Preset;

    public float PenetrationTresshold
    {
        get => 1- Preset.penetrationTresshold;
    }
}
