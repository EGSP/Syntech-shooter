#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PoolManager : MonoBehaviour
{
    #region Singleton
    public static PoolManager Instance;
    private void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Попытка создания второго экземпляра PoolManager");

        Instance = this;
    }
    #endregion


    public List<PoolPreset> Presets;
    private Dictionary<string, Queue<PooledObject>> PooledObjects;

    
    void Start()
    {
        // Создаём экземпляр словаря
        PooledObjects = new Dictionary<string, Queue<PooledObject>>();

        var parentObject = gameObject;

        foreach(PoolPreset preset in Presets)
        {
            // Создание "каталога" для новых объектов
            GameObject catalog = new GameObject();
            catalog.name = preset.ID + "- Catalog";
            catalog.transform.parent = parentObject.transform;
            
            // Создаём очередь для вставки в словарь
            var newIPooledObjects = new Queue<PooledObject>();
            
            // Добавляем объекты IPooledObject в очередь
            for (int i = 0; i < preset.SpawnCount; i++)
            {
                var iPooledObject = Instantiate(preset.Prefab);
                iPooledObject.transform.parent = catalog.transform;

                // Передаём ссылку на родительскую очередь
                iPooledObject.InitializeByPool(newIPooledObjects);

                newIPooledObjects.Enqueue(iPooledObject);
            }

            // Добавляем в словарь заполненную очередь
            PooledObjects.Add(preset.ID, newIPooledObjects);
        }


    }

    public PooledObject Take(string ID)
    {
        if (!PooledObjects.ContainsKey(ID))
            throw new System.Exception("Вы пытаетесь получить объект пула которого не существует: " + ID);

        var queue = PooledObjects[ID];

        // Создание дополнительных экземпляров объекта
        if(queue.Count == 0)
        {
            var preset = Presets.FirstOrDefault(x => x.ID == ID);
            
            for(int i = 0; i < preset.LateSpawnCount; i++)
            {
                var iPooledObject = Instantiate(preset.Prefab);
                // Передаём ссылку на родительскую очередь
                iPooledObject.InitializeByPool(queue);
                queue.Enqueue(iPooledObject);
            }
        }

        // Получение объекта из очереди
        var obj = queue.Dequeue();
        obj.OnSpawnFromPool();

        return obj;
    }
    
}

public abstract class PooledObject : MonoBehaviour
{
    // Ссылка на родительский очередь, чтобы пул не искал нужную очередь, а брал ссылку из этого объекта
    public Queue<PooledObject> ParentQueue { get; private set; }
    /// <summary>
    /// Назначение родительского пула
    /// </summary>
    public void InitializeByPool(Queue<PooledObject> _ParentQueue)
    {
        ParentQueue = _ParentQueue;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Вызывается самим объектом при необъодимости уничтожения
    /// </summary>
    public virtual void InsertToPool()
    {
        gameObject.SetActive(false);
        ParentQueue.Enqueue(this);
    }


    /// <summary>
    /// Вызывается из вне пулом при спавне
    /// </summary>
    public virtual void OnSpawnFromPool()
    {
        gameObject.SetActive(true);
    }
}
