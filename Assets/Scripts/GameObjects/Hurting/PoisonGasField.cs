﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGasField : MonoBehaviour
{
    [SerializeField] private float EmmitionTime;
    [Range(0.1f, 5)]
    [SerializeField] private float Duration;
    [SerializeField] private float PoisonValue;


    public void OnTriggerStay(Collider other)
    {
        var player = other.GetComponent<PlayerLifeComponent>();

        if (player != null)
            player.AddEffect(new PoisonGasEffect(EmmitionTime, Duration, PoisonValue));
    }
}
