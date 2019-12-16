#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffectComponent : MonoBehaviour
{
    [SerializeField] private TrailRenderer TrailRenderer;
    // Скорость нагрева пули
    [Range(0,1)]
    [SerializeField] private float HeatSpeed;
    [SerializeField] private AnimationCurve Curve;
    private Material material;

    // Текущий нагрев
    private float heat;


    private void Start()
    {
        material = TrailRenderer.material;
    }
    
    public void UpdateEffect(float deltaTime)
    {
        heat += HeatSpeed * deltaTime;
        if (heat >= 1)
            heat = 1;

        material.SetFloat("_Lifetime",Curve.Evaluate(heat));
    }

    public void Cool()
    {
        TrailRenderer.Clear();
        heat = 0;
    }
}
