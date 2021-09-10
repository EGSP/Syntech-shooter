using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CraftTableWindow : MonoBehaviour
{
    /// <summary>
    /// Крафтовый стол к которому принадлежит данное окно
    /// </summary>
    public CraftTable CraftTable { get; private set; }
    
    /// <summary>
    /// Корневой канвас окна
    /// </summary>
    [SerializeField] protected Canvas MainCanvas;

    /// <summary>
    /// Позиция камеры в активном сосотоянии
    /// </summary>
    [SerializeField] protected Transform cameraPosition;
    public Transform CameraPosition { get => cameraPosition; private set => cameraPosition = value; }

    /// <summary>
    /// Контроллер игрока переданный окну
    /// </summary>
    public PlayerControllerComponent PlayerControllerComponent { get; protected set; }
    
    /// <summary>
    /// Объекты, которым нужен игрок. Они устанавливаются в Awake
    /// </summary>
    protected List<IPlayerObserver> PlayerObservers { get; set; }

    protected virtual void Awake()
    {
        PlayerObservers = new List<IPlayerObserver>();
        // Получение списка обсерверов в дочерних объектах
        GetComponentsInChildren<IPlayerObserver>(includeInactive: true, result: PlayerObservers);

        if (MainCanvas == null)
            MainCanvas = GetComponent<Canvas>();
    }

    protected virtual void Start()
    {

    }

    /// <summary>
    /// Инициализация окна
    /// </summary>
    public virtual void Initialize()
    {

    }

    /// <summary>
    /// Открытие окна
    /// </summary>
    /// <param name="playerControllerComponent">Компонент управления игрока</param>
    public virtual void Open(PlayerControllerComponent playerControllerComponent)
    {
        PlayerControllerComponent = playerControllerComponent;
        MainCanvas.gameObject.SetActive(true);
        
        // Передаем полученный контроллер
        PlayerObservers.ForEach(x => x.ChangePlayerController(PlayerControllerComponent));
    }

    /// <summary>
    /// Закрытие окна
    /// </summary>
    public virtual void Close()
    {
        PlayerControllerComponent = null;
        MainCanvas.gameObject.SetActive(false);

        // Передаем, что контроллера больше нет
        PlayerObservers.ForEach(x => x.PlayerControllerNull());
    }

    public void SetCraftTable(CraftTable craftTable)
    {
        CraftTable = craftTable;

        OnSetCraftTable();
    }

    protected abstract void OnSetCraftTable();
}
