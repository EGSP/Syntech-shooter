using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitativeCompanionBundleLoader : MonoBehaviour
{
    [SerializeField] private List<ImitativeCompanionBundle> imitativeCompanionBundles;

    private void Awake()
    {
        foreach (var a in imitativeCompanionBundles)
        {
            a.Initialize();
            GameManager.Instance.CompanionAssetManager.AddCompanionBundle(a);
        }
    }
}
