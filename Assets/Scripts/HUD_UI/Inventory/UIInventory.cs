using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIInventory : MonoBehaviour, IPlayerObserver
{
    public event UIController.CallHandler OnAmmoReset = delegate { };

    [Header("InventoryMain")]
    [SerializeField] private GameObject InventoryCanvas;
    [SerializeField] private bool DeactivatePlayerControll;

    [Header("Module")]
    [SerializeField] private Image ModulePrefab;
    /// <summary>
    /// Количество создаваемых элементов отображения
    /// </summary>
    [SerializeField] private int ModulesCount;
    /// <summary>
    /// Родительский объект, которому будут принадлежать созданные элементы отображения
    /// </summary>
    [SerializeField] private RectTransform ModulesParent;

    private List<Image> ModulesList;


    [Space][Header("Ammo")]
    [SerializeField] private UIAmmoElement AmmoPrefab;
    /// <summary>
    /// Количество создаваемых элементов отображения
    /// </summary>
    [SerializeField] private int AmmoCount;
    /// <summary>
    /// Родительский объект, которому будут принадлежать созданные элементы отображения
    /// </summary>
    [SerializeField] private RectTransform AmmoParent;

    private List<UIAmmoElement> AmmoList;

    /// <summary>
    /// Инвентарь игрока
    /// </summary>
    private InventorySystem PlayerInventory;
    private PlayerControllerComponent PlayerController;

    /// <summary>
    /// Открыто ли сейчас окно инвентаря
    /// </summary>
    public bool IsOpened { get; private set; }

    private void Awake()
    {
        // Присваивание в Awake, т.к. при старте вызывается первое событие
        UIController.Instance.OnPlayerChanged += ChangePlayerController;

        UIController.Instance.OnPlayerNull += PlayerControllerNull;
        
        UIController.Instance.OnDisable += OnDisableCustom;
        UIController.Instance.OnUpdate += UpdateInput;
    }

    private void Start()
    {
        InstantiateGrid();

    }

    private void UpdateInput(float deltaTime)
    {

    }

    public void InstantiateGrid()
    {
        if (ModulePrefab == null)
            throw new System.Exception("No ModulePrefab in UIInventory");

        if (AmmoPrefab == null)
            throw new System.Exception("No AmmoPrefab in UIInventory");

        
        ModulesList = new List<Image>(ModulesCount);
        AmmoList = new List<UIAmmoElement>(AmmoCount);

        // Создание элементов отображения модулей
        for(var i = 0; i < ModulesCount; i++)
        {
            var instance = Instantiate(ModulePrefab);
            instance.transform.SetParent(ModulesParent, false);

            ModulesList.Add(instance);
        }

        // Создание элементов отображения боеприпасов
        for (var i = 0; i < ModulesCount; i++)
        {
            var instance = Instantiate(AmmoPrefab);
            instance.transform.SetParent(AmmoParent, false);

            // Подписка на обновление информации
            OnAmmoReset += instance.ResetToDefault;

            AmmoList.Add(instance);
        }
    }

    public void OnDisableCustom(int priority)
    {
        // Приоритет не имеет значения
        Close();
    }

    public void Open(PlayerControllerComponent playerController)
    {
        IsOpened = true;
        InventoryCanvas.SetActive(true);

        PlayerController = playerController;

        if (DeactivatePlayerControll == true)
            PlayerController.DeactivateControll();
        PlayerInventory = PlayerController.InventoryComponent.InventorySystem;

        DrawInventoryModules();
        DrawInventoryAmmo();
    }

    public void Close()
    {
        IsOpened = false;
        InventoryCanvas.SetActive(false);

        PlayerInventory = null;
    }

    /// <summary>
    /// Отрисовка модулей
    /// </summary>
    public void DrawInventoryModules()
    {
        var modules = PlayerInventory.GetListOfInventoryItem(InventoryItemType.WeaponModules);

        for(var i = 0; i < modules.Count; i++)
        {

        }
    }

    /// <summary>
    /// Отрисовка боеприпасов
    /// </summary>
    public void DrawInventoryAmmo()
    {
        var resManager = ResourceManager.Instance;
        var ammo = PlayerInventory.GetListOfInventoryItem(InventoryItemType.Ammo);

        OnAmmoReset();
        for (var i = 0; i < ammo.Count; i++)
        {
            var ammoData = ammo[i] as AmmoData;
            var uiAmmo = AmmoList[i];

            uiAmmo.SetData(ammoData);
            uiAmmo.SetImage(resManager.GetAmmoSprite(ammoData.BulletID) as Sprite);
        }
    }

    public void Unsubscribe(IObservable observable)
    {
        PlayerController = null;
        Close();
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        PlayerController = playerControllerComponent;
    }

    public void PlayerControllerNull()
    {
        PlayerController = null;
        Close();
    }
}
