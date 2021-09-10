using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMedkit : Medkit
{
    [SerializeField] private float HealValue;

    public override LifeComponentEffect Use()
    {
        if (Used)
            return new NullLifeComponentEffect();

        Used = true;
        return new SingleHealEffect(HealValue);
    }

    public void Update()
    {
        if (Used)
        {
            Destroy();
        }
    }
}
