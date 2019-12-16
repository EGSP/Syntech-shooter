
/// <summary>
/// Эффект разового лечения 
/// </summary>
[System.Serializable]
public class SingleHealEffect : LifeComponentEffect
{
    public SingleHealEffect(float _healValue)
    {
        ID = 0;

        if (_healValue < 0)
            _healValue = 0;

        healValue = _healValue;
    }

    private float healValue;

    public override LifeComponentEffect Invoke(LifeComponent component, float deltaTime)
    {
        component.Heal(healValue);

        return null;
    }

    public override LifeComponentEffect Merge(LifeComponentEffect effect)
    {
        var anotherEffect = effect as SingleHealEffect;

        healValue += anotherEffect.healValue;

        return this;
    }
}
