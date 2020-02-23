using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveArmour
{
    public ActiveArmour(ActiveArmourPreset Preset)
    {
        MaxBattery = Preset.MaxBattery;
        Battery = MaxBattery;

        RegenerationDelay = Preset.RegenerationDelay;
        RegenerationSpeed = Preset.RegenerationSpeed;
        IsRegenerationTime = false;

        timerCallbacker = new TimerCallbacker(RegenerationDelay);
        timerCallbacker.OnEmmitionEndCallback += () => IsRegenerationTime = true;
        timerCallbacker.OnResetCallback += () => IsRegenerationTime = false;
    }

    // Максимальный показатель брони
    public float MaxBattery { get; protected set; }
    /// <summary>
    /// Текущий показатель брони
    /// </summary>
    public float Battery
    {
        get => battery;
        protected set
        {
            IsActive = true;
            if (value <= 0)
            {
                value = 0;
                // Броня неактивна
                IsActive = false;
            }

            if (value > MaxBattery)
                value = MaxBattery;
            battery = value;
        }
    }
    private float battery;

    /// <summary>
    /// Активна ли броня на данный момент 
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Задрежка перед началом восстановления
    /// </summary>
    public float RegenerationDelay { get; private set; }

    /// <summary>
    /// Скорость восстановление за единицу времени
    /// </summary>
    public float RegenerationSpeed { get; private set; }

    private bool IsRegenerationTime;

    private TimerCallbacker timerCallbacker;


    // Update is called once per frame
    public void Update(float deltaTime)
    {
        if (IsRegenerationTime)
        {
            Regenerate(deltaTime);
        }

        timerCallbacker.Update(deltaTime);
    }


    public void Regenerate(float deltaTime)
    {
        Battery += RegenerationSpeed * deltaTime;
    }

    public void Hurt(float value)
    {
        timerCallbacker.Reset();

        Battery -= value;
    }


    
}
