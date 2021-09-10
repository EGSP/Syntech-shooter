#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[System.Serializable]
public class DashSkill
{
    /// <summary>
    /// Состояние в котором находится способность
    /// </summary>
    public enum SkillState
    {
        Done,
        Active,
        Reloading
    }

    /// <summary>
    /// Время действия рывка
    /// </summary>
    [Range(0,5)]
    [SerializeField] private float UseTimeLength;
    
    /// <summary>
    /// Время перезарядки способности
    /// </summary>
    [SerializeField] private float reloadTime;
    public float ReloadTime {
        get => reloadTime;
        private set
        {
            reloadTime = value;
        }
    }

    /// <summary>
    /// Модификатор скорости
    /// </summary>
    [SerializeField] private AnimationCurve SpeedModifierOverLifetime;

    /// <summary>
    /// Модификатор вращения по оси Y
    /// </summary>
    [SerializeField] private AnimationCurve RotationYOverLifetime;

    /// <summary>
    /// Текущее состояние способности
    /// </summary>
    public SkillState skillState
    {
        get => _skillState;
        private set
        {
            _skillState = value;
            OnSkillStateChanged(_skillState);
        }
    }
    private SkillState _skillState;
    
    public bool Active { get; private set; }
    
    // Dash start keyinput
    private Vector2 dashInput;

    private float useTimeLength;

    /// <summary>
    /// Текущее время перезарядки
    /// </summary>
    public float CurrentReloadTime
    {
        get => currentReloadTime;
        set
        {
            currentReloadTime = value;
            OnReloadValueChanged(currentReloadTime, ReloadTime);
        }
    }
    private float currentReloadTime;

    /// <summary>
    /// Вызывается при изменение значения перезарядки способности
    /// </summary>
    public Action<float, float> OnReloadValueChanged = delegate { };

    /// <summary>
    /// Вызывается при изменении состояния способности
    /// </summary>
    public Action<SkillState> OnSkillStateChanged = delegate { };

    public void Start()
    {
        useTimeLength = UseTimeLength;
        currentReloadTime = ReloadTime;

        skillState = SkillState.Done;
    }

    public DashOutput Update(float deltaTime)
    {
        // Если идет перезарядка
        if(skillState == SkillState.Reloading)
        {
            CurrentReloadTime -= Time.deltaTime;

            // Если перезарядка закончилась
            if (CurrentReloadTime < 0)
            {
                CurrentReloadTime = ReloadTime;
                skillState = SkillState.Done;
            }

            return new DashOutput(true);
        }

        // Если способность доступна, но не активирована
        if (skillState == SkillState.Done)
            return new DashOutput(true);

        useTimeLength -= deltaTime;
        if (useTimeLength < 0)
        {
            useTimeLength = 0;
            skillState = SkillState.Reloading;
        }

        var evaluate = 1 - useTimeLength / UseTimeLength;
        // Moving
        var modifier = SpeedModifierOverLifetime.Evaluate(evaluate);

        var horizontalModifier = modifier;
        var verticalModifier = modifier;

        // Rotating
        var rotationYModifier = RotationYOverLifetime.Evaluate(evaluate);

        return new DashOutput()
        {
            horizontalInput = (int)dashInput.x,
            verticalInput = (int)dashInput.y,
            speedModifier = new Vector2(horizontalModifier, verticalModifier),
            rotationXModifier = 1,
            rotationYModifier = rotationYModifier
        };
    }

    public void Use(float h,float v)
    {
        // Если скилл закончился
        if (skillState == SkillState.Done)
        {
            skillState = SkillState.Active;
            useTimeLength = UseTimeLength;
            dashInput = new Vector2(h, v);
        }
    }
}


public struct DashOutput
{
    /// <summary>
    /// Конструктор стандартных значений
    /// </summary>
    /// <param name="Default">Может быть любым, ни на что не виляет</param>
    public DashOutput(bool Default)
    {
        horizontalInput = 1;
        verticalInput = 1;

        speedModifier = Vector2.one;

        rotationXModifier = 1;
        rotationYModifier = 1;
    }

    public int horizontalInput;
    public int verticalInput;

    public Vector2 speedModifier;

    public float rotationXModifier;
    public float rotationYModifier;
}


