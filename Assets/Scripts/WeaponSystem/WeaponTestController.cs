using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTestController : MonoBehaviour
{
    public WeaponComponent Weapon;

    public WeaponUpdateOutput output;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        output = Weapon.UpdateComponent(new WeaponUpdateInput()
        {
            fire = Input.GetKey(KeyCode.Mouse0),
            reload = Input.GetKey(KeyCode.R)
        });
    }
}
