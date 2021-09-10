#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;


public class EffectManager : MonoBehaviour
{
    #region Singleton
    public static EffectManager Instance;
    private void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Попытка создания второго экземпляра PoolManager");

        Instance = this;
    }
    #endregion
    public List<EffectPreset> Presets;
    private Dictionary<string, CycleList<EffectObject>> PooledObjects;

    /// <summary>
    /// Вызывается при обновлении кадра
    /// </summary>
    public Action<float> OnUpdate = delegate { };

    private void Start()
    {
        // Создаём экземпляр словаря
        PooledObjects = new Dictionary<string, CycleList<EffectObject>>();

        var parentObject = gameObject;
        parentObject.name = "EffectManager";

        foreach (EffectPreset preset in Presets)
        {
            // Создание "каталога" для новых объектов
            GameObject catalog = new GameObject();
            catalog.name = preset.ID + "- Catalog";
            catalog.transform.parent = parentObject.transform;
            catalog.transform.localPosition = Vector3.zero;


            // Создаём очередь для вставки в словарь
            var newIPooledObjects = new CycleList<EffectObject>();

            // Добавляем объекты IPooledObject в очередь
            for (int i = 0; i < preset.SpawnCount; i++)
            {
                var iPooledObject = Instantiate(preset.Prefab, catalog.transform);

                // Передаём ссылку на родительскую очередь
                iPooledObject.Initialize();

                newIPooledObjects.Add(iPooledObject);
            }

            // Добавляем в словарь заполненную очередь
            PooledObjects.Add(preset.ID, newIPooledObjects);
        }
    }

    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    public EffectObject Take(string ID)
    {
        if (!PooledObjects.ContainsKey(ID))
            throw new System.Exception("Вы пытаетесь получить объект пула которого не существует: " + ID);

        var cycleList = PooledObjects[ID];
        
        // Получение объекта из списка
        var obj = cycleList.Next();

        return obj;
    }
}

/// <summary>
/// Данный список возвращает элементы циклически
/// </summary>
public class CycleList<T> where T : EffectObject
{
    public CycleList()
    {
        TValues = new LinkedList<T>();
    }
    public CycleList(List<T> _TValues)
    {
        TValues = new LinkedList<T>();
        for (int i = 0; i < _TValues.Count; i++)
        {
            TValues.AddLast(_TValues[i]);
        }

        if (TValues.Count == 0)
            throw new System.Exception("No elemnts in CycleList<" + typeof(T) + ">");

        currentElement = TValues.First;
    }
    public CycleList(LinkedList<T> _TValues)
    {
        TValues = _TValues;

        if (TValues.Count == 0)
            throw new System.Exception("No elemnts in CycleList<"+typeof(T)+">");

        currentElement = TValues.First;
    }

    private LinkedList<T> TValues;
    // Возвращаемый элемент
    private LinkedListNode<T> currentElement;
    
    /// <summary>
    /// Возвращает следующий свободный элемент списка
    /// </summary>
    public T Next()
    {
        var nextElement = currentElement.Next;
        
        for(int i = 0; i < TValues.Count; i++)
        {
            if (nextElement == null)
                nextElement = TValues.First;

            currentElement = nextElement;

            // Если элемент свободен
            if (currentElement.Value.IsFree)
            {
                break;
            }

            nextElement = currentElement.Next;
        }

        currentElement.Value.StopEffect();

        return currentElement.Value;
    }


    /// <summary>
    /// Добавленяет элемент в конец списка
    /// </summary>
    public void Add(T _Value)
    {
        TValues.AddLast(_Value);
        currentElement = TValues.Last;
    }

}
