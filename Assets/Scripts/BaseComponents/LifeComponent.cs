using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private void Constructor()
    {
        MaxHealth = Preset.MaxHealth;
        Health = MaxHealth;

        LifeComponentEffects = new List<LifeComponentEffect>();
    }
    
    // Start is called before the first frame update
    void Start()
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

        LifeComponentEffects.RemoveAll(x => x == null);
    }

    // Лечение
    public void Heal(float value)
    {
        Health += value;
    }
    // Получение урона
    public void Hurt(float value)
    {
        Health -= value;
    }

    public void AddEffect(LifeComponentEffect effect)
    {
        // Совпадение
        var coincidence = LifeComponentEffects.FirstOrDefault(x => x.ID == effect.ID);

        if(coincidence == null)
        {
            LifeComponentEffects.Add(effect);
        }
        else
        {
            print("Coincidence exist");
            // Если совпадение нашлось, то эффекты складываются
            coincidence.Merge(effect);
        }
    }
}
