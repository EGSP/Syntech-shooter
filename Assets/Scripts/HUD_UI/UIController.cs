using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public delegate void CallHandler();

    public static UIController Instance;
    public void Awake()
    {
        if (Instance != null)
            throw new System.Exception("UIController singleton Exception");

        Instance = this;
    }

    public delegate void PlayerChangedHandler(PlayerControllerComponent playerControllerComponent);
    public event PlayerChangedHandler OnPlayerChanged = delegate { };

    public delegate void PlayerNullHandler();
    public event PlayerNullHandler OnPlayerNull = delegate { };

    public delegate void UIUpdateHandler(float deltaTime);
    public event UIUpdateHandler OnUpdate = delegate { };

    [SerializeField] private PlayerControllerComponent playerController;
    public PlayerControllerComponent PlayerController
    {
        get => playerController;
        set
        {
            if(value == null)
            {
                OnPlayerNull();
                playerController = null;
                return;
            }

            playerController = value;
            OnPlayerChanged(playerController);
        }
    }

    [SerializeField] private UIInventory Inventory;
    [SerializeField] private KeyCode InventoryOpen;

    // Start is called before the first frame update
    void Start()
    {
        OnPlayerChanged(playerController);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InventoryOpen) && Inventory.IsOpened == false) 
        {
            Inventory.Open(PlayerController);
            return;
        }

        if(Input.GetKeyDown(InventoryOpen)&&Inventory.IsOpened == true)
        {
            Inventory.Close();
            return;
        }
    }
}

public interface IPlayerObserver
{
    /// <summary>
    /// Отписка от наблюдения над объектом
    /// </summary>
    /// <param name="observable"></param>
    void Unsubscribe(IObservable observable);

    /// <summary>
    /// Изменение контроллера игрока
    /// </summary>
    /// <param name="playerControllerComponent">Новый контроллер, который не является null</param>
    void ChangePlayerController(PlayerControllerComponent playerControllerComponent);

    /// <summary>
    /// Действия при пустом контроллере игрока
    /// </summary>
    void PlayerControllerNull();
}