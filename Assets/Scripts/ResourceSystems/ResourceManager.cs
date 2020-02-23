using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public void Awake()
    {
        if (Instance != null)
            throw new System.Exception("ResourceManager singleton Exception");

        Instance = this;
    }

    /// <summary>
    /// Хранит ассет спрайта боеприпаса
    /// </summary>
    public Dictionary<string, Sprite> AmmoSprites { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        AmmoSprites = new Dictionary<string, Sprite>();
        StartCoroutine(LoadAmmoSpriteAsync("9"));
        StartCoroutine(LoadAmmoSpriteAsync("10"));

    }

    /// <summary>
    /// Получение спрайта патрона
    /// </summary>
    /// <param name="recourceName"></param>
    /// <returns></returns>
    public Sprite GetAmmoSprite(string recourceName)
    {
        return AmmoSprites[recourceName];
    }

    /// <summary>
    /// Загрузка спрайта боеприпаса ассинхронно по имени. Использует Resources unity class
    /// </summary>
    /// <param name="resourceName">Имя файла спрайта без расширения</param>
    public IEnumerator LoadAmmoSpriteAsync(string resourceName)
    {
        if (AmmoSprites.ContainsKey(resourceName))
            yield break;

        var resourceReq = Resources.LoadAsync<Sprite>("Sprites/" + resourceName);
        
        // Ждем конца загрузки
        while (!resourceReq.isDone)
        {
            yield return null;
        }

        // Если ассет не был найден
        if(resourceReq.asset == null)
        {
            Debug.Log(resourceName + "Doesn`t exist in Resources/Sprites/");
        }
        else
        {
            AmmoSprites.Add(resourceName, resourceReq.asset as Sprite);
        }
    }

    /// <summary>
    /// Выгрузка неиспользуемых ресурсов
    /// </summary>
    public IEnumerator UnloadUnusedResources()
    {
        yield return Resources.UnloadUnusedAssets();
    }
}
