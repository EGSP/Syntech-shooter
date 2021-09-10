using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "NewPenetrationPreset", menuName = "ScriptableObjects/PenetrationPreset", order = 1)]
public class PenetrationPreset : ScriptableObject
{
    [Range(0, 1)]
    [Tooltip("Сопротивление пробитию")]
    public float penetrationTresshold;
}
