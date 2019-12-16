[System.Serializable]
public class PoisonGasEffect : LifeComponentEffect
{
    public PoisonGasEffect(float _EmmitTime, float _Duration, float _hurtValue)
    {
        ID = 100;

        EmmitTime = _EmmitTime;
        emmitTime = EmmitTime;

        Duration = _Duration;
        duration = Duration;

        hurtValue = _hurtValue;
    }

    private float EmmitTime;
    private float emmitTime;

    private float Duration;
    private float duration;

    private float hurtValue;

    public override LifeComponentEffect Invoke(LifeComponent component, float deltaTime)
    {
        emmitTime -= deltaTime;

        duration -= deltaTime;
        if (duration < 0)
        {
            duration = Duration;

            component.Hurt(hurtValue);
        }

        if (emmitTime < 0)
            return null;

        return this;
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        var anotherEffect = effect as PoisonGasEffect;

        emmitTime = anotherEffect.EmmitTime;
        Duration = anotherEffect.Duration;
        

        return this;
    }
}