using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewActiveArmourPreset", menuName = "ScriptableObjects/BaseComponents/ActiveArmourPreset", order = 1)]
public class ActiveArmourPreset : ScriptableObject
{
    public float MaxBattery;

    public float RegenerationDelay;
    public float RegenerationSpeed;
}
