using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DamageEffects;

public class LifeComponent : MonoBehaviour
{
    [SerializeField] private LifeComponentPreset Preset;

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
        }
    }
    private float health;

    private List<LifeComponentEffect> LifeComponentEffects;
    private List<DamageBehaviour> DamageBehaviours;

    private void Constructor()
    {
        MaxHealth = Preset.MaxHealth;
        Health = MaxHealth;

        LifeComponentEffects = new List<LifeComponentEffect>();
        DamageBehaviours = new List<DamageBehaviour>();
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Constructor();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        var deltaTime = Time.deltaTime;
        for (int i = 0; i < LifeComponentEffects.Count; i++)
        {
            var newEffect = LifeComponentEffects[i].Invoke(this, deltaTime);

            LifeComponentEffects[i] = newEffect;
        }
        // Удаление отработавших эффектов
        LifeComponentEffects.RemoveAll(x => x == null);

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
    // Получение урона
    public virtual void Hurt(float value)
    {
        Health -= value;
    }

    public void AddEffect(LifeComponentEffect effect)
    {
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
