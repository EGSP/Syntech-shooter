
public abstract class DurationEffect : LifeComponentEffect
{
    public DurationEffect(float _EmmitTime, float _Duration)
    {
        ID = -100;
        EmmitTime = _EmmitTime;
        emmitTime = EmmitTime;

        Duration = _Duration;
        duration = Duration;
        
    }

    private float EmmitTime;
    private float emmitTime;

    private float Duration;
    private float duration;

    public override LifeComponentEffect Invoke(LifeComponent component, float deltaTime)
    {
        emmitTime -= deltaTime;

        duration -= deltaTime;
        if (duration < 0)
        {
            duration = Duration;

            OnDuration(component);
        }

        if (emmitTime < 0)
            return null;

        return this;
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        var anotherEffect = effect as DurationEffect;

        emmitTime = anotherEffect.EmmitTime;
        Duration = anotherEffect.Duration;

        return this;
    }

    protected abstract void OnDuration(LifeComponent component);
}

