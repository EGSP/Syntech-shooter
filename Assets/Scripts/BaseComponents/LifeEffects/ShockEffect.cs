using UnityEngine;

using LifeEffects;
[System.Serializable]
public class ShockEffect : DurationEffect
{
    public ShockEffect(float _ShockTime):base(_ShockTime,_ShockTime)
    {
        ID = 102;
        ShockTime = _ShockTime;
    }
    
    private float ShockTime { get; }

    // Визуальное представление данного эффекта
    private EffectObject VFX;

    private LifeComponent Carrier;

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

    public override void SetLifeComponent(LifeComponent component)
    {
        VFX = EffectManager.Instance.Take(ID.ToString());
        VFX.IsFree = false;
        VFX.PlayEffect();

        VFX.transform.SetParent(component.transform);
        VFX.transform.localPosition = Vector3.zero;

        Carrier = component;

        ShockLifeComponent(component);
    }

    private void ShockLifeComponent(LifeComponent component)
    {
        var shockable = component as IShockable;

        if(shockable != null)
        {
            shockable.Shock(ShockTime);
        }
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        base.Merge(effect);
        ShockLifeComponent(Carrier);
        return this;
    }


    protected override void OnDuration(LifeComponent component)
    {
        return;
    }
}
