#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PoolManager : MonoBehaviour
{
    
    public static PoolManager Instance;
    
    
    public List<PoolPreset> Presets;

    /// <summary>
    /// Объекты хранящиеся в пуле
    /// </summary>
    private Dictionary<string, Queue<PooledObject>> PooledObjects;

    
    void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Попытка создания второго экземпляра PoolManager");

        Instance = this;

        // Создаём экземпляр словаря
        PooledObjects = new Dictionary<string, Queue<PooledObject>>();

        var parentObject = gameObject;

        foreach(PoolPreset preset in Presets)
        {
            // Создание "каталога" для новых объектов
            GameObject catalog = new GameObject();
            catalog.name = preset.ID + "- Catalog";
            catalog.transform.parent = parentObject.transform;
            catalog.transform.localPosition = Vector3.zero;
            
            // Создаём очередь для вставки в словарь
            var newIPooledObjects = new Queue<PooledObject>();
            
            // Добавляем объекты IPooledObject в очередь
            for (int i = 0; i < preset.SpawnCount; i++)
            {
                var iPooledObject = Instantiate(preset.Prefab, catalog.transform);

                // Передаём ссылку на родительскую очередь
                iPooledObject.InitializeByPool(newIPooledObjects);
                iPooledObject.SetPoolID(preset.ID);

                newIPooledObjects.Enqueue(iPooledObject);
            }

            // Добавляем в словарь заполненную очередь
            PooledObjects.Add(preset.ID, newIPooledObjects);
        }


    }

    /// <summary>
    /// Получение объекта из пула по идентификатору. Объекты изначально включены
    /// </summary>
    /// <param name="ID">Идентификатор объекта в пуле</param>
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
                iPooledObject.SetPoolID(ID);

                queue.Enqueue(iPooledObject);
            }
        }

        // Получение объекта из очереди
        var obj = queue.Dequeue();
        obj.OnSpawnFromPool();

        return obj;
    }

    public Queue<PooledObject> GetParentQueue(string ID)
    {
        if (!PooledObjects.ContainsKey(ID))
            throw new System.Exception("Вы пытаетесь получить очередь пула которой не существует: " + ID);

        var queue = PooledObjects[ID];

        return queue;
    }
    
}

public abstract class PooledObject : MonoBehaviour
{
    [Tooltip("Идентификатор для нахождения этого объекта в пуле")]
    [SerializeField] protected string PoolID;

    /// <summary>
    /// Требуется ли аходить пул самостоятельно
    /// </summary>
    [SerializeField] private bool FindPool;

    // Ссылка на родительский очередь, чтобы пул не искал нужную очередь, а брал ссылку из этого объекта
    public Queue<PooledObject> ParentQueue { get; protected set; }

    protected virtual void Awake()
    {
        if (FindPool == true)
            SetParentQueue(PoolManager.Instance.GetParentQueue(PoolID));
    }

    /// <summary>
    /// Назначение родительского пула
    /// </summary>
    public void InitializeByPool(Queue<PooledObject> _ParentQueue)
    {
        ParentQueue = _ParentQueue;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Вызывается самим объектом при необходимости уничтожения
    /// </summary>
    public virtual void InsertToPool()
    {
        gameObject.SetActive(false);
        ParentQueue.Enqueue(this);
    }


    /// <summary>
    /// Вызывается из вне, пулом, при спавне
    /// </summary>
    public virtual void OnSpawnFromPool()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Установка родительской очереди пула
    /// </summary>
    /// <param name="parentQueue"></param>
    protected void SetParentQueue(Queue<PooledObject> parentQueue)
    {
        ParentQueue = parentQueue;
    }

    /// <summary>
    /// Установка ID в пуле. Нежелательно устанавливать вне пула
    /// </summary>
    /// <param name="poolID"></param>
    public void SetPoolID(string poolID)
    {
        PoolID = poolID;
    }
}
