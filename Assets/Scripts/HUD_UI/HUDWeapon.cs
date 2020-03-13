using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class HUDWeapon : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private TMP_Text AmmoText;
    [SerializeField] private TMP_Text MagazineAmmoText;
    [SerializeField] private TMP_Text WeaponStatusText;
    [Space(10)]
    [Header("Список оружия")]
    [SerializeField] private RectTransform WeaponListParent;
    [SerializeField] private UIWeaponBlock WeaponBlockPrefab;
    [SerializeField] private CanvasGroup WeaponListAlpha;

    /// <summary>
    /// Время до старта затухания
    /// </summary>
    [SerializeField] private float VisibleTime;
    
    /// <summary>
    /// Скорость затухания списка
    /// </summary>
    [SerializeField] private float FadeSpeed;

    /// <summary>
    /// Может ли начаться процесс затухания. 0 - нет, 1 - да
    /// </summary>
    private bool canFade;

    private TimerCallbacker timerCallbacker;


    private StringBuilder AmmoTextBuilder;
    private StringBuilder WeaponStatusTextBuilder;

    private List<UIWeaponBlock> WeaponsList;

    /// <summary>
    /// Индекс текущего выбранного оружия
    /// </summary>
    private int selectedIndex;

    /// <summary>
    /// Держатель оружия игрока
    /// </summary>
    private WeaponHolder WeaponHolder;

    /// <summary>
    /// Текущее оружие игрока
    /// </summary>
    private WeaponComponent Weapon;

    /// <summary>
    /// Инвентарь игрока
    /// </summary>
    private InventorySystem InventorySystem;

    /// <summary>
    /// Используемые боеприпасы
    /// </summary>
    private AmmoData WeaponUsedAmmo;

    private Action<IObservable> MagazineUnsubscribe;

    private void Awake()
    {
        AmmoTextBuilder = new StringBuilder();
        WeaponStatusTextBuilder = new StringBuilder();

        WeaponsList = new List<UIWeaponBlock>();

        // Настройка затухания
        timerCallbacker = new TimerCallbacker(VisibleTime);
        timerCallbacker.OnEmmitionEndCallback += () =>
        {
            canFade = true;
        };

        timerCallbacker.OnResetCallback += () =>
        {
            WeaponListAlpha.alpha = 1;
            canFade = false;
        };
        

        MagazineUnsubscribe = (observable) =>
        {
            Weapon.MagazineComponent.OnCountChanged -= OnMagazineAmmoCountChanged;
        };

        // Присваивание в Awake, т.к. при старте вызывается первое событие
        UIController.Instance.OnPlayerChanged += ChangePlayerController;

        UIController.Instance.OnPlayerNull += PlayerControllerNull;
    }

    private void Update()
    {
        if (canFade == true)
        {
            WeaponListAlpha.alpha -= FadeSpeed * Time.deltaTime;

            if(WeaponListAlpha.alpha == 0)
            {
                canFade = false;
            }
        }
        else
        {
            timerCallbacker.Update(Time.deltaTime);
        }
        
    }

    /// <summary>
    /// Установка оружия для отрисовки в интерфейсе
    /// </summary>
    /// <param name="weapon">Текущее оружие в руках</param>
    public void SetWeapon(WeaponComponent weapon)
    {
        // Если новое оружие пустое
        if(weapon == null)
        {
            // Прошлое оружие не пустое
            if(Weapon != null)
            {
                UnsubscribeFromWeapon();

                Weapon = weapon;
            }
        }
        else
        {
            // Прошлое оружие пустое
            if (Weapon == null)
            {
                Weapon = weapon;
                
            }
            else
            {
                UnsubscribeFromWeapon();

                Weapon = weapon;
            }
        }

        SubscribeToWeapon();
    }

    private void SubscribeToWeapon()
    {
        Weapon.MagazineComponent.OnCountChanged += OnMagazineAmmoCountChanged;
        OnMagazineAmmoCountChanged(Weapon.MagazineComponent.Count);

        Weapon.MagazineComponent.OnForceUnsubcribe += MagazineUnsubscribe;

        Weapon.OnStateChanged += DrawWeaponStatusText;
        DrawWeaponStatusText(Weapon.State);

        // Установка WeaponUsedAmmo
        SetAmmo();
    }

    private void UnsubscribeFromWeapon()
    {
        Weapon.MagazineComponent.OnCountChanged -= OnMagazineAmmoCountChanged;
        Weapon.MagazineComponent.OnForceUnsubcribe -= MagazineUnsubscribe;

        Weapon.OnStateChanged -= DrawWeaponStatusText;

        if (WeaponUsedAmmo != null)
        {
            WeaponUsedAmmo.OnForceUnsubcribe -= OnAmmoDispose;
            WeaponUsedAmmo.OnCountChanged -= OnAmmoCountChanged;

            WeaponUsedAmmo = null;
        }
    }

    /// <summary>
    /// Срабатывает при добавлении нового типа боеприпаса в инвентарь
    /// </summary>
    /// <param name="ammo">Добавленный тип боеприпаса</param>
    public void OnInventoryAmmoAdd(IInventoryItem ammo)
    {
        if (Weapon == null)
            return;

        // Проерка на нужный тип боеприпаса
        // Если у нас уже есть ссылка на нужный боеприпас, то ничего делать не нужно
        if(WeaponUsedAmmo == null)
        {
            var ammodata = ammo as AmmoData;

            if(ammodata.BulletID == Weapon.MagazineComponent.BulletID)
            {
                WeaponUsedAmmo = ammodata;

                WeaponUsedAmmo.OnForceUnsubcribe += OnAmmoDispose;
                WeaponUsedAmmo.OnCountChanged += OnAmmoCountChanged;
            }
        }
    }

    /// <summary>
    /// Срабатывает при уничтожении используемого боеприпаса
    /// </summary>
    public void OnAmmoDispose(IObservable obesrable)
    {
        WeaponUsedAmmo = null;
    }

    public void OnAmmoCountChanged(int count)
    {
        AmmoText.text = count.ToString();
    }

    public void OnMagazineAmmoCountChanged(int count)
    {
        MagazineAmmoText.text = count.ToString();
    }
    
    private void SetAmmo()
    {
        // Ищем нужный тип боеприпасов
        var inventoryAmmo = InventorySystem.GetListOfInventoryItem(InventoryItemType.Ammo)
            .FirstOrDefault(x => x.ItemSendMessage(Weapon.MagazineComponent.BulletID) == true);

        // Если боеприпасы нашлись
        if (inventoryAmmo != null)
        {
            var ammodata = inventoryAmmo as AmmoData;

            WeaponUsedAmmo = ammodata;

            WeaponUsedAmmo.OnForceUnsubcribe += OnAmmoDispose;
            WeaponUsedAmmo.OnCountChanged += OnAmmoCountChanged;
            OnAmmoCountChanged(WeaponUsedAmmo.Count);
        }
        else
        {
            OnAmmoCountChanged(0);
        }
    }

    
    /// <summary>
    /// Отрисовка состояния оружия
    /// </summary>
    private void DrawWeaponStatusText(WeaponState weaponState)
    {
        var state = Weapon.State;

        if (state == WeaponState.Shooting)
            state = WeaponState.Done;
        WeaponStatusText.text = state.ToString();
    }

    private void SelectWeaponFromList(WeaponComponent selectedWeapon, WeaponInfo info)
    {
        WeaponsList[selectedIndex].Deselect();

        selectedIndex = info.IndexInList;

        var coincidence = WeaponsList[selectedIndex];

        coincidence.Select();

        // Отображение списка оружия
        ShowWeaponList();
    }

    private void CreateWeaponBlock(WeaponComponent newWeapon, WeaponInfo info)
    {
        var instance = Instantiate(WeaponBlockPrefab, WeaponListParent, false);
        
        instance.WeaponName.text = newWeapon.Name;
        instance.BlockNumber.text = (info.IndexInList + 1).ToString();
        instance.WeaponIcon.sprite = GameManager.Instance.WeaponAssetManager.GetWeaponBundleByID(newWeapon.ID).weaponSprite;

        WeaponsList.Insert(info.IndexInList,instance);
    }

    private void DeleteWeaponBlock(WeaponComponent deletedWeapon)
    {
        // Находим блок оружия с таким же именем
        var coincidence = WeaponsList.FirstOrDefault(x => x.WeaponName.text == deletedWeapon.Name);

        if(coincidence != null)
        {
            WeaponsList.Remove(coincidence);
            Destroy(coincidence.gameObject);
        }
    }

    /// <summary>
    /// Показывает список оружия
    /// </summary>
    private void ShowWeaponList()
    {
        timerCallbacker.Reset();
    }

    /// <summary>
    /// Показывает список оружия. Используется в событиях с аргументом
    /// </summary>
    /// <param name="weaponComponent">Аргумент события</param>
    private void ShowWeaponList(WeaponComponent weaponComponent)
    {
        timerCallbacker.Reset();
    }

    public void Unsubscribe(IObservable observable)
    {
        var pc = observable as PlayerControllerComponent;

        UnsubscribeFromWeapon();
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        var inventory = playerControllerComponent.InventoryComponent.InventorySystem;
        var weaponHolder = playerControllerComponent.WeaponHolder;

        inventory.OnAmmoAdded += OnInventoryAmmoAdd;
        weaponHolder.OnWeaponChanged += SetWeapon;
        weaponHolder.OnWeaponChangedExtended += SelectWeaponFromList;

        weaponHolder.OnWeaponAddedExtended += CreateWeaponBlock;
        weaponHolder.OnWeaponAdded += ShowWeaponList;

        weaponHolder.OnWeaponRemoved += DeleteWeaponBlock;

        weaponHolder.OnWeaponChangeInitiated += ShowWeaponList;

        // Получаем текущее оружие и отрисовываем 
        var weapon = weaponHolder.GetCurrentWeapon();
        if(weapon != null)
        {
            SetWeapon(weapon);
        }

        WeaponHolder = weaponHolder;
        InventorySystem = inventory;
    }

    public void PlayerControllerNull()
    {
        throw new System.NotImplementedException();
    }
}
