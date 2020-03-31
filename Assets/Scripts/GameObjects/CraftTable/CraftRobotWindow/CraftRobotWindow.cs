using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AIB.AIBehaviours;

using System;
using System.Linq;

[RequireComponent(typeof(RobotBuilder))]
public class CraftRobotWindow : CraftTableWindow
{
    /// <summary>
    /// Элемент отрисовки показателей робота
    /// </summary>
    [SerializeField] private RobotStats RobotStats;

    /// <summary>
    /// Элемент отрисовки характерстик робота
    /// </summary>
    [SerializeField] private RobotOverview RobotOverview;

    /// <summary>
    /// Элемент отрисовки списка доступных роботов для создания
    /// </summary>
    [SerializeField] private RobotSelector RobotSelector;

    [Space(5)]
    [SerializeField] private Transform Spawn;

    /// <summary>
    /// Информация по постройке роботов
    /// </summary>
    public RobotBuilder RobotBuilder { get; private set; }

    /// <summary>
    /// Текущий отображаемый робот
    /// </summary>
    private string CurrentRobot;
    
    protected override void Awake()
    {
        base.Awake();

        RobotBuilder = GetComponent<RobotBuilder>();

        RobotBuilder.Initialize();

        RobotSelector.OnRobotChoosed += ChooseRobot;

        RobotOverview.RobotBuildPanel.OnBuildButtonClicked += BuildRobot;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Open(PlayerControllerComponent playerControllerComponent)
    {
        base.Open(playerControllerComponent);

        if (PlayerControllerComponent.SignalAI != null)
        {
            ChooseRobot(PlayerControllerComponent.SignalAI.ID);
        }
        else
        {
            // Выбор стандартного робота
            ChooseRobot();
        }
    }

    /// <summary>
    /// Выбор нового робота и его отображение
    /// </summary>
    /// <param name="id">Идентификатор выбираемого робота</param>
    private void ChooseRobot(string id)
    {
        CurrentRobot = id;
        
        // Если робот есть в наличии у игрока
        if (PlayerRobotAvailability(id))
        {
            SignalAIBehaviour robot = PlayerControllerComponent.SignalAI;

            RobotStats.Robot = robot;

            RobotOverview.ToBuild = false;
            RobotOverview.Robot = robot;

            RobotSelector.SelectRobot(id,false);

        }
        else
        {
            var data = RobotBuilder.Data.FirstOrDefault(x => x.RobotPrefab.ID == id);

            if (data != null)
            {
                SignalAIBehaviour robot = data.RobotPrefab;

                // Null т.к. это робот не игрока
                RobotStats.Robot = null;

                // Это необязательное условие, просто нечего отрисовывать обзору при отсутствии 
                if (robot != null)
                {
                    RobotOverview.ToBuild = true;
                    RobotOverview.RobotBuildPanel.SetDetails(data);

                    RobotOverview.Robot = robot;

                    RobotSelector.SelectRobot(id, true);
                }
            }
            else
            {
                ChooseRobot();
            }
        }
        
    }

    /// <summary>
    /// Выбирает первого робота из списка доступных
    /// </summary>
    private void ChooseRobot()
    {
        var data = RobotBuilder.Data.FirstOrDefault();

        if (data != null)
        {
            SignalAIBehaviour robot = data.RobotPrefab;

            RobotStats.Robot = null;

            // Это необязательное условие, протсо нечего отрисовывать обзору при отсутствии 
            if (robot != null)
            {
                CurrentRobot = robot.ID;
                RobotOverview.ToBuild = true;
                RobotOverview.RobotBuildPanel.SetDetails(data);

                RobotOverview.Robot = robot;

                RobotSelector.SelectRobot(robot.ID, true);
            }
        }
        else
        {
            // Уведомление об отсутствии роботов для постройки
        }
    }
    
    protected override void OnSetCraftTable()
    {

    }

    /// <summary>
    /// Проверка на наличие робота у игрока
    /// </summary>
    /// <param name="id">Идентификатор робота</param>
    private bool PlayerRobotAvailability(string id)
    {
        if(PlayerControllerComponent.SignalAI != null)
        {
            if(PlayerControllerComponent.SignalAI.ID == id)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private void BuildRobot()
    {
        var data = RobotBuilder.Data.FirstOrDefault(x => x.RobotPrefab.ID == CurrentRobot);

        // Если робот готов к постройке, то строим
        if (data.ReadyToBuild)
        {
            var instance = RobotBuilder.BuildRobot(data.RobotPrefab.ID);

            instance.transform.position = Spawn.transform.position;
            instance.gameObject.SetActive(true);

            instance.SendCreator(PlayerControllerComponent.gameObject);
            instance.Enable();

            PlayerControllerComponent.SetSignalAI(instance);

            ChooseRobot(instance.ID);
        }
        else
        {
            //List<IInventoryItem> playerDetails = PlayerControllerComponent.InventoryComponent.InventorySystem.GetListOfInventoryItem(InventoryItemType.Detail);
            //DetailData detail = playerDetails.FirstOrDefault() as DetailData;

            //if(detail != null)
            //{
            //    // Добавляем деталь
            //    data.AddDetail(1);

            //    // Изменяем интерфейс
            //    RobotOverview.RobotBuildPanel.SetDetails(data.Current, data.Details);
            //}
            //else
            //{
            //    // Уведомление о нехватке деталей
            //}
            //Добавляем деталь
            data.AddDetail(1);

            // Изменяем интерфейс
            RobotOverview.RobotBuildPanel.SetDetails(data);

        }
    }
    
}


public interface ISignalAIObserver
{
    /// <summary>
    /// Текущий робот. Может быть null
    /// </summary>
    SignalAIBehaviour Robot { get; }

    /// <summary>
    /// Установка робота
    /// </summary>
    /// <param name="robot">Не может быть null</param>
    void SetRobot(SignalAIBehaviour robot);

    /// <summary>
    /// Вызывается из вне если нет робота 
    /// </summary>
    void OnRobotNull();

    /// <summary>
    /// Очистка информации о текущем роботе
    /// </summary>
    void ClearRobot();
}