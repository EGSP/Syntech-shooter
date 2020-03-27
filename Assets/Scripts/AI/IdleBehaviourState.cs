using AIB.AIBehaviours;

using UnityEngine;

namespace AIB.AIBehaviourStates.DoctorBotStates
{
    public class IdleBehaviourState : SignalAIBehaviourState
    {
        /// <summary>
        /// Бот этого поведения
        /// </summary>
        private readonly DoctorBot Doctor;

        public IdleBehaviourState(AIBehaviour parent) : base(parent)
        {
            var doctor = parent as DoctorBot;

            if(doctor != null)
            {
                Doctor = doctor;
            }
            else
            {
                throw new System.Exception("Попытка назначить состояние не принадлежащее DoctorBot");
            }

            healTimerCallbacker = new TimerCallbacker(Doctor.HealingInterval);
            healTimerCallbacker.OnEmmitionEndCallback += () => CanHeal = true;
            healTimerCallbacker.OnResetCallback += () => CanHeal = false;

            Name = "DoctorIdle";
        }

        private TimerCallbacker healTimerCallbacker;

        private bool CanHeal;

        public override AIBehaviourState Update(AIUpdateData updateData)
        {
            healTimerCallbacker.Update(updateData.deltaTime);

            // Если есть пациент
            if(Doctor.Patient != null)
            {
                var patient = Doctor.Patient;

                // Если нужно лечить
                if (patient.IsHealthful == false)
                {
                    // Если есть заряд у доктора
                    if (Doctor.IsChargeless == false)
                    {
                        var pos = Doctor.NavAgent.transform.position;
                        var dir = (patient.transform.forward - pos).normalized;

                        var distance = (patient.transform.position - pos).magnitude;

                        // Если бот на дистанции лечения
                        if(distance < Doctor.HealingDistance)
                        {
                            // Смотрит ли бот на пациента
                            var dot = Vector3.Dot(Doctor.NavAgent.transform.forward, dir); 

                            // Если бот смотрит на пациента
                            if(dot >= Doctor.HealingDot)
                            {
                                if (CanHeal)
                                {
                                    // Хилим пациента
                                    HealPatient();

                                    healTimerCallbacker.Reset();
                                }

                                return this;
                            }

                            // Нужно повернуться в сторону игрока
                            Doctor.NavAgent.isStopped = true;
                            var doctorRotation = Doctor.transform.rotation;

                            Doctor.transform.rotation = Quaternion.RotateTowards(
                                doctorRotation,
                                Quaternion.LookRotation(dir, Vector3.up),
                                Doctor.AngularSpeed);

                            return this;
                        }

                        // Если можем лечить, но не достаем
                        Doctor.NavAgent.SetDestination(patient.transform.position);
                        return this;
                    }

                    // Если заряда нет
                    return new RefillBehaviourState(Parent);

                }

                // Если не нужно лечить 
                Doctor.NavAgent.SetDestination(patient.transform.position);
                return this;
            }

            // Стоим на месте
            Doctor.NavAgent.isStopped = true; 
            return this;
        }

        /// <summary>
        /// Лечение пациента
        /// </summary>
        public void HealPatient()
        {
            var heal = Doctor.TakeCharge(1 * Doctor.ChargePerHealing) * Doctor.HealPerCharge;

            Doctor.Patient.Heal(heal);
        }

        public override void OnBehaviourSignalReceived()
        {
            var supply = Doctor.FindSupplyStation();

            // Если найдены припасы и доктор не полностью заряжен
            if (supply != null && !Doctor.IsChargeful)
            {
                Doctor.SetBehaviourState(new RefillBehaviourState(Parent));
            }
        }
    }

    /// <summary>
    /// Состояние заправки
    /// </summary>
    public class RefillBehaviourState : SignalAIBehaviourState
    {
        /// <summary>
        /// Бот этого поведения
        /// </summary>
        private readonly DoctorBot Doctor;

        public RefillBehaviourState(AIBehaviour parent) : base(parent)
        {
            var doctor = parent as DoctorBot;

            if (doctor != null)
            {
                Doctor = doctor;
            }
            else
            {
                throw new System.Exception("Попытка назначить состояние не принадлежащее DoctorBot");
            }

            refillIntervalCallbacker = new TimerCallbacker(Doctor.ChargeRefillInterval);

            refillIntervalCallbacker.OnEmmitionEndCallback += ()=> CanFill = true;
            refillIntervalCallbacker.OnResetCallback += () => CanFill = false;

            Name = "DoctorRefill";
        }
        

        /// <summary>
        /// Станция пополнения. Может быть null.
        /// </summary>
        private RobotSupplyStation SupplyStation;

        /// <summary>
        /// Таймер интервалов перезарядки
        /// </summary>
        private TimerCallbacker refillIntervalCallbacker;
        
        /// <summary>
        /// Можно ли в данный момент пополнить заряд
        /// </summary>
        private bool CanFill;

        public override AIBehaviourState Update(AIUpdateData updateData)
        {
            // Текущая позиция бота
            var pos = Doctor.NavAgent.transform.position;

            if (SupplyStation == null)
            {
                Doctor.IsRefilling = false;

                // Пока двигаемся за пациентом 
                Doctor.NavAgent.SetDestination(pos);

                // Находим ближайшую станцию 
                SupplyStation = Doctor.FindSupplyStation();
                return this; 
            }
            else
            {
                // Если бот заправлен или станция опустела
                if(Doctor.IsChargeful || SupplyStation.IsEmpty)
                {
                    Doctor.IsRefilling = false;

                    // Переходим в обычный режим 
                    return new IdleBehaviourState(Parent);
                }

                // Если станция есть 

                // Расстояние между ботом и станцией
                var distance = (SupplyStation.transform.position - pos).magnitude;

                // Если бот вышел на дистанцию пополнения
                if(distance < Doctor.RefillDistance)
                {
                    Doctor.IsRefilling = true;

                    Doctor.NavAgent.isStopped = true;

                    refillIntervalCallbacker.Update(updateData.deltaTime);

                    if (CanFill)
                    {
                        Fill();

                        refillIntervalCallbacker.Reset();
                    }

                    return this;
                }


                Doctor.IsRefilling = false;
                // Устанавливаем точку для движения
                Doctor.NavAgent.SetDestination(SupplyStation.transform.position);
                return this;
            }

        }

        /// <summary>
        /// Установка станции пополнения. 
        /// </summary>
        /// <param name="supplyStation">Станция должна подходить по supplyID. Однако проверка имеется.</param>
        public void SetRefillStation(RobotSupplyStation supplyStation)
        {
            if (supplyStation == null)
                return;

            if (supplyStation.SupplyID != Doctor.SupplyID)
                return;

            SupplyStation = supplyStation;
        }

        /// <summary>
        /// Пополнение заряда с помощью станции
        /// </summary>
        private void Fill()
        {
            var taken = SupplyStation.TakeSupply(Doctor.ChargePerRefill);

            Doctor.ChargeCount += taken;
        }

        public override void OnBehaviourSignalReceived()
        {
            // Отменяем состояние пополнения, если пополнение еще не началось и есть заряды
            if(Doctor.IsRefilling == false && !Doctor.IsChargeless)
            {
                Doctor.SetBehaviourState(new IdleBehaviourState(Parent));
            }
        }
    }


}
