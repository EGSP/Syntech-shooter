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


    private StringBuilder AmmoTextBuilder;
    private StringBuilder WeaponStatusTextBuilder;
    

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

        MagazineUnsubscribe = (observable) =>
        {
            Weapon.MagazineComponent.OnCountChanged -= OnMagazineAmmoCountChanged;
        };

        // Присваивание в Awake, т.к. при старте вызывается первое событие
        UIController.Instance.OnPlayerChanged += ChangePlayerController;

        UIController.Instance.OnPlayerNull += PlayerControllerNull;
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

                SubscribeToWeapon();
            }
            else
            {
                UnsubscribeFromWeapon();

                Weapon = weapon;
            }
        }

    }

    private void SubscribeToWeapon()
    {
        Weapon.MagazineComponent.OnCountChanged += OnMagazineAmmoCountChanged;
        Weapon.MagazineComponent.OnForceUnsubcribe += MagazineUnsubscribe;

        Weapon.OnStateChanged += DrawWeaponStatusText;

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

        // Получаем текущее оружие и отрисовываем 
        var weapon = weaponHolder.GetCurrentWeapon();
        if(weapon != null)
        {
            SetWeapon(weapon);
        }

        InventorySystem = inventory;
    }

    public void PlayerControllerNull()
    {
        throw new System.NotImplementedException();
    }
}
