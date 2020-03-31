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
    public List<RobotBuilderData> Data { get => data; private set => data = value; }
    private List<RobotBuilderData> data;

    [Header("Spawn")]
    [SerializeField] private Transform SpawnPosition;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (IsInitialized)
            return;

        Data = new List<RobotBuilderData>();

        // Получаем всех загруженных роботов
        var bundles = GameManager.Instance.CompanionAssetManager.CompanionBundles;

        foreach(var a in bundles)
        {
            var newPrefab = Instantiate(a.Value.companionPrefab);

            newPrefab.gameObject.SetActive(false);

            Data.Add(new RobotBuilderData()
            {
                RobotPrefab = newPrefab,
                CompanionBundle = a.Value
            });
        }

        Resources.UnloadUnusedAssets();

        IsInitialized = true;
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        Data = new List<RobotBuilderData>();

        // Получаем всех загруженных роботов
        var bundles = GameManager.Instance.CompanionAssetManager.CompanionBundles;

        foreach (var a in bundles)
        {
            var newPrefab = Instantiate(a.Value.companionPrefab);

            newPrefab.gameObject.SetActive(false);

            Data.Add(new RobotBuilderData()
            {
                RobotPrefab = newPrefab,
                CompanionBundle = a.Value
            });
        }

        Resources.UnloadUnusedAssets();

        IsInitialized = true;

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
    /// Префаб, который уже отработал Awake
    /// </summary>
    public SignalAIBehaviour RobotPrefab;

    /// <summary>
    /// Бандл, в котором хранится информация о роботе
    /// </summary>
    public CompanionBundle CompanionBundle { get; set; }
    
    /// <summary>
    /// Текущее количество деталей 
    /// </summary>
    public int Current { get => current; private set => current = Mathf.Clamp(value, 0, CompanionBundle.details); }
    private int current;

    /// <summary>
    /// Готов ли робот к постройке
    /// </summary>
    public bool ReadyToBuild { get => Current == CompanionBundle.details; }

    /// <summary>
    /// Законченность постройки от 0 до 1
    /// </summary>
    public float BuildOpacity { get => (float)Current / (float)CompanionBundle.details; }

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
