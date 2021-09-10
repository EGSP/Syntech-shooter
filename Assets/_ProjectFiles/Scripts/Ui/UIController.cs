using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

using MUI;

public class UIController : MonoBehaviour
{
    public delegate void CallHandler();

    public static UIController Instance;
    public void Awake()
    {
        if (Instance != null)
            throw new System.Exception("UIController singleton Exception");

        Instance = this;

        if (Notificator == null)
            throw new System.NullReferenceException();

    }

    [SerializeField] private UI_Notificator Notificator;

    [SerializeField] private PlayerControllerComponent playerController;
    public PlayerControllerComponent PlayerController
    {
        get => playerController;
        set
        {
            if (value == null)
            {
                OnPlayerNull();
                playerController = null;
                return;
            }

            playerController = value;
            OnPlayerChanged(playerController);
        }
    }

    /// <summary>
    /// Вызывается при смене контроллера игрока
    /// </summary>
    public event Action<PlayerControllerComponent> OnPlayerChanged = delegate { };

    /// <summary>
    /// Вызывается при нулевом компоненте игрока
    /// </summary>
    public event Action OnPlayerNull = delegate { };

    /// <summary>
    /// Вызывается при обновлении кадра
    /// </summary>
    public event Action<float> OnUpdate = delegate { };

    /// <summary>
    /// Вызывается при открытии более важного окна. Передает значение приоритета
    /// </summary>
    public event Action<int> OnDisable = delegate { };

    /// <summary>
    /// Вызывается при закрытии более важного окна. int - приоритет более важного окна
    /// </summary>
    public event Action<int> OnEnable = delegate { };
    

    /// <summary>
    /// Активен ли на данный момент крафтовый стол
    /// </summary>
    public bool isCraftTableActive { get; private set; }
    


    // Start is called before the first frame update
    void Start()
    {
        OnPlayerChanged(playerController);
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    /// <summary>
    /// Открытие крафтового стола
    /// </summary>
    /// <param name="craftTable">Крафтовый стол</param>
    public void OpenCraftTable(int priority)
    {
        OnDisable(priority);
        isCraftTableActive = true;
    }

    public void CloseCraftTable(int priority)
    {
        OnEnable(priority);
        isCraftTableActive = false;
    }

    /// <summary>
    /// Открытие инвентаря
    /// </summary>
    public void OpenInventory(UIInventory inventory)
    {
        if (inventory.IsOpened == false && isCraftTableActive == false)
            inventory.Open(PlayerController);
    }

    /// <summary>
    /// Закрытие инвентаря
    /// </summary>
    public void CloseInventory(UIInventory inventory)
    {
        if (inventory.IsOpened == true)
            inventory.Close();
    }

    /// <summary>
    /// Показывает уведомления
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="messageType">Тип сообщение</param>
    public void ShowNotify(string message, MessageType messageType = MessageType.Information)
    {
        Notificator.ShowNotify(message, messageType);
    }

    ///// <summary>
    ///// Добавление группы в список UIManager. Возвращает группу из коллекции
    ///// </summary>
    ///// <param name="group"></param>
    //public InterfaceGroup AddGroup(InterfaceGroup group)
    //{
    //    if(Groups.ContainsKey(group.ID) == false)
    //    {
    //        Groups.Add(group.ID, group);

    //        return group;
    //    }

    //    return Groups[group.ID];
    //}

    ///// <summary>
    ///// Добавление элемента в группу
    ///// </summary>
    ///// <param name="ID">Идентификатор группы</param>
    ///// <param name="element">Элемент который нужно добавить</param>
    //public void SubscribeToGroup(string ID,IInterfaceElement element)
    //{
    //    if (Groups.ContainsKey(ID))
    //    {
    //        Groups[ID].AddElement(element);
    //    }
    //    else
    //    {
    //        AddGroup(new InterfaceGroup(ID)).AddElement(element);
    //    }
    //}

    ///// <summary>
    ///// Включает текущую группу
    ///// </summary>
    ///// <param name="group"></param>
    ///// <returns></returns>
    //public bool EnableThisGroup(InterfaceGroup group)
    //{
    //    // Если подчиняется приоритету
    //    if (group.PriorityRule)
    //    {
    //        // Разрешено ли другим быть активными
    //        if (!group.AllowAnother)
    //        {
    //            ActiveGroups.ForEach(x => 
    //            {
    //                if (x.MaxPriority < group.MaxPriority)
    //                    x.DisableElements();

    //                ActiveGroups.Remove(x);
    //            });

    //            ActiveGroups.Add(group);
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        // Разрешено ли другим быть активными
    //        if (!group.AllowAnother)
    //        {
    //            ActiveGroups.ForEach(x => x.DisableElements());

    //            ActiveGroups.Clear();

    //            ActiveGroups.Add(group);

    //            return true;
    //        }

    //        ActiveGroups.Add(group);

    //        return true;
    //    }

    //    return false;
    //}
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

public interface IInterfaceElement
{
    /// <summary>
    /// Приоритет элемента. Учитывается при открытии и закрытии
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Включение элемента
    /// </summary>
    void Enable();

    /// <summary>
    /// Выключение элемента
    /// </summary>
    void Disable();
}

//public class InterfaceGroup
//{
//    public InterfaceGroup(string id)
//    {
//        ID = id;

//        Elements = new List<IInterfaceElement>();
//    }

//    /// <summary>
//    /// Идентификатор группы
//    /// </summary>
//    public string ID { get; }

//    /// <summary>
//    /// Элементы группы
//    /// </summary>
//    public List<IInterfaceElement> Elements { get; }

//    /// <summary>
//    /// Максимальный приоритет в элементах в группы
//    /// </summary>
//    public int MaxPriority { get; private set; }

//    /// <summary>
//    /// Подчиняется ли группа правилу приоритета
//    /// </summary>
//    public bool PriorityRule { get; set; }

//    /// <summary>
//    /// Позволено ли другим группам быть включенными
//    /// </summary>
//    public bool AllowAnother { get; set; }



//    /// <summary>
//    /// Добавление элемента группы
//    /// </summary>
//    /// <param name="element"></param>
//    public void AddElement(IInterfaceElement element)
//    {
//        if (element.Priority > MaxPriority)
//            MaxPriority = element.Priority;

//        Elements.Add(element);
//    }

//    /// <summary>
//    /// Включает все элементы
//    /// </summary>
//    public void EnableElements()
//    {
//        for(int i = 0; i < Elements.Count; i++)
//        {
//            Elements[i].Enable();
//        }
//    }

//    /// <summary>
//    /// Отключает все элементы
//    /// </summary>
//    public void DisableElements()
//    {
//        for (int i = 0; i < Elements.Count; i++)
//        {
//            Elements[i].Enable();
//        }
//    }
//}