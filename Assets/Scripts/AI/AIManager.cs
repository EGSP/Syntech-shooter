using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    /// <summary>
    /// Вызывается при обновлении кадра
    /// </summary>
    public Action<AIUpdateData> OnUpdate = delegate { };

    /// <summary>
    /// Закешированный экземпляр
    /// </summary>
    private AIUpdateData updateData;

    private void Awake()
    {
        // Уничтожение компонента
        if (Instance != null)
            Destroy(this);

        Instance = this;

        updateData = new AIUpdateData();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        updateData.deltaTime = Time.deltaTime;
        updateData.GenerateRandomOne();

        OnUpdate(updateData);
    }
}

/// <summary>
/// Класс содержащий данные обновления AI
/// </summary>
public class AIUpdateData
{
    /// <summary>
    /// Time.deltaTime передаваемый через AIManager
    /// </summary>
    public float deltaTime { get; set; }

    /// <summary>
    /// Случайное число в диапазоне от 0 до 1. Для получения агентом использовать метод GetRandomOne
    /// </summary>
    public float RandomOne { get; private set; }

    /// <summary>
    /// Генерирует случайное значение в пределе от 0 до 1
    /// </summary>
    public void GenerateRandomOne()
    {
        RandomOne = UnityEngine.Random.Range(0f, 1f);
    }

    /// <summary>
    /// Возвращает случайное значение и заменяет его на другое для последующих вызовов
    /// </summary>
    /// <returns></returns>
    public float GetRandomOne()
    {
        return RandomOne = 1 - RandomOne;
    }
}
