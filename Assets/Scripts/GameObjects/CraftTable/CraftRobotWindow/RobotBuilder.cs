using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AIB.AIBehaviours;

using System.Linq;

public class RobotBuilder : MonoBehaviour
{
    /// <summary>
    /// Список роботов для постройки с текущим количеством деталей
    /// </summary>
    public List<RobotBuilderData> Data { get => data; }
    [SerializeField] private List<RobotBuilderData> data;

    [Header("Spawn")]
    [SerializeField] private Transform SpawnPosition;

    private void Awake()
    {
        for(int i = 0; i < Data.Count; i++)
        {
            var newPrefab = Instantiate(Data[i].RobotPrefab);
            newPrefab.gameObject.SetActive(false);

            Data[i].RobotPrefab = newPrefab;
        }

        Resources.UnloadUnusedAssets();
    }

    public SignalAIBehaviour BuildRobot(string id)
    {
        RobotBuilderData data = Data.FirstOrDefault(x => x.RobotPrefab.ID == id);

        SignalAIBehaviour prefab = data.RobotPrefab;
        var instance = Instantiate(prefab);

        data.Reset();

        return instance;
    }

}


/// <summary>
/// Информация по постройке робота
/// </summary>
[System.Serializable]
public class RobotBuilderData
{
    /// <summary>
    /// Префаб создаваемого робота
    /// </summary>
    public SignalAIBehaviour RobotPrefab;

    /// <summary>
    /// Количество частей для постройки робота
    /// </summary>
    public int Details { get => details; private set => details = value; }
    [SerializeField] private int details;

    /// <summary>
    /// Текущее количество деталей 
    /// </summary>
    public int Current { get => current; private set => current = Mathf.Clamp(value, 0, Details); }
    [SerializeField] private int current;

    /// <summary>
    /// Готов ли робот к постройке
    /// </summary>
    public bool ReadyToBuild { get => Current == Details; }

    /// <summary>
    /// Добавление детали
    /// </summary>
    /// <param name="count">Количество добавляемых деталей</param>
    public void AddDetail(int count)
    {
        Current += count;
    }

    public void Reset()
    {
        Current = 0;
    }
}
