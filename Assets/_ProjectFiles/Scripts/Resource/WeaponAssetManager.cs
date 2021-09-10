using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using Newtonsoft.Json;

using ResourceSystems;
using WeaponSystem;


public class WeaponAssetManager : MonoBehaviour
{
    /// <summary>
    /// Содержит название и конфиг оружия. Название совпадает с названием ассетбандла
    /// </summary>
    private List<WeaponConfig> AllWeapons;

    /// <summary>
    /// Загруженные бандлы с оружием. Строка - это id оружия
    /// </summary>
    private Dictionary<string, WeaponBundle> WeaponBundles;

    /// <summary>
    /// Путь к папке с оружием
    /// </summary>
    private string WeaponsFolder;

    public void Initialize()
    {
        WeaponsFolder = ConfigManager.AssetBundlesFolder + "weapons\\";

        AllWeapons = new List<WeaponConfig>();
        //InitializeWeapons();

        WeaponBundles = new Dictionary<string, WeaponBundle>();
    }

    /// <summary>
    /// Заполняет dictionary пустыми значениями, а после загружает все оружие
    /// </summary>
    private void InitializeWeapons()
    {
        StartCoroutine(LoadAllWeaponConfigs());
    }

    /// <summary>
    /// Загрузка оружия по названию
    /// </summary>
    /// <param name="name">Название оружия, которое полностью соотвествует названию папки и файла</param>
    public void LoadWeaponByName(string name)
    {
        if (AllWeapons.FirstOrDefault(x => x.Name == name) == null)
            throw new System.Exception($"Попытка загрузить оружия, которое не числится в конфигах: {name}");

        // Если подобный ассет уже был загружен
        if (WeaponBundles.ContainsKey(name))
            return;

        StartCoroutine(LoadWeaponBundle(name));
    }

    public void LoadWeaponByRarity()
    {

    }

    /// <summary>
    /// Загрузка списка оружия с краткими данными
    /// </summary>
    private IEnumerator LoadAllWeaponConfigs()
    {
        var path = ConfigManager.AssetBundlesFolder;
        path += WeaponsFolder + "weaponsconfigs.txt";

        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(fs);

        while (!sr.EndOfStream)
        {
            var weaponConfig = JsonConvert.DeserializeObject<WeaponConfig>(sr.ReadLine());
            AllWeapons.Add(weaponConfig);
        }

        sr.Close();
        fs.Close();

        yield break;
    }

    /// <summary>
    /// Загрузка бандла по имени
    /// </summary>
    /// <param name="name">Имя бандла соответсвует имени файла</param>
    private IEnumerator LoadWeaponBundle(string name)
    {
        
        // name - это имя папки и бандла
        var assetRequest = AssetBundle.LoadFromFileAsync(WeaponsFolder + $"{name}\\{name}");

        yield return assetRequest;

        // Если ассет бандл не загрузился
        if (assetRequest.assetBundle == null)
            throw new System.Exception($"Null assetbundle: {name}");

        // В конструкторе weaponBundle происходит загрузка необъодимых компонентов из бандла
        var weaponBundle = new WeaponBundle(name, assetRequest.assetBundle);
        WeaponBundles.Add(name, weaponBundle);
    }

    /// <summary>
    /// Добавление бандла с оружием. Если бандл с соответствующим id уже был загружен, то новый будет выгружен
    /// </summary>
    /// <param name="weaponBundle"></param>
    public void AddWeaponBundle(WeaponBundle weaponBundle)
    {
        if (!WeaponBundles.ContainsKey(weaponBundle.ID))
        {
            WeaponBundles.Add(weaponBundle.ID, weaponBundle);
        }
        else
        {
            // Если подобный бандл уже был загружен
            weaponBundle.Unload();
        }
    }

    /// <summary>
    /// Получение загруженного бандла с оружием
    /// </summary>
    /// <param name="id">Идентификатор оружия</param>
    public WeaponBundle GetWeaponBundleByID(string id)
    {
        if (WeaponBundles.ContainsKey(id))
        {
            return WeaponBundles[id];
        }
        else
        {
            throw new System.NullReferenceException();
        }
    }

}


/// <summary>
/// Бандл оружия, загружаемый из файла
/// </summary>
[System.Serializable]
public class WeaponBundle
{
    public WeaponBundle()
    {
        // Пустой конструктор для дочерних классов
    }

    /// <param name="_assetBundle">Бандл содержащий оружие</param>
    public WeaponBundle(string id, AssetBundle _assetBundle)
    {
        ID = id;
        assetBundle = _assetBundle;

        weaponComponentPrefab = assetBundle.LoadAsset<WeaponComponent>(id+"_weapon");
        ammoSprite = assetBundle.LoadAsset<Sprite>(id + "_ammosprite");
        ammoPrefab = assetBundle.LoadAsset<Ammo>(id+"_ammo");
    }

    /// <summary>
    /// Идентификатор оружия
    /// </summary>
    public string ID { get; protected set; }

    /// <summary>
    /// Бандл из которого были загружены ассеты
    /// </summary>
    public AssetBundle assetBundle { get; protected set; }

    /// <summary>
    /// Компонент оружия, который висит на GameObject
    /// </summary>
    public WeaponComponent weaponComponentPrefab { get; protected set; }
    
    /// <summary>
    /// Спрайт оружия
    /// </summary>
    public Sprite weaponSprite { get; protected set; }

    /// <summary>
    /// Спрайт боеприпаса
    /// </summary>
    public Sprite ammoSprite { get; protected set; }

    /// <summary>
    /// Префаб боеприпаса
    /// </summary>
    public Ammo ammoPrefab { get; protected set; }

    public virtual void Unload()
    {

    }
}

[System.Serializable]
public class ImitativeWeaponBundle : WeaponBundle
{

    public void Initialize()
    {
        weaponComponentPrefab = _weaponComponentPrefab;
        weaponSprite = _weaponSprite;
        ammoSprite = ammoSprite;

        ID = weaponComponentPrefab.ID;
    }

    /// <summary>
    /// Префаб для инспектора
    /// </summary>
    public WeaponComponent _weaponComponentPrefab;

    /// <summary>
    /// Спрайт оружия для редактора
    /// </summary>
    public Sprite _weaponSprite;

    public Sprite _ammoSprite;

}