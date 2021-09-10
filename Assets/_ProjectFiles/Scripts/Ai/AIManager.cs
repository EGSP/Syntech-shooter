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

    /// <summary>
    /// Контроллер игрока. Всегда не null
    /// </summary>
    public PlayerControllerComponent PlayerControllerComponent { get; private set; }

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
        // Если компонент игрока пуст, то не обновляется AI
        if (PlayerControllerComponent == null)
            return;

        updateData.deltaTime = Time.deltaTime;
        updateData.GenerateRandomOne();
        updateData.playerControllerComponent = PlayerControllerComponent;

        OnUpdate(updateData);
    }

    /// <summary>
    /// Установка контроллера игрока
    /// </summary>
    /// <param name="playerControllerComponent">Контроллер игрока</param>
    public void SetPlayerController(PlayerControllerComponent playerControllerComponent)
    {
        PlayerControllerComponent = playerControllerComponent;
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
    /// Контроллер игрока. Всегда не равен null
    /// </summary>
    public PlayerControllerComponent playerControllerComponent;

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
