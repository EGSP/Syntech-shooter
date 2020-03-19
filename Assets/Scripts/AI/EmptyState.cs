using AIB.AIBehaviours;

namespace AIB.AIBehaviourStates
{
    /// <summary>
    /// Пустое состояние, которое делает ничего
    /// </summary>
    public class EmptyState : AIBehaviourState
        {
            public EmptyState(AIBehaviour parent) : base(parent)
            {

            }

            public override AIBehaviourState Update(AIUpdateData updateData)
            {
                return this;
            }
        }
    
}