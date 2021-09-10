using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitativeWeaponBundleLoader : MonoBehaviour
{
    public List<ImitativeWeaponBundle> imitativeWeaponBundles;
    // Start is called before the first frame update

    private void Awake()
    {
        foreach (var a in imitativeWeaponBundles)
        {
            a.Initialize();
            GameManager.Instance.WeaponAssetManager.AddWeaponBundle(a);
        }
    }
}
