using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewLifeComponentPreset", menuName = "ScriptableObjects/BaseComponents/LifeComponentPreset", order = 1)]
public class LifeComponentPreset : ScriptableObject
{
    public float MaxHealth;
}
