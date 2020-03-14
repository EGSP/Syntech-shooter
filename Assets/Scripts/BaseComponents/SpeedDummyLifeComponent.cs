using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LifeEffects;
public class SpeedDummyLifeComponent : LifeComponent, ISpeedModifiable
{
    public GameObject HealthBar;
    public GameObject ArmourBar;

    public float RegenerationSpeed;

    public float RegenerationDelay;
    private float regenerationDelay;

    private Material mat;
    private Material armourMat;

    [SerializeField] private Animator anim;

    public float SpeedModifier { get; private set; }
    public float ModifierTime { get; private set; }

    private TimerCallbacker modifierTimer;

    protected override void Awake()
    {
        base.Awake();
        regenerationDelay = RegenerationDelay;
        mat = HealthBar.GetComponent<Renderer>().material;
        armourMat = ArmourBar.GetComponent<Renderer>().material;

        if (anim == null)
            anim = GetComponent<Animator>();

        modifierTimer = new TimerCallbacker(ModifierTime);
        SpeedModifier = 1;
        // Спад эффекта модфификатора
        modifierTimer.OnEmmitionEndCallback += () =>
        {
            SpeedModifier = 1;
            anim.SetFloat("Speed",SpeedModifier);
        };
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

        modifierTimer.Update(Time.deltaTime);
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

    public void EnqueueModifier(float _SpeedModifier, float _ModifierTime)
    {
        // Берется минимальный модификатор
        SpeedModifier = Mathf.Min(SpeedModifier, _SpeedModifier);
        ModifierTime = _ModifierTime;

        modifierTimer.ChangeTime(ModifierTime);
        modifierTimer.Reset();

        anim.SetFloat("Speed", SpeedModifier);
    }
}