﻿using UnityEngine;
[System.Serializable]
public class FireEffect : DurationEffect
{
    public FireEffect(float _EmmitTime, float _Duration, float _Damage) : base(_EmmitTime, _Duration)
    {
        ID = 100;

        Damage = _Damage;
    }

    private float Damage;

    // Визуальное представление данного эффекта
    private EffectObject VFX;

    public override LifeComponentEffect Invoke(LifeComponent component, float deltaTime)
    {
        
        var outputComponent = base.Invoke(component, deltaTime);

        if (outputComponent == null)
        {
            VFX.StopEffect();
            VFX.IsFree = true;

            return null;
        }
        return outputComponent;
    }

    protected override void OnDuration(LifeComponent component)
    {
        component.Hurt(Damage);
    }

    public override void SetLifeComponent(LifeComponent component)
    {
        VFX = EffectManager.Instance.Take(ID.ToString());
        VFX.IsFree = false;
        VFX.PlayEffect();

        VFX.transform.SetParent(component.transform);
        VFX.transform.localPosition = Vector3.zero;
       
    }
}
    

