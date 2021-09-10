using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class DamageData
{
    /// <summary>
    /// Базовый урон ни на что не влияющий
    /// </summary>
    public float baseDamage;

    /// <summary>
    /// Коэфицент урона наносимого броне (1 == 100%), умножается на базовый урон
    /// </summary>
    public float armourModifier;

    /// <summary>
    /// Прохождение активной брони насквозь
    /// </summary>
    public bool armourPenetration;
}

public enum DamageType
{
    Base,
    Fire,
    Plasma,
    Shock,
    Acid
}