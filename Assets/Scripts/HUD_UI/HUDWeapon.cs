using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class HUDWeapon : MonoBehaviour
{

    [SerializeField] private WeaponComponent Weapon;

    [SerializeField] private TMP_Text AmmoText;
    [SerializeField] private TMP_Text WeaponStatusText;


    private StringBuilder AmmoTextBuilder;
    private StringBuilder WeaponStatusTextBuilder;

    private InventorySystem InventorySystem;
    private AmmoData WeaponUsedAmmo;

    private void Awake()
    {
        AmmoTextBuilder = new StringBuilder();
        WeaponStatusTextBuilder = new StringBuilder();
    }


    // Update is called once per frame
    public void Update()
    {

        DrawAmmoText();
        DrawWeaponStatusText();
    }

    /// <summary>
    /// Устанавливает текущий инвентарь
    /// </summary>
    public void SetInventorySystem(InventorySystem _InventorySystem)
    {
        InventorySystem = _InventorySystem;
    }

    /// <summary>
    /// Установка оружия для отрисовка в интерфейсе
    /// </summary>
    /// <param name="_Weapon">Текущее оружие в руках</param>
    public void SetWeapon(WeaponComponent _Weapon)
    {
        Weapon = _Weapon;

        SetAmmo();
    }

    
    private void SetAmmo()
    {
        // Ищем нужный тип боеприпасов
        var inventoryAmmo = InventorySystem.GetListOfInventoryItem(InventoryItemType.Ammo)
            .FirstOrDefault(x => x.ItemSendMessage(Weapon.MagazineComponent.BulletID) == true);

        // Если боеприпасы нашлись
        if (inventoryAmmo != null)
        {
            var ammo = inventoryAmmo as AmmoData;

            WeaponUsedAmmo = ammo;
        }
        else
        {
            // Создаём пустышку
            WeaponUsedAmmo = new AmmoData(Weapon.MagazineComponent.BulletID, 0);
        }
    }

    public void UpdateAmmo()
    {
        // Ищем нужный тип боеприпасов
        var inventoryAmmo = InventorySystem.GetListOfInventoryItem(InventoryItemType.Ammo)
            .FirstOrDefault(x => x.ItemSendMessage(Weapon.MagazineComponent.BulletID) == true);

        // Если боеприпасы нашлись
        if (inventoryAmmo != null)
        {
            var ammo = inventoryAmmo as AmmoData;

            WeaponUsedAmmo = ammo;
        }
    }

    /// <summary>
    /// Отрисовка значений боеприпасов оружия
    /// </summary>
    private void DrawAmmoText()
    {
        AmmoTextBuilder.Clear();
        AmmoTextBuilder.Append(Weapon.MagazineComponent.Count);
        AmmoTextBuilder.Append('/');
        AmmoTextBuilder.Append(WeaponUsedAmmo.Count);

        AmmoText.text = AmmoTextBuilder.ToString();
    }

    /// <summary>
    /// Отрисовка состояния оружия
    /// </summary>
    private void DrawWeaponStatusText()
    {
        var state = Weapon.State;

        if (state == WeaponState.Shooting)
            state = WeaponState.Done;
        WeaponStatusText.text = state.ToString();
    }
}
