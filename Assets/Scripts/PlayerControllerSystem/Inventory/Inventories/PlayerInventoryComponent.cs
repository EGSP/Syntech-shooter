using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventoryComponent : InventoryComponent
{
    public LayerMask AmmoLayer;
    public float OverlapRadius;
    public Vector3 CentreOffset;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

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

            // Если не был найден боезапас с таким же идентификатором
            if(inventoryAmmo != null)
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


    public void OnDrawGizmosSelected()
    {
        if (enabled == false)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + CentreOffset, OverlapRadius);
    }
}
