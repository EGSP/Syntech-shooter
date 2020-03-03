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
    /// Загруженные бандлы с оружием. Строка - это имя оружия
    /// </summary>
    private Dictionary<string, WeaponBundle> WeaponBundles;

    /// <summary>
    /// Путь к папке с оружием
    /// </summary>
    private string WeaponsFolder;

    private void Awake()
    {
        WeaponsFolder = ConfigManager.AssetBundlesFolder + "weapons\\";

        AllWeapons = new List<WeaponConfig>();
        InitializeWeapons();

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

}


/// <summary>
/// Бандл оружия, загружаемый из файла
/// </summary>
public class WeaponBundle
{
    /// <param name="_assetBundle">Бандл содержащий оружие</param>
    public WeaponBundle(string name, AssetBundle _assetBundle)
    {
        Name = name;
        assetBundle = _assetBundle;

        weaponComponentPrefab = assetBundle.LoadAsset<WeaponComponent>(name+"_weapon");
        ammoSprite = assetBundle.LoadAsset<Sprite>(name + "_ammosprite");
        ammoPrefab = assetBundle.LoadAsset<Ammo>(name+"_ammo");
    }

    /// <summary>
    /// Имя оружия
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Бандл из которого были загружены ассеты
    /// </summary>
    public AssetBundle assetBundle { get; private set; }

    /// <summary>
    /// Компонент оружия, который висит на GameObject
    /// </summary>
    public WeaponComponent weaponComponentPrefab { get; private set; }
    
    /// <summary>
    /// Спрайт боеприпаса
    /// </summary>
    public Sprite ammoSprite { get; private set; }

    /// <summary>
    /// Префаб боеприпаса
    /// </summary>
    public Ammo ammoPrefab { get; private set; }

    
}