#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectObjects
{
    public class ParticleEffect : EffectObject
    {
        [Tooltip("Системы частиц, запускаемые при активации, автоматически заполняется при отсутствии элементов")]
        [SerializeField] private ParticleSystem[] Particles;
        
        // Инициализация массива
        public override void Initialize()
        {
            if (Particles.Length == 0)
            {
                Particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            }

            if (Particles == null)
            {
                Particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            }
            IsFree = true;
        }

        // Проигрывает эффект
        public override void PlayEffect()
        {
            IsFree = false;
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].gameObject.SetActive(true);
                Particles[i].Play();
            }
        }

        public override void PlayEffect(Vector3 position, Vector3 normal)
        {
            IsFree = false;
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(normal);
            PlayEffect();
            
        }

        // Переигрывает эффект
        public override void ResetEffect()
        {
            IsFree = false;
            for (int i = 0; i < Particles.Length; i++)
            {
                var particleSystem = Particles[i];
                particleSystem.gameObject.SetActive(false);
                particleSystem.Stop();

                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();
            }
            
        }

        // Останавливает эффект
        public override void StopEffect()
        {
            IsFree = true;
            for (int i = 0; i < Particles.Length; i++)
            {
                var particleSystem = Particles[i];
                Particles[i].gameObject.SetActive(false);
                Particles[i].Stop();
            }
        }
    }
}
