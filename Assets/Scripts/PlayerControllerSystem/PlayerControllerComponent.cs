#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

[RequireComponent(typeof(PlayerInventoryComponent))]
public class PlayerControllerComponent : MonoBehaviour
{
    [Header("Sensivity")]
    [SerializeField] private float mouseSensivityX;
    [SerializeField] private float mouseSensivityY;
    [Space]
    // Системы
    [SerializeField] private MoveSystem MoveSystem;
    [Space]
    [SerializeField] private CameraSystem CameraSystem;
    [Space]
    [SerializeField] private HandSystem HandSystem;
    [Space]
    [SerializeField] private SlopeSystem SlopeSystem;
    [Space]
    [Header("Player Skills")]
    [SerializeField] private DashSkill DashSkill;

    // Изменить потом на другую систему
    [SerializeField] private WeaponComponent Weapon;

    public InventoryComponent inventoryComponent { get; private set; }

    private IEnumerator reloadRoutine;
    private bool IsReloading;

    // UI ELEMENTS
    [SerializeField] private HUDWeapon HUDWeapon;


    // Start is called before the first frame update
    void Start()
    {
        CameraSystem.Start();
        HandSystem.Start();
        MoveSystem.Start(GetComponent<Rigidbody>());

        DashSkill.Start();

        inventoryComponent = GetComponent<InventoryComponent>();

        HUDWeapon.SetInventorySystem(inventoryComponent.InventorySystem);
        HUDWeapon.SetWeapon(Weapon);

    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        
        // Slope Input
        bool inputRight = Input.GetKey(KeyCode.E);
        bool inputLeft = Input.GetKey(KeyCode.Q);
        // Move Input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Mouse rotation Input
        float rotationY = Input.GetAxis("Mouse X")*mouseSensivityX; // Вращение по горизонту
        float rotationX = -Input.GetAxis("Mouse Y")*mouseSensivityY; // Вращение по вертикали

        bool inputDash = Input.GetKeyDown(KeyCode.Space); // DashSkill
        
        // Use DashSkill
        #region DashSkill
        if (inputDash)
            DashSkill.Use(horizontalInput, verticalInput);

        var dashOut = DashSkill.Update(Time.deltaTime);

        if (DashSkill.Active)
        {
            horizontalInput = dashOut.horizontalInput;
            verticalInput = dashOut.verticalInput;

            rotationX *= dashOut.rotationXModifier;
            rotationY *= dashOut.rotationYModifier;
        }
        #endregion

        Vector2 AbsRotationXY = new Vector2(Mathf.Abs(rotationX), Mathf.Abs(rotationY));

        // Оружие
        var weaponOutput = new WeaponUpdateOutput
        {
            recoil = 0,
            recoilopacity = 0
        };

        // Если оружие не перезаряжается
        if (!IsReloading)
        {
            bool inputReload = Input.GetKeyDown(KeyCode.R);
            var reloadNow = false;
            if (inputReload)
            {
                reloadNow = ReloadWeapon(inventoryComponent.InventorySystem);
            }

            // Если оружие не перезаряжается
            if (!reloadNow)
            {
                weaponOutput = Weapon.UpdateComponent(new WeaponUpdateInput
                {
                    fire = Input.GetKey(KeyCode.Mouse0)
                });
            }
        }

        // Ходьба
        var moveOutput = MoveSystem.Update(new MoveSystemInput()
        {
            rotationY = rotationY,
            horizontalInput = horizontalInput,
            verticalInput = verticalInput,
            speedModifier = dashOut.speedModifier // dash
            
        });

        // Камера
        var cameraOutput = CameraSystem.Update(new CameraSystemInput()
        {
            rotationX = rotationX,
            weaponRecoil = weaponOutput.recoil,
            weaponRecoilOpacity = weaponOutput.recoilopacity
           
        });

        // Руки
        var handOutput = HandSystem.Update(new HandSystemInput()
        {
            rotationX = rotationX,
            rotationY = rotationY,
            cameraRotation = cameraOutput.rotation,
            weaponRecoil = weaponOutput.recoil
        });

        // Наклоны
        var slopeOutput = SlopeSystem.Update(new SlopeSystemInput()
        {
            inputRight = inputRight,
            inputLeft = inputLeft,
            cameraLocalEulerAnglesX = cameraOutput.localEulerAngles.x
        });
        
        // Hide and lock cursor when right mouse button pressed
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Unlock and show cursor when right mouse button released
        if (Input.GetKeyDown(KeyCode.U))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }


    /// <summary>
    /// Перезарядка текущего оружия
    /// </summary>
    /// <returns>Произошла ли перезарядка (true)</returns>
    public bool ReloadWeapon(InventorySystem inventorySystem)
    {
        var weaponMagazine = Weapon.MagazineComponent;
        int needToReload = weaponMagazine.NeedAmmo;


        // Ищем нужный тип боеприпасов
        var inventoryAmmo = inventorySystem.GetListOfInventoryItem(InventoryItemType.Ammo)
            .FirstOrDefault(x => x.ItemSendMessage(weaponMagazine.BulletID) == true);

        // Если боеприпасы нашлись
        if (inventoryAmmo != null)
        {
            var ammo = inventoryAmmo as AmmoData;

            // Если боезапас в инвентаре есть
            if (ammo.IsEmpty == false)
            {
                int takenAmmo = Mathf.Min(ammo.Count, needToReload);
                ammo.Take(takenAmmo);
                weaponMagazine.Fill(takenAmmo);

                reloadRoutine = ReloadCoroutine();
                StartCoroutine(reloadRoutine);

                
                return true;
            }
        }
        return false;
    }

    private IEnumerator ReloadCoroutine()
    {
        Weapon.StopShooting();
        IsReloading = true;
        Weapon.State = WeaponState.Reloading;
        HUDWeapon.UpdateAmmo();

        yield return new WaitForSeconds(Weapon.ReloadTime);
        Weapon.CheckMagazine();
        IsReloading = false;
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        var relativePos = transform.position+MoveSystem.GetBodyBones().BodyCenterOffset;
        Gizmos.DrawLine(relativePos, relativePos +transform.forward * MoveSystem.GetDampLength());
    }
}
