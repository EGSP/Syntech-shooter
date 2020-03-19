using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using AIB.AIBehaviourStates;

namespace AIB.AIBehaviours
{
    public abstract class AIBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Текущее состояние
        /// </summary>
        protected AIBehaviourState CurrentBehaviourState;

        /// <summary>
        /// Подключается к AIManager.OnUpdate
        /// </summary>
        protected virtual void Awake()
        {
            if (CurrentBehaviourState == null)
                CurrentBehaviourState = new EmptyState(this);

            AIManager.Instance.OnUpdate += UpdateState;
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

        public bool CanReceiveSignal { get; protected set; }

        /// <summary>
        /// Вызывается при получении сигнала
        /// </summary>
        public Action OnSignalReceived = delegate { };

        /// <summary>
        /// Метод вызываемый из вне любым объектом
        /// </summary>
        public abstract void SignalReceive();
    }
    
}