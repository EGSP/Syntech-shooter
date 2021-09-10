using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

using AIB.AIBehaviours;

/// <summary>
/// Этот компонент вешается на любой элемент окна. 
/// Родительский объект его сам найдет через GetComponentsChildren[]
/// </summary>
public class RobotBlock : MonoBehaviour, IPlayerObserver
{
    /// <summary>
    /// Кнопка которая отвечает за установку робота
    /// </summary>
    [SerializeField] private EventElement AcceptButton;

    /// <summary>
    /// Робот которого игрок может получить
    /// </summary>
    [SerializeField] private SignalAIBehaviour robotPrefab; 
    public SignalAIBehaviour RobotPrefab { get => robotPrefab; private set => robotPrefab = value; }

    /// <summary>
    /// Компонент игрока получаемый из вне
    /// </summary>
    private PlayerControllerComponent PlayerControllerComponent { get; set; }

    /// <summary>
    /// Свободное пространство для спавна роботов
    /// </summary>
    public Transform SpaceTransform { get; set; }

    void Awake()
    {
        AcceptButton.OnPointerClickEvent += OnAccept;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Вызывается при нажатии на кнопку установки
    /// </summary>
    /// <param name="eventData"></param>
    private void OnAccept(PointerEventData eventData)
    {
        if (PlayerControllerComponent.SignalAI != null)
        {
            var robot = Instantiate(RobotPrefab);
            robot.gameObject.SetActive(true);

            // Установка позиции и включение
            robot.transform.position = SpaceTransform.position;
            robot.Enable();

            // Передача сигнала
            PlayerControllerComponent.SetSignalAI(robot);

            robot.SendCreator(PlayerControllerComponent.gameObject);
        }
    }

    public void ChangePlayerController(PlayerControllerComponent playerControllerComponent)
    {
        PlayerControllerComponent = playerControllerComponent;
    }

    public void PlayerControllerNull()
    {
        PlayerControllerComponent = null;
    }

    public void Unsubscribe(IObservable observable)
    {
        throw new System.NotImplementedException();
    }
}
