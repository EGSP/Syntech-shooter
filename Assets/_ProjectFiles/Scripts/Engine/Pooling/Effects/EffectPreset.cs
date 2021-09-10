using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewEffectPreset", menuName = "ScriptableObjects/EffectPreset", order = 1)]
public class EffectPreset : ScriptableObject
{
    [Tooltip("Идентификатор для нахождения этого объекта в пуле")]
    public string ID;

    public EffectObject Prefab;

    [Tooltip("Количество объектов, создаваемых при загрузке пула")]
    public int SpawnCount;
}
