using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGripComponent : MonoBehaviour
{
    public WeaponGripPreset Preset;
    // Start is called before the first frame update
    public void Awake()
    {
        RecoilForceFactor = Preset.RecoilForceFactor;
    }
    

    public float RecoilForceFactor { get; private set; }
}
