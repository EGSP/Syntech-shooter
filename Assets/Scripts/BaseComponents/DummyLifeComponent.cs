using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyLifeComponent : LifeComponent
{
    public GameObject HealthBar;
    public GameObject ArmourBar;

    public float RegenerationSpeed;

    public float RegenerationDelay;
    private float regenerationDelay;

    private Material mat;
    private Material armourMat;
    protected override void Start()
    {
        base.Start();
        regenerationDelay = RegenerationDelay;
        mat = HealthBar.GetComponent<Renderer>().material;
        armourMat = ArmourBar.GetComponent<Renderer>().material;
    }

    public override void Update()
    {
        base.Update();
        regenerationDelay -= Time.deltaTime;
        if(regenerationDelay < 0)
        {
            regenerationDelay = 0;

            Heal(RegenerationSpeed * Time.deltaTime);
        }
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

        armourMat.SetFloat("_HealthOpacity", ActiveArmour.Battery/ActiveArmour.MaxBattery);
        mat.SetFloat("_HealthOpacity", Health / MaxHealth);

    }
}
