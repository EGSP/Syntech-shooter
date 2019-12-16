#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : EffectObject
{
    [Tooltip("Системы частиц, запускаемые при активации, автоматически заполняется при отсутствии элементов")]
    [SerializeField] private ParticleSystem[] Particles;


    // Инициализация массива
    public override void Initialize()
    {
        if(Particles.Length == 0)
        {
            Particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        }

        if (Particles == null)
        {
            Particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        }
        
    }

    // Проигрывает эффект
    public override void PlayEffect()
    {
        for(int i = 0; i < Particles.Length;i++)
        {
            Particles[i].Play();
        }
    }

    // Переигрывает эффект
    public override void ResetEffect()
    {
        for (int i = 0; i < Particles.Length; i++)
        {
            Particles[i].Stop();
            Particles[i].Play();
        }
    }

    // Останавливает эффект
    public override void StopEffect()
    {
        for (int i = 0; i < Particles.Length; i++)
        {
            Particles[i].Stop();
        }
    }
}
