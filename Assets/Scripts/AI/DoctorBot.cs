using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using UnityEngine.AI;

using AIB.AIBehaviourStates.DoctorBotStates;

namespace AIB.AIBehaviours
{
    public class DoctorBot : SignalAIBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float angularSpeed;

        [Header("Healing")]
        [Range(-1,1)]
        [SerializeField] private float healingDot;
        [SerializeField] private float healingDistance;
        [SerializeField] private float healingInterval;
        [SerializeField] private float healPerCharge;

        [Header("Charge")]
        [SerializeField] private int chargePerHealing;
        [SerializeField] private int chargeCapacity;
        [SerializeField] private float chargeRefillInterval;
        [SerializeField] private int chargePerRefill;

        [Header("Supply")]
        [SerializeField] private float findSupplyRadius;
        [SerializeField] private float refillDistance;
        [SerializeField] private LayerMask supplyLayerMask;
        [SerializeField] private string supplyID;
        

        [SerializeField] private Animator animator;
        

        /// <summary>
        /// Скорость передвижения
        /// </summary>
        public float MoveSpeed
        {
            get => moveSpeed;
            private set
            {
                moveSpeed = value;
                NavAgent.speed = moveSpeed;
            }
        }

        /// <summary>
        /// Скорость поворота
        /// </summary>
        public float AngularSpeed
        {
            get => angularSpeed;
            private set
            {
                angularSpeed = value;
                NavAgent.angularSpeed = angularSpeed;
            }
        }

        /// <summary>
        /// Угол при котором боту не нужно поворачиваться для лечения
        /// </summary>
        public float HealingDot { get => healingDot; private set => healingDot = value; }

        /// <summary>
        /// Дистанция на которой бот может лечить
        /// </summary>
        public float HealingDistance { get => healingDistance; private set => healingDistance = value; }

        /// <summary>
        /// Интервал использования зарядов лечения
        /// </summary>
        public float HealingInterval { get => healingInterval; private set => healingInterval = value; }

        /// <summary>
        /// Количество хила за единицу заряда
        /// </summary>
        public float HealPerCharge { get => healPerCharge; private set => healPerCharge = value; }

        /// <summary>
        /// Количество затрачиваемых зарядов за один интервал лечения
        /// </summary>
        public int ChargePerHealing { get => chargePerHealing; private set => chargePerHealing = value; }

        /// <summary>
        /// Вместимость зарядов
        /// </summary>
        public int ChargeCapacity { get => chargeCapacity; private set => chargeCapacity = value; }

        /// <summary>
        /// Текущее количество зарядов
        /// </summary>
        public int ChargeCount { get => chargeCount;
            set
            {
                chargeCount = Mathf.Clamp(value, 0, ChargeCapacity);
                ChargeCountChanged(ChargeOpacity);
            }
        }
        private int chargeCount;

        /// <summary>
        /// Интервал пополнения зарядов
        /// </summary>
        public float ChargeRefillInterval { get => chargeRefillInterval; private set => chargeRefillInterval = value; }

        /// <summary>
        /// Количество пополняемых зарядов за единицу пополнения
        /// </summary>
        public int ChargePerRefill { get => chargePerRefill; private set => chargePerRefill = value; }

        /// <summary>
        /// Радиус поиска снабжения
        /// </summary>
        public float FindSupplyRadius { get => findSupplyRadius; private set => findSupplyRadius = value; }
        
        /// <summary>
        /// Расстояние на котором можно пополнять заряды
        /// </summary>
        public float RefillDistance { get => refillDistance; set => refillDistance = value; }

        /// <summary>
        /// Идентификатор запаса зарядов
        /// </summary>
        public string SupplyID { get => supplyID; set => supplyID = value; }

        /// <summary>
        /// Используемый аниматор
        /// </summary>
        public Animator Animator { get => animator; set => animator = value; }
        
        /// <summary>
        /// Объект которого нужно лечить 
        /// </summary>
        public LifeComponent Patient { get; private set; }

        /// <summary>
        /// Идет ли в данный момент пополнение заряда
        /// </summary>
        public bool IsRefilling { get; set; }

        /// <summary>
        /// Полон ли запас зарядов
        /// </summary>
        public override bool IsChargeful
        {
            get
            {
                if (ChargeCount == ChargeCapacity)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Пуст ли запас зарядов
        /// </summary>
        public override bool IsChargeless
        {
            get
            {
                if (ChargeCount == 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Наполненность заряда
        /// </summary>
        public override float ChargeOpacity
        {
            get
            {
                return ChargeCount / ChargeCapacity;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            if (Animator == null)
                Animator = GetComponent<Animator>();

            // Для вызова свойств
            MoveSpeed = MoveSpeed;
            AngularSpeed = AngularSpeed;

            ChargeCount = ChargeCapacity;
        }

        protected override void Start()
        {
            base.Start();
        }

        public void SetPatient(LifeComponent patient)
        {
            // Если пациент не мы
            if (patient != LifeComponent)
                Patient = patient;
        }

        public override void SignalReceive()
        {
            SignalReceived();
        }

        /// <summary>
        /// Возвращает ближайшую станцию снабжения. Возвращает null если не найдена станция
        /// </summary>
        public RobotSupplyStation FindSupplyStation()
        {
            var navPos = NavAgent.transform.position;
            var colliders = Physics.OverlapSphere(navPos, FindSupplyRadius, supplyLayerMask, QueryTriggerInteraction.Collide);

            if (colliders.Length > 0)
            {
                var supplies = colliders.Cast<RobotSupplyStation>()
                    .Where(x => x.SupplyID == SupplyID);

                // Находим ближайшую станцию
                if (supplies.Count() > 0)
                {
                    float minDist = (supplies.First().transform.position - navPos).magnitude;
                    var nearestSup = supplies.First();

                    foreach (var sup in supplies)
                    {
                        var dist = (sup.transform.position - navPos).magnitude;
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearestSup = sup;
                        }
                    }

                    return nearestSup;
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Возвращает заряд в максимально возможном количестве. Может вернуть 0
        /// </summary>
        /// <param name="countNeed">Сколько нужно получить</param>
        /// <returns></returns>
        public int TakeCharge(int countNeed)
        {
            var taken = Mathf.Clamp(countNeed, 0, ChargeCount);

            return taken;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, healingDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, findSupplyRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, refillDistance);
        }

        protected override void OnEnableBehaviour()
        {
            SetBehaviourState(new IdleBehaviourState(this));
        }

        protected override void OnDisableBeahviour()
        {
            NavAgent.isStopped = true;
        }

        public override void SendCreator(GameObject creator)
        {
            var life = creator.GetComponent<LifeComponent>();

            // Если создатель имеет здоровье, то он станет пациентом
            if(life != null)
            {
                SetPatient(life);
            }
        }
    }
}
