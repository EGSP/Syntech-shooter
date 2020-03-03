using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DamageEffects;

public class LifeComponent : MonoBehaviour
{
    [SerializeField] private LifeComponentPreset Preset;
    [SerializeField] private ActiveArmourPreset ArmourPreset;

    public event Action<float, float> OnHealthChanged = delegate { };

    public float MaxHealth { get; protected set; }
    public float Health {
        get =>health;
        protected set
        {
            if (value < 0)
                value = 0;

            if (value > MaxHealth)
                value = MaxHealth;
            health = value;

            OnHealthChanged(health, MaxHealth);
        }
    }
    private float health;

    public ActiveArmour ActiveArmour { get; private set; }

    private List<LifeComponentEffect> LifeComponentEffects;
    private List<DamageBehaviour> DamageBehaviours;

    private void Constructor()
    {
        MaxHealth = Preset.MaxHealth;
        Health = MaxHealth;

        LifeComponentEffects = new List<LifeComponentEffect>();
        DamageBehaviours = new List<DamageBehaviour>();

        ActiveArmour = new ActiveArmour(ArmourPreset);
    }
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        Constructor();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        var deltaTime = Time.deltaTime;

        // Обновление активной брони
        ActiveArmour.Update(deltaTime);

        // Обновление эффектов
        for (int i = 0; i < LifeComponentEffects.Count; i++)
        {
            currentEffect = LifeComponentEffects[i];
            var newEffect = currentEffect.Invoke(this, deltaTime);
           
            LifeComponentEffects[i] = newEffect;
        }
        // Удаление отработавших эффектов
        LifeComponentEffects.RemoveAll(x => x == null);

        // Обновление поведений
        for(int i = 0; i < DamageBehaviours.Count; i++)
        {
            var newBeh = DamageBehaviours[i].Update(deltaTime);

            DamageBehaviours[i] = newBeh;
        }
        // Удаление отработавших поведений
        DamageBehaviours.RemoveAll(x => x == null);
        
    }

    // Лечение
    public virtual void Heal(float value)
    {
        Health += value;
    }

    // Эта магическая переменная нужна для определения пробиваемости брони эффектом. 
    // Она сделана отдельной переменной, потому что делать дополнительный аргумент было невозможным из-за большой кодовой базы
    private LifeComponentEffect currentEffect;
    // Получение урона
    public virtual void Hurt(DamageData damageData)
    {
        // Нанесение урона сквозь броню
        if (currentEffect != null && currentEffect.ArmourPenetration == true)
        {
            Health -= damageData.baseDamage;
            return;
        }

        // Погашенный урон
        // Если число меньше нуля, то броня полностью поглатила урон
        var damageForHealth = damageData.baseDamage * damageData.armourModifier - ActiveArmour.Battery;
        ActiveArmour.Hurt(damageData.baseDamage * damageData.armourModifier);

        // Урон наносимый телу
        var remain = Mathf.Max(0, damageForHealth);
        Health -= remain;
    }

    public void AddEffect(LifeComponentEffect effect)
    {
        // Если активна броня и эффект не может пробить ее
        if (ActiveArmour.IsActive == true && effect.ArmourPenetration == false)
            return;

        // Совпадение
        var coincidence = LifeComponentEffects.FirstOrDefault(x => x.ID == effect.ID);

        if(coincidence == null)
        {
            effect.SetLifeComponent(this);
            LifeComponentEffects.Add(effect);
        }
        else
        {
            print("Coincidence exist (Eff)");
            // Если совпадение нашлось, то эффекты складываются
            coincidence.Merge(effect);
        }
    }

    public void AddDamageBehaviour(DamageBehaviour beh)
    {
        // Если активна броня и поведение не может пробить ее
        if (ActiveArmour.IsActive == true && beh.ArmourPenetration == false)
            return;

        // Совпадение
        var coincidence = DamageBehaviours.FirstOrDefault(x => x.ID == beh.ID);

        if (coincidence == null)
        {
            DamageBehaviours.Add(beh);
        }
        else
        {
            print("Coincidence exist (Beh)");
            // Если совпадение нашлось, то эффекты складываются
            coincidence.Merge(beh);
        }
    }
}
