using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResourceSystems;

using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Singleton Exception");

        Instance = this;

        ConfigManager.Setup(new ConfigSetup()
        {
            ExecutionRoot = false
        });
        
        WorldSettings.Setup();

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

