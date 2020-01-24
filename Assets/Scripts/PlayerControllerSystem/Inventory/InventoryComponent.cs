using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    // Инвентарь компонента
    public InventorySystem InventorySystem { get; protected set; }
    

    protected virtual void Awake()
    {
        InventorySystem = new InventorySystem();
    }

    protected virtual void Update()
    {

    }
}
