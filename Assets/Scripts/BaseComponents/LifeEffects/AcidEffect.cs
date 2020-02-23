using UnityEngine;

using LifeEffects;
[System.Serializable]
public class AcidEffect : DurationEffect
{
    public AcidEffect(float _Duration, float _Damage,float _SpeedModifier, float _ModifierTime) : base(_ModifierTime, _Duration)
    {
        ID = 103;

        Damage = new DamageData();
        Damage.armourModifier = 1f;
        Damage.baseDamage = _Damage;
        

        SpeedModifier = _SpeedModifier;
        ModifierTime = _ModifierTime;
    }

    private DamageData Damage;

    // Визуальное представление данного эффекта
    private EffectObject VFX;

    private float SpeedModifier { get; }
    private float ModifierTime { get; }

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

        Carrier = component;

        EnqueueModifier(component);
    }

    private void EnqueueModifier(LifeComponent component)
    {
        var speedLife = component as ISpeedModifiable;
        // Если тело с изменяемой скоростью 
        if (speedLife != null)
        {
            speedLife.EnqueueModifier(SpeedModifier, ModifierTime);
        }
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        base.Merge(effect);
        EnqueueModifier(Carrier);
        return this;
    }
}

