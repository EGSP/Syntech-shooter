using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailDummyLifeComponent : LifeComponent
{

    /// <summary>
    /// Количество деталей при выпадении
    /// </summary>
    [SerializeField] private int detailDropCount;
    /// <summary>
    /// Импульс передаваемый телу
    /// </summary>
    [SerializeField] private float impulse;
    [SerializeField] private Vector3 maxAngles;
    [SerializeField] private Vector3 spawnOffset;
    /// <summary>
    /// Задержка между следующим дропом
    /// </summary>
    [SerializeField] private float delay;

    private TimerCallbacker dropDelay;
    private bool canDrop;

    protected override void Awake()
    {
        base.Awake();

        dropDelay = new TimerCallbacker(delay);

        dropDelay.OnEmmitionEndCallback += () => canDrop = true;
        dropDelay.OnResetCallback += () => canDrop = false;
    }

    public override void Update()
    {
        base.Update();

        dropDelay.Update(Time.deltaTime);
    }

    public override void Hurt(DamageData damageData)
    {
        base.Hurt(damageData);

        dropDelay.Reset();

        for (int i = 0; i < detailDropCount; i++)
        {
            // Игровой объект детали
            var detail = PoolManager.Instance.Take("detail") as Detail;

            detail.transform.position = transform.position+spawnOffset;
            detail.gameObject.SetActive(true);

            var angle = Quaternion.Euler(0, Random.Range(-maxAngles.y, maxAngles.y), Random.Range(0, maxAngles.x));
            
            // Прикладываем импульс
            detail.AddImpulse((angle * transform.forward).normalized, impulse);


        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 offseted = transform.position + spawnOffset;
        // X angle
        Gizmos.color = Color.red;
        Gizmos.DrawLine(offseted, offseted + Quaternion.Euler(0, 0, maxAngles.x) * transform.forward);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(offseted, offseted  + Quaternion.Euler(0, maxAngles.y, 0) * transform.forward);
        Gizmos.DrawLine(offseted, offseted  + Quaternion.Euler(0, -maxAngles.y, 0) * transform.forward);
    }
}
