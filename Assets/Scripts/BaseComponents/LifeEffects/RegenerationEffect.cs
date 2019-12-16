[System.Serializable]
public class RegenerationEffect : LifeComponentEffect
{
    public RegenerationEffect(float _EmmitTime, float _Duration, float _regenValue)
    {
        ID = 1;

        EmmitTime = _EmmitTime;
        emmitTime = EmmitTime;

        Duration = _Duration;
        duration = Duration;

        regenValue = _regenValue;
    }

    private float EmmitTime;
    private float emmitTime;

    private float Duration;
    private float duration;

    private float regenValue;
    
    public override LifeComponentEffect Invoke(LifeComponent component, float deltaTime)
    {
        emmitTime -= deltaTime;
        
        duration -= deltaTime;
        if(duration < 0)
        {
            duration = Duration;

            component.Heal(regenValue);
        }
        
        if (emmitTime < 0)
            return null;

        return this;
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        var anotherEffect = effect as RegenerationEffect;

        emmitTime = anotherEffect.EmmitTime;
        Duration = anotherEffect.Duration;

        regenValue += anotherEffect.regenValue;

        return this;
    }
}
