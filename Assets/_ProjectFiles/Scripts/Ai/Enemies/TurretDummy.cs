using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AIB.AIBehaviourStates;
using AIB.AIBehaviourStates.TurretDummyStates;
using AIB.AIBehaviours;

namespace AIB.AIBehaviours
{
    public class TurretDummy : AIBehaviour
    {
        /// <summary>
        /// Скорость поворота
        /// </summary>
        public float TurnSpeed { get => turnSpeed; }
        [SerializeField] private float turnSpeed;

        /// <summary>
        /// Дистанция захвата цели
        /// </summary>
        public float CaptureDistance { get => captureDistance; }
        [SerializeField] private float captureDistance;

        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDisableBeahviour()
        {
            SetBehaviourState(new EmptyState(this));
        }

        protected override void OnEnableBehaviour()
        {
            SetBehaviourState(new IdleBehaviourState(this));
        }


        public override void SendCreator(GameObject creator)
        {
            return;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(transform.position,transform.position+ transform.forward * CaptureDistance);
        }
    }
}

namespace AIB.AIBehaviourStates.TurretDummyStates
{
    public class IdleBehaviourState : AIBehaviourState
    {
        private readonly TurretDummy Turret;

        public IdleBehaviourState(AIBehaviour parent) : base(parent)
        {
            Turret = parent as TurretDummy;

            if (Turret == null)
                throw new System.InvalidCastException();

            Name = "TurretIdle";

            forwardAnim = Animator.StringToHash("Forward");
            sideAnim = Animator.StringToHash("Side");

        }

        private int forwardAnim;
        private int sideAnim;

        public override AIBehaviourState Update(AIUpdateData updateData)
        {
            var player = updateData.playerControllerComponent;
            var turretTransform = Turret.transform;
            var dir = player.transform.position - turretTransform.position;

            if (dir.magnitude <= Turret.CaptureDistance)
            {
                dir.y = 0.0f;
                dir.Normalize();

                var rotationGoal = Quaternion.LookRotation(dir);
                var newRotation = Quaternion.RotateTowards(turretTransform.rotation, rotationGoal, Turret.TurnSpeed * updateData.deltaTime);

                Turret.transform.rotation = newRotation;
            }

            Turret.Animator.SetFloat(sideAnim, Turret.AngularVelocity);

            return this;
        }
    }
}
