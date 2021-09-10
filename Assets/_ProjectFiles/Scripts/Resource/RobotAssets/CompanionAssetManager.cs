using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AIB.AIBehaviours;

public class CompanionAssetManager : MonoBehaviour
{
    /// <summary>
    /// Коллекция бандлов с компаньонами
    /// </summary>
    public Dictionary<string, CompanionBundle> CompanionBundles { get; private set; }

    public void Initialize()
    {
        CompanionBundles = new Dictionary<string, CompanionBundle>();
    }

    /// <summary>
    /// Добавление бандла с оружием. Если бандл с соответствующим id уже был загружен, то новый будет выгружен
    /// </summary>
    /// <param name="weaponBundle"></param>
    public void AddCompanionBundle(CompanionBundle companionBundle)
    {
        if (!CompanionBundles.ContainsKey(companionBundle.ID))
        {
            CompanionBundles.Add(companionBundle.ID, companionBundle);
        }
        else
        {
            // Если подобный бандл уже был загружен
            companionBundle.Unload();
        }
    }

    /// <summary>
    /// Получение загруженного бандла с роботом
    /// </summary>
    /// <param name="id">Идентификатор оружия</param>
    public CompanionBundle GetWeaponBundleByID(string id)
    {
        if (CompanionBundles.ContainsKey(id))
        {
            return CompanionBundles[id];
        }
        else
        {
            throw new System.NullReferenceException();
        }
    }
}


[System.Serializable]
public class CompanionBundle
{
    public CompanionBundle()
    {
        // Для дочерних классов
    }

    public CompanionBundle(string id, AssetBundle _assetBundle)
    {
        ID = id;
        assetBundle = _assetBundle;
    }

    /// <summary>
    /// Идентификатор робота
    /// </summary>
    public string ID { get; protected set; }

    /// <summary>
    /// Бандл из которого были загружены ассеты
    /// </summary>
    public AssetBundle assetBundle { get; protected set; }

    /// <summary>
    /// Компонент оружия, который висит на GameObject
    /// </summary>
    public SignalAIBehaviour companionPrefab { get; protected set; }

    /// <summary>
    /// Спрайт класса робота
    /// </summary>
    public Sprite companionIcon { get; protected set; }

    /// <summary>
    /// Реальное изображение робота
    /// </summary>
    public Sprite companionImage { get; protected set; }

    /// <summary>
    /// Количество частей для постройки робота
    /// </summary>
    public int details { get; protected set; }
    

    /// <summary>
    /// Компонент отрисовки характеристик в верстаке
    /// </summary>
    public RobotCharacters companionCharacters { get; protected set; }

    public virtual void Unload()
    {

    }
}


[System.Serializable]
public class ImitativeCompanionBundle: CompanionBundle
{
    public void Initialize()
    {
        companionPrefab = _companionPrefab;
        companionIcon = _companionIcon;
        companionImage = _companionImage;
        companionCharacters = _companionCharacters;
        details = _details;

        ID = companionPrefab.ID;
    }

    /// <summary>
    /// Компонент оружия, который висит на GameObject
    /// </summary>
    [SerializeField] public SignalAIBehaviour _companionPrefab;

    /// <summary>
    /// Спрайт класса робота
    /// </summary>
    [SerializeField] public Sprite _companionIcon;

    [SerializeField] public Sprite _companionImage;

    /// <summary>
    /// Компонент отрисовки характеристик в верстаке
    /// </summary>
    [SerializeField] public RobotCharacters _companionCharacters;

    /// <summary>
    /// Количество деталей для постройки
    /// </summary>
    [SerializeField] private int _details;
}