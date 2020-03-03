using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LifeEffects;
public class ShockDummyLifeComponent : LifeComponent, IShockable
{
    public GameObject HealthBar;
    public GameObject ArmourBar;

    public float RegenerationSpeed;

    public float RegenerationDelay;
    private float regenerationDelay;

    private Material mat;
    private Material armourMat;

    private Animator anim;

    public bool IsShocked { get; private set; }

    private TimerCallbacker ShockTimer;

    protected override void Awake()
    {
        base.Awake();
        regenerationDelay = RegenerationDelay;
        mat = HealthBar.GetComponent<Renderer>().material;
        armourMat = ArmourBar.GetComponent<Renderer>().material;

        anim = GetComponent<Animator>();
        ShockTimer = new TimerCallbacker(0);

        ShockTimer.OnEmmitionEndCallback += () => anim.SetFloat("Speed", 1);
        ShockTimer.OnResetCallback += () => anim.SetFloat("Speed", 0);
    }

    public override void Update()
    {
        base.Update();
        regenerationDelay -= Time.deltaTime;
        if (regenerationDelay < 0)
        {
            regenerationDelay = 0;

            Heal(RegenerationSpeed * Time.deltaTime);
        }

        ShockTimer.Update(Time.deltaTime);
        
    }

    public override void Heal(float value)
    {
        base.Heal(value);

        armourMat.SetFloat("_HealthOpacity", ActiveArmour.Battery / ActiveArmour.MaxBattery);
        mat.SetFloat("_HealthOpacity", Health / MaxHealth);
    }

    public override void Hurt(DamageData value)
    {
        base.Hurt(value);

        regenerationDelay = RegenerationDelay;

        armourMat.SetFloat("_HealthOpacity", ActiveArmour.Battery / ActiveArmour.MaxBattery);
        mat.SetFloat("_HealthOpacity", Health / MaxHealth);

    }

    public void Shock(float _ShockTime)
    {
        ShockTimer.ChangeTime(_ShockTime);
        ShockTimer.Reset();
    }

   
}