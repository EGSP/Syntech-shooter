using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponComponent))]
public class WeaponSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSource;

    private WeaponComponent weaponComponent;

    private void Awake()
    {
        if (AudioSource == null)
            AudioSource = GetComponent<AudioSource>();

        weaponComponent = GetComponent<WeaponComponent>();
        if(weaponComponent != null)
        {
            weaponComponent.OnFireStarted += PlaySound;
        }
    }

    /// <summary>
    /// Проигрывает звук при вызове
    /// </summary>
    private void PlaySound()
    {
        AudioSource.Play();
    }
}
