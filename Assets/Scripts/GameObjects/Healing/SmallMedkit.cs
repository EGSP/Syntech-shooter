using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallMedkit : Medkit
{
    [SerializeField] private float EmmitionTime;
    [Range(0.1f,5)]
    [SerializeField] private float Duration;
    [SerializeField] private float RegenerationValue;

    public override LifeComponentEffect Use()
    {
        if (Used)
            return new NullLifeComponentEffect();

        Used = true;
        return new RegenerationEffect(EmmitionTime, Duration, RegenerationValue);
    }

    public void Update()
    {
        if (Used)
        {
            Destroy();
        }
    }
}
