using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventoryComponent : InventoryComponent
{
    [Header("LayerMasks")]
    [SerializeField] private LayerMask AmmoLayer;
    [SerializeField] private LayerMask DetailLayer;
    [Header("Settings")]
    [SerializeField] private float OverlapRadius;
    [SerializeField] private Vector3 CentreOffset;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        
        var detailList = InventorySystem.GetListOfInventoryItem(InventoryItemType.Detail);
        // Добавление предмета детали
        if (detailList.Count == 0)
            detailList.Add(new DetailData(0));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Подбор боеприпасов
        TakeUpAmmo();

        // Подбор деталей
        TakeUpDetails();
    }

    /// <summary>
    /// Подбор боеприпасов в радиусе
    /// </summary>
    private void TakeUpAmmo()
    {
        // Подбор боеприпасов
        var colliders = Physics.OverlapSphere(transform.position + CentreOffset, OverlapRadius, AmmoLayer, QueryTriggerInteraction.Collide);

        var ammoList = InventorySystem.GetListOfInventoryItem(InventoryItemType.Ammo);
        for (int i = 0; i < colliders.Length; i++)
        {
            var ammo = colliders[i].GetComponent<Ammo>();
            if (ammo == null)
                continue;

            var ammoData = ammo.GetAmmoData();
            // Если попался пустой боезапас
            if (ammoData.Count == 0)
                continue;

            // Поиск боезапаса с таким же идентификатором
            var inventoryAmmo = ammoList.FirstOrDefault
                (x => x.ItemSendMessage(ammoData.BulletID) == true);

            // Если был найден боезапас с таким же идентификатором
            if (inventoryAmmo != null)
            {
                // Слияние боеприпасов
                var inventoryAmmoData = inventoryAmmo as AmmoData;
                inventoryAmmoData.Merge(ammoData);
            }
            else
            {
                print("Ammo listed");
                InventorySystem.AddItem(ammoData.ItemType, ammoData);
            }

        }
    }

    /// <summary>
    /// Подбор деталей в радиусе
    /// </summary>
    private void TakeUpDetails()
    {
        var colliders = Physics.OverlapSphere(transform.position + CentreOffset, OverlapRadius, DetailLayer, QueryTriggerInteraction.Collide);

        var detailList = InventorySystem.GetListOfInventoryItem(InventoryItemType.Detail);

        for(int i = 0; i < colliders.Length; i++)
        {
            var detail = colliders[i].GetComponent<Detail>();
            if (detail == null)
                continue;

            var detailData = detail.GetDetailData();

            // Совмещение деталей
            if (detailList.Count > 0)
            {
                var inventoryDetail = detailList[0] as DetailData;

                inventoryDetail.Merge(detailData);
            }
            else
            {
                detailList.Add(detailData);
            }
        }
    }


    public void OnDrawGizmosSelected()
    {
        if (enabled == false)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + CentreOffset, OverlapRadius);
    }
}
