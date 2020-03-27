#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using AIB.AIBehaviours;

[RequireComponent(typeof(PlayerInventoryComponent))]
[RequireComponent(typeof(PlayerLifeComponent))]
[RequireComponent(typeof(WeaponHolder))]
public class PlayerControllerComponent : MonoBehaviour, IObservable
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
    [SerializeField] private DashSkill _DashSkill;
    

    [Header("Weapons")]
   
    // Кнопки смены оружия. Позиция в массиве влияет на индекс оружия
    [SerializeField] private KeyCode[] ChangeWeaponKeys;
    // Время смены оружия
    [SerializeField] private float WeaponChangeTime;


    /// <summary>
    /// Инвентарь игрока
    /// </summary>
    public InventoryComponent InventoryComponent { get; private set; }

    /// <summary>
    /// Здоровье игрока
    /// </summary>
    public PlayerLifeComponent PlayerLifeComponent { get; private set; }

    /// <summary>
    /// Оружие игрока
    /// </summary>
    public WeaponHolder WeaponHolder { get; private set; }

    /// <summary>
    /// Подконтрольный робот
    /// </summary>
    public SignalAIBehaviour SignalAI { get; private set; }

    /// <summary>
    /// Вызывается при изменении робота. Не передает null
    /// </summary>
    public Action<SignalAIBehaviour> OnSignalAIChanged = delegate { };

    /// <summary>
    /// Вызывается при убирании текущего робота
    /// </summary>
    public Action OnSignalAIRemoved = delegate { };

    /// <summary>
    /// Текущее оружие
    /// </summary>
    private WeaponComponent CurrentWeapon;

    /// <summary>
    /// Включен ли контроллер
    /// </summary>
    private bool IsControlActive;

    private IEnumerator reloadRoutine;
    private bool IsReloading;

    private IEnumerator weaponChangeRoutine;
    private bool IsWeaponChanging;

    public event Action<IObservable> OnForceUnsubcribe = delegate { };

    #region InputCaching
    // Slope Input
    private bool inputRight { get; set; }
    private bool inputLeft { get; set; }

    // Move Input
    private float horizontalInput { get; set; }
    private float verticalInput { get; set; }

    // Mouse rotation Input
    private float rotationY { get; set; }
    private float rotationX { get; set; }

    // Dash Input
    private bool inputDash { get; set; }

    // Fire Input
    private bool inputFire { get; set; }

    public DashSkill DashSkill { get => _DashSkill; set => _DashSkill = value; }

    #endregion

    void Awake()
    {
        InventoryComponent = GetComponent<InventoryComponent>();
        PlayerLifeComponent = GetComponent<PlayerLifeComponent>();
        WeaponHolder = GetComponent<WeaponHolder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ActivateControll();

        CameraSystem.Start();
        HandSystem.Start();
        MoveSystem.Start(GetComponent<Rigidbody>());

        DashSkill.Start();

        WeaponHolder.OnWeaponChanged += ChangeWeapon;
        
    }

    // Здесь обновляется ввод
    void LateUpdate()
    {

        

        // Hide and lock cursor when right mouse button pressed
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Unlock and show cursor when right mouse button released
        if (Input.GetKeyDown(KeyCode.U))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Здесь обновляются действия
    void Update()
    {
        // Slope Input
        inputRight = Input.GetKeyDown(KeyCode.E);
        inputLeft = Input.GetKeyDown(KeyCode.Q);

        // Move Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Mouse rotation Input
        rotationY = Input.GetAxis("Mouse X") * mouseSensivityX; // Вращение по горизонтали
        rotationX = -Input.GetAxis("Mouse Y") * mouseSensivityY; // Вращение по вертикали

        // DashSkill
        inputDash = Input.GetKeyDown(KeyCode.Space);

        // Fire Input
        inputFire = Input.GetKey(KeyCode.Mouse0);

        if (IsControlActive == false)
            return;

        float deltaTime = Time.deltaTime;
        
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

        // Если оружие не перезаряжается и не меняется
        if (!IsReloading && !IsWeaponChanging)
        {
            // После этого цикла IsWeaponChanging может стать true
            for(var i = 0; i < ChangeWeaponKeys.Length; i++)
            {
                // Если игрок решил сменить оружие
                if (Input.GetKeyDown(ChangeWeaponKeys[i]))
                {
                    bool changed;
                    WeaponHolder.ChangeWeapon(i, out changed);

                    break;
                }
            }

            // Если оружие не меняется
            if (!IsWeaponChanging)
            {
                // Если оружие есть в руках
                if (CurrentWeapon != null)
                {
                    bool inputReload = Input.GetKeyDown(KeyCode.R);
                    var reloadNow = false;

                    // Если нажата кнопка перезарядки
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

    /// <summary>
    /// Смена оружия в контроллере
    /// </summary>
    /// <param name="newWeapon">Новое оружие</param>
    public void ChangeWeapon(WeaponComponent newWeapon)
    {
        if (IsWeaponChanging == true)
            return;

        if (CurrentWeapon != null)
            CurrentWeapon.gameObject.SetActive(false);

        CurrentWeapon = newWeapon;

        CurrentWeapon.gameObject.SetActive(true);

        if (newWeapon != null)
        {
            weaponChangeRoutine = ChangeWeaponCoroutine();
            StartCoroutine(weaponChangeRoutine);
        }
    }

    private IEnumerator ChangeWeaponCoroutine()
    {
        IsWeaponChanging = true;
        CurrentWeapon.StopShooting();
       
        yield return new WaitForSeconds(WeaponChangeTime);

        var settings = CurrentWeapon.GetComponent<WeaponSettingsComponent>();
        CurrentWeapon.gameObject.transform.parent = HandSystem.WeaponParent;

        CurrentWeapon.SetDefaultLocalPosition(settings.DefaultPosition);
        CurrentWeapon.SetDefaultLocalRotation(Quaternion.Euler(settings.DefaultRotation));
        CurrentWeapon.transform.localPosition = settings.DefaultPosition;
        CurrentWeapon.transform.localEulerAngles = settings.DefaultRotation;

        IsWeaponChanging = false;
    }

    

    /// <summary>
    /// Включение управления 
    /// </summary>
    public void ActivateControll()
    {
        IsControlActive = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

    
    /// <summary>
    /// Добавление робота. Не использовать null
    /// </summary>
    /// <param name="signalAI"></param>
    public void SetSignalAI(SignalAIBehaviour signalAI)
    {
        SignalAI = signalAI;

        OnSignalAIChanged(SignalAI);
    }

    /// <summary>
    /// Удаляет робота и передает его по ссылке
    /// </summary>
    public SignalAIBehaviour RemoveSignalAI()
    {
        var signalAI = SignalAI;

        SignalAI = null;

        OnSignalAIRemoved();

        return signalAI;
    }
}

public interface IObservable
{
    /// <summary>
    /// Событие при котором все подписчики должны отписаться
    /// </summary>
    event Action<IObservable> OnForceUnsubcribe;
}