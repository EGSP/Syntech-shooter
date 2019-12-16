#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DashSkill
{
    [Range(0,5)]
    [SerializeField] private float LifeTime;
    [SerializeField] private AnimationCurve SpeedModifierOverLifetime;
    [SerializeField] private AnimationCurve RotationYOverLifetime;
    
    
    public bool Active { get; private set; }
    
    // Dash start keyinput
    private Vector2 dashInput;
    private float lifeTime;

    public void Start()
    {
        lifeTime = LifeTime;
    }

    public DashOutput Update(float deltaTime)
    {
        if (Active == false)
            return new DashOutput(true);

        lifeTime -= deltaTime;
        if (lifeTime < 0)
        {
            lifeTime = 0;
            Active = false;
        }

        var evaluate = 1 - lifeTime / LifeTime;
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
        if (Active == false)
        {
            Active = true;
            lifeTime = LifeTime;
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
