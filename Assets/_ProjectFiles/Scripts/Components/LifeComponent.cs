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

    /// <summary>
    /// Вызывается при изменении количества текущего здоровья
    /// </summary>
    public event Action<float, float> OnHealthChanged = delegate { };

    /// <summary>
    /// Вызывается при получении урона
    /// </summary>
    public event Action<DamageData> OnDamageTaken = delegate { };

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

    /// <summary>
    /// Возвращает true если полное здоровье
    /// </summary>
    public bool IsHealthful
    {
        get
        {
            if (Health == MaxHealth)
                return true;

            return false;
        }
    }

    public ActiveArmour ActiveArmour { get; private set; }

    private List<LifeComponentEffect> LifeComponentEffects;
    private List<DamageBehaviour> DamageBehaviours;
    private List<IDamageTakePerk> DamageResisters;

    private void Constructor()
    {
        MaxHealth = Preset.MaxHealth;
        Health = MaxHealth;

        LifeComponentEffects = new List<LifeComponentEffect>();
        DamageBehaviours = new List<DamageBehaviour>();

        if (DamageResisters != null)
        {
            DamageResisters.Sort((x,y) => x.DamagePerkPriority.CompareTo(y.DamagePerkPriority));
        }
        else
        {
            DamageResisters = new List<IDamageTakePerk>();
        }

        ActiveArmour = new ActiveArmour(ArmourPreset);
    }
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        OnDamageTaken += InvokeDamageTakePerks;
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
        OnDamageTaken(damageData);



        // Погашенный урон
        // Если число меньше нуля, то броня полностью поглатила урон
        var armourPen = 1 - damageData.armourPenetration.ToInt();
        var damageForHealth = damageData.baseDamage * damageData.armourModifier - ActiveArmour.Battery * armourPen;
        ActiveArmour.Hurt(damageData.baseDamage * damageData.armourModifier * armourPen);

        // Урон наносимый телу
        var remain = Mathf.Max(0, damageForHealth);
        Health -= remain;
    }

    public void AddEffect(LifeComponentEffect effect)
    {
        // Если активна броня и эффект не может пробить ее
        if (ActiveArmour.IsActive == true && effect.AddThroughArmour == false)
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
            // Если совпадение нашлось, то эффекты складываются
            coincidence.Merge(effect);
        }
    }

    public void AddDamageBehaviour(DamageBehaviour beh)
    {
        // Если активна броня и поведение не может пробить ее
        if (ActiveArmour.IsActive == true && beh.AddThroughArmour == false)
            return;

        // Совпадение
        var coincidence = DamageBehaviours.FirstOrDefault(x => x.ID == beh.ID);

        if (coincidence == null)
        {
            DamageBehaviours.Add(beh);
        }
        else
        {
            // Если совпадение нашлось, то эффекты складываются
            coincidence.Merge(beh);
        }
    }

    public void AddDamageTakePerk(IDamageTakePerk damageResister)
    {
        if (DamageResisters == null)
            DamageResisters = new List<IDamageTakePerk>();

        InsertByPriority(damageResister);
    }

    /// <summary>
    /// Вызов всех перков получения урона
    /// </summary>
    /// <param name="damageData">Полученный урон</param>
    public void InvokeDamageTakePerks(DamageData damageData)
    {
        for (int i = 0; i < DamageResisters.Count; i++)
            DamageResisters[i].PerkDamage(damageData);
    }

    /// <summary>
    /// Добавляет в список резист с приоритетом. При равном приоритете новый будет иметь меньший индекс 
    /// </summary>
    /// <param name="damageResister"></param>
    private void InsertByPriority(IDamageTakePerk damageResister)
    {
        if(DamageResisters.Count == 0)
        {
            DamageResisters.Add(damageResister);
        }
            
        for(int i = 0; i < DamageResisters.Count; i++)
        {
            if(damageResister.DamagePerkPriority <= DamageResisters[i].DamagePerkPriority)
            {
                DamageResisters.Insert(++i, damageResister);
                return;
            }
        }
    }
}
