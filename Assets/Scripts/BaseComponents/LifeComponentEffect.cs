using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LifeComponentEffect
{
    public int ID { get; protected set; }

    // Возвращаемый эффект. Если эффект закончился, то вернётся null
    public abstract LifeComponentEffect Invoke(LifeComponent component, float deltaTime);

    // Слияние эффектов
    public abstract LifeComponentEffect Merge(LifeComponentEffect effect);

    // Метод для передачи компонента
    public virtual void SetLifeComponent(LifeComponent component)
    {
        return;
    }
    
}


public class NullLifeComponentEffect : LifeComponentEffect
{
    public NullLifeComponentEffect()
    {
        ID = -1;
    }

    public override LifeComponentEffect Invoke(LifeComponent component, float deltaTime)
    {
        return null;
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        return this;
    }
}
