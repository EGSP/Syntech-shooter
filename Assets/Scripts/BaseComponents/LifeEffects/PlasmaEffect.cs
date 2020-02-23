using UnityEngine;
[System.Serializable]

public class PlasmaEffect : DurationEffect
{
    public PlasmaEffect(float _EmmitTime, float _Duration, float _Damage) : base(_EmmitTime, _Duration)
    {
        ID = 101;
        ArmourPenetration = true;

        Damage = new DamageData();
        Damage.armourModifier = 1f;
        Damage.baseDamage = _Damage;
    }

    private DamageData Damage;

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

