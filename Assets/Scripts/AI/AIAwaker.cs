using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AIB.AIBehaviours;

namespace AIB
{
    /// <summary>
    /// Автоматически включает AI
    /// </summary>
    public class AIAwaker : MonoBehaviour
    {
        private void Awake()
        {
            var ai = GetComponent<AIBehaviour>();

            if (ai != null)
                EnableAI(ai);

            Destroy(this);
        }

        private void EnableAI(AIBehaviour ai)
        {
            ai.Enable();
        }
    }
}
