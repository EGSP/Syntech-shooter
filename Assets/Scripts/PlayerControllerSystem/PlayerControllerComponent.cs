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
    [Header("Systems")]
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


    [Header("Weapons")]
    [SerializeField] private WeaponHolder WeaponHolder;
    // Кнопка поднятия оружия
    [SerializeField] private KeyCode GetUpWeaponKey;
    // Кнопки смены оружия. Позиция в массиве влияет на индекс оружия
    [SerializeField] private KeyCode[] ChangeWeaponKeys;
    // Время смены оружия
    [SerializeField] private float WeaponChangeTime;

    /// <summary>
    /// Текущее оружие
    /// </summary>
    private WeaponComponent CurrentWeapon;

    /// <summary>
    /// Инвентарь игрока
    /// </summary>
    public InventoryComponent InventoryComponent { get; private set; }

    /// <summary>
    /// Включен ли контроллер
    /// </summary>
    private bool IsControlActive;

    private IEnumerator reloadRoutine;
    private bool IsReloading;

    private IEnumerator weaponChangeRoutine;
    private bool IsWeaponChanging;

 

    // Start is called before the first frame update
    void Start()
    {
        ActivateControll();

        CameraSystem.Start();
        HandSystem.Start();
        MoveSystem.Start(GetComponent<Rigidbody>());

        DashSkill.Start();

        InventoryComponent = GetComponent<InventoryComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsControlActive == false)
            return;

        float deltaTime = Time.deltaTime;
        
        // Slope Input
        bool inputRight = Input.GetKeyDown(KeyCode.E);
        bool inputLeft = Input.GetKeyDown(KeyCode.Q);
        // Move Input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Mouse rotation Input
        float rotationY = Input.GetAxis("Mouse X")*mouseSensivityX; // Вращение по горизонту
        float rotationX = -Input.GetAxis("Mouse Y")*mouseSensivityY; // Вращение по вертикали

        // DashSkill
        bool inputDash = Input.GetKeyDown(KeyCode.Space); 
        
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
        if (!IsReloading && !IsWeaponChanging)
        {
            for(var i = 0; i < ChangeWeaponKeys.Length; i++)
            {
                // Если игрок решил сменить оружие
                if (Input.GetKeyDown(ChangeWeaponKeys[i]))
                {
                    bool changed;
                    CurrentWeapon = WeaponHolder.TakeWeapon(i, out changed);

                    // Смена оружия
                    if (changed)
                    {
                        weaponChangeRoutine = WeaponChangeCoroutine();
                        StartCoroutine(weaponChangeRoutine);
                    }

                    break;
                }
            }

            // Если оружие не меняется
            if (IsWeaponChanging == false)
            {
                // Если оружие есть в руках
                if (CurrentWeapon != null)
                {
                    bool inputReload = Input.GetKeyDown(KeyCode.R);
                    var reloadNow = false;
                    if (inputReload)
                    {
                        reloadNow = ReloadWeapon(InventoryComponent.InventorySystem);
                    }

                    // Если оружие не перезаряжается
                    if (!reloadNow)
                    {
                        weaponOutput = CurrentWeapon.UpdateComponent(new WeaponUpdateInput
                        {
                            fire = Input.GetKey(KeyCode.Mouse0)
                        });
                    }
                }
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
        var weaponMagazine = CurrentWeapon.MagazineComponent;
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
        CurrentWeapon.StopShooting();
        IsReloading = true;
        CurrentWeapon.State = WeaponState.Reloading;
        //HUDWeapon.UpdateAmmo();

        yield return new WaitForSeconds(CurrentWeapon.ReloadTime);
        CurrentWeapon.CheckMagazine();
        IsReloading = false;
    }

    private IEnumerator WeaponChangeCoroutine()
    {
        CurrentWeapon.StopShooting();
        IsWeaponChanging = true;
        yield return new WaitForSeconds(WeaponChangeTime);

        IsWeaponChanging = false;
    }

    /// <summary>
    /// Включение управления 
    /// </summary>
    public void ActivateControll()
    {
        IsControlActive = true;
    }

    /// <summary>
    /// Полное выключение управления
    /// </summary>
    public void DeactivateControll()
    {
        IsControlActive = false;
    }

    /// <summary>
    /// Активация камеры
    /// </summary>
    public void ActivateCamera()
    {
        CameraSystem.UnityCamera.enabled = true;
    }

    /// <summary>
    /// Деактивация камеры
    /// </summary>
    public void DeactivateCamera()
    {
        CameraSystem.UnityCamera.enabled = false;
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        var relativePos = transform.position+MoveSystem.GetBodyBones().BodyCenterOffset;
        Gizmos.DrawLine(relativePos, relativePos +transform.forward * MoveSystem.GetDampLength());
    }
}
