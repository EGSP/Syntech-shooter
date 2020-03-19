public interface IDamageTakePerk
{
    /// <summary>
    /// Приоритет перка
    /// </summary>
    int DamagePerkPriority { get; }

    /// <summary>
    /// Изменение урона перком
    /// </summary>
    /// <param name="data">Полученный урон</param>
    void PerkDamage(DamageData data);
}