using AIB.AIBehaviours;

namespace AIB.AIBehaviourStates
{
    public abstract class AIBehaviourState
    {
        /// <summary>
        /// Родитель состояния
        /// </summary>
        public readonly AIBehaviour Parent;

        /// <summary>
        /// Наименование состояния
        /// </summary>
        public readonly string Name;

        public AIBehaviourState(AIBehaviour parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Обновление состояния. Возвращает новое состояние.
        /// </summary>
        /// <param name="updateData">Информация обновления</param>
        /// <returns></returns>
        public abstract AIBehaviourState Update(AIUpdateData updateData);

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public virtual void Dispose()
        {
            
        }

    }

    public abstract class SignalAIBehaviourState : AIBehaviourState
    {
        public SignalAIBehaviourState(AIBehaviour parent) : base(parent)
        {
            var sparent = Parent as SignalAIBehaviour;
            if (sparent != null)
                sparent.OnSignalReceived += OnBehaviourSignalReceived;
        }

        /// <summary>
        /// Вызывается при получении родителем сигнала
        /// </summary>
        public abstract void OnBehaviourSignalReceived();

        public override void Dispose()
        {
            base.Dispose();

            var sparent = Parent as SignalAIBehaviour;
            if (sparent != null)
                sparent.OnSignalReceived -= OnBehaviourSignalReceived;
        }
    }
    
    
}