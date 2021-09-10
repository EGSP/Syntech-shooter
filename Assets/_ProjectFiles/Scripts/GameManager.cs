using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResourceSystems;

using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private WorldSettings WorldSettings;

    public void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Singleton Exception");

        Instance = this;

        if (WorldSettings == null && (WorldSettings = GetComponent<WorldSettings>()) == null)
            throw new System.Exception("No WorldSettings in Scene");
        

        ConfigManager.Setup(new ConfigSetup()
        {
            ExecutionRoot = false
        });
        

        WeaponAssetManager.Initialize();
        companionAssetManager.Initialize();
        
    }

    [SerializeField] private WeaponAssetManager weaponAssetManager;
    public WeaponAssetManager WeaponAssetManager
    {
        get => weaponAssetManager;
        private set
        {
            weaponAssetManager = value;
        }
    }

    [SerializeField] private CompanionAssetManager companionAssetManager;
    public CompanionAssetManager CompanionAssetManager
    {
        get => companionAssetManager; private set => companionAssetManager = value;
    }

    public LayerMask LifeLayerMask;

    // Слои для просчета физики пробития
    public LayerMask ShootRayMask;
    [Range(0, 5)]
    // Длинна шага пробития
    public float PenetrationStepLength;
    // Коллайдер, используемый для расчета пробитий
    public Collider PenetrationCollider;
}

