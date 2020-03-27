using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System;

using AIB.AIBehaviourStates;

namespace AIB.AIBehaviours
{
    [RequireComponent(typeof(LifeComponent))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AIBehaviour : MonoBehaviour, IObservable
    {
        [SerializeField] private string id;
        /// <summary>
        /// Идентификатор робота
        /// </summary>
        public string ID { get => id; }

        [SerializeField] private string robotName;
        /// <summary>
        /// Название робота, ни на что не влияет
        /// </summary>
        public string RobotName
        {
            get
            {
                return robotName;
            }
        }

        /// <summary>
        /// Текущее состояние
        /// </summary>
        protected AIBehaviourState CurrentBehaviourState;

        /// <summary>
        /// Включен ли AI
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Вызывает насильную отписку
        /// </summary>
        public event Action<IObservable> OnForceUnsubcribe;

        /// <summary>
        /// Компонент жизни бота
        /// </summary>
        public LifeComponent LifeComponent { get; private set; }

        /// <summary>
        /// Агент навигационного меша Unity
        /// </summary>
        public NavMeshAgent NavAgent { get; private set; }

        /// <summary>
        /// Подключается к AIManager.OnUpdate
        /// </summary>
        protected virtual void Awake()
        {
            if (CurrentBehaviourState == null)
                CurrentBehaviourState = new EmptyState(this);

            LifeComponent = GetComponent<LifeComponent>();
            NavAgent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            AIManager.Instance.OnUpdate += UpdateState;
        }

        private void OnDisable()
        {
            AIManager.Instance.OnUpdate -= UpdateState;
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        /// <summary>
        /// Метод обновления текущего состояния
        /// </summary>
        /// <param name="updateData"></param>
        protected virtual void UpdateState(AIUpdateData updateData)
        {
            CurrentBehaviourState = CurrentBehaviourState.Update(updateData);
        }

        /// <summary>
        /// Включает поведение и вызывается из вне
        /// </summary>
        public void Enable()
        {
            if (isEnabled)
                return;

            OnEnableBehaviour();
        }

        /// <summary>
        /// Выключает поведение и вызывается из вне
        /// </summary>
        public void Disable()
        {
            if (!isEnabled)
                return;

            OnDisableBeahviour();
        }

        /// <summary>
        /// Вызывается при включении
        /// </summary>
        protected abstract void OnEnableBehaviour();

        /// <summary>
        /// Вызывается при выключении
        /// </summary>
        protected abstract void OnDisableBeahviour();

        /// <summary>
        /// Устанавливает новое состояние если оно не равно null
        /// </summary>
        /// <param name="behaviourState">Новое состояние</param>
        public void SetBehaviourState(AIBehaviourState behaviourState)
        {
            if (behaviourState != null)
            {
                // Освобождаем ресурсы прошлого состояния
                CurrentBehaviourState.Dispose();

                // Присваиваем новое состояние
                CurrentBehaviourState = behaviourState;
            }
        }

        /// <summary>
        /// Получение объекта создателя
        /// </summary>
        /// <param name="creator">объект создатель</param>
        public abstract void SendCreator(GameObject creator);
    }

    /// <summary>
    /// Этот АИ имеет методы-тригеры 
    /// </summary>
    public abstract class SignalAIBehaviour : AIBehaviour
    {
        [Header("Command")]
        [SerializeField] private float commandReload;

        /// <summary>
        /// Время перезарядки команды перед началом следующего вызова
        /// </summary>
        public float CommandReload { get => commandReload; protected set => commandReload = value; }

        /// <summary>
        /// Вызывается при изменении количества зарядов способности
        /// </summary>
        public event Action<float> OnChargeCountChanged = delegate { };

        /// <summary>
        /// Заполненность заряда от 0 до 1
        /// </summary>
        public abstract float ChargeOpacity
        {
            get;
        }

        /// <summary>
        /// Полон ли запас зарядов
        /// </summary>
        public abstract bool IsChargeful
        {
            get;
        }

        /// <summary>
        /// Пуст ли запас зарядов
        /// </summary>
        public abstract bool IsChargeless
        {
            get;
        }

        public bool CanReceiveSignal { get; protected set; }

        /// <summary>
        /// Вызывается при получении сигнала
        /// </summary>
        public event Action OnSignalReceived = delegate { };

        /// <summary>
        /// Метод вызываемый из вне любым объектом
        /// </summary>
        public abstract void SignalReceive();

        protected virtual void ChargeCountChanged(float opacity)
        {
            OnChargeCountChanged(opacity);
        }

        protected virtual void SignalReceived()
        {
            OnSignalReceived();
        }
    }
    
}