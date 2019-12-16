using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewPoolPreset", menuName = "ScriptableObjects/PoolPreset", order = 1)]
public class PoolPreset : ScriptableObject
{
    [Tooltip("Идентификатор для нахождения этого объекта в пуле")]
    public string ID;

    public PooledObject Prefab;

    [Tooltip("Количество объектов, создаваемых при загрузке пула")]
    public int SpawnCount;

    [Range(1,100)]
    [Tooltip("Количество объектов, создаваемых при при нехватке в пуле данного типа объекта")]
    public int LateSpawnCount;
}
