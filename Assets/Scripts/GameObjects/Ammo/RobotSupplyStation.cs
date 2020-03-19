using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RobotSupplyStation : MonoBehaviour
{
    [SerializeField] protected string supplyID;
    [Space(10)]
    [Tooltip("задержка перед началом регенерации")]
    [SerializeField] protected float regenerationDelay;
    [Tooltip("Время регенерации одного припаса")]
    [SerializeField] protected float regenerationSpeed;
    [Space(10)]
    [SerializeField] protected int supplyCapacity;


    /// <summary>
    /// Идентификатор припасов
    /// </summary>
    public string SupplyID { get => supplyID; protected set => supplyID = value; }
    
    /// <summary>
    /// Задрежка перед началом восстановления после полного опустошения
    /// </summary>
    public float RegenerationDelay { get => regenerationDelay; protected set => regenerationDelay = value; }

    /// <summary>
    /// Время восстановления единицы припасов
    /// </summary>
    public float RegenerationSpeed { get => regenerationSpeed; protected set => regenerationSpeed = value; }
    
    /// <summary>
    /// Вместимость станции
    /// </summary>
    public int SupplyCapacity { get => supplyCapacity; protected set => supplyCapacity = value; }

    /// <summary>
    /// Текущее количество припасов
    /// </summary>
    public int SupplyCount
    {
        get => supplyCount;
        protected set
        {
            supplyCount = value;
            OnSupplyCountChanged(supplyCount, SupplyCapacity);
        }
    }
    private int supplyCount;

    public bool IsEmpty
    {
        get
        {
            if (SupplyCount == 0)
                return true;

            return false;
        }
    }

    /// <summary>
    /// Должны ли запасы восстанавливаться
    /// </summary>
    protected bool isRegenerationTime;
    
    protected TimerCallbacker timerCallbacker;
    protected DurationCallbacker durationCallbacker;

    /// <summary>
    /// Вызывается при изменении количества припасов
    /// </summary>
    public Action<int, int> OnSupplyCountChanged = delegate { };

    // ToRemove
    public GameObject HealthBar;
    private Material mat;
    

    // Start is called before the first frame update
    void Awake()
    {
        SupplyCount = SupplyCapacity;

        timerCallbacker = new TimerCallbacker(RegenerationDelay);
        durationCallbacker = new DurationCallbacker(1000f, RegenerationSpeed);

        timerCallbacker.OnEmmitionEndCallback += () => isRegenerationTime = true;
        timerCallbacker.OnEmmitionEndCallback += durationCallbacker.Reset;
        timerCallbacker.OnResetCallback += () => isRegenerationTime = false;
        
        durationCallbacker.OnDurationCallback += AutoRefill;

        mat = HealthBar.GetComponent<Renderer>().material;

        OnSupplyCountChanged += (c,m) =>
        {
            mat.SetFloat("_HealthOpacity", c / m);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(SupplyCount != SupplyCapacity)

        if (isRegenerationTime)
        {
            durationCallbacker.Update(Time.deltaTime);
        }

        timerCallbacker.Update(Time.deltaTime);
    }

    /// <summary>
    /// Автоматическое пополнение припасов
    /// </summary>
    protected void AutoRefill()
    {
        Fill(1);
    }

    /// <summary>
    /// Метод взятия припасов из хранилища
    /// </summary>
    /// <param name="count">Нужное количество припасов</param>
    /// <returns>Возвращает возможное количетсво припасов</returns>
    public int TakeSupply(int count)
    {
        timerCallbacker.Reset();

        count = Mathf.Clamp(count, 0, SupplyCount);
        return count;
    }

    /// <summary>
    /// Пополнение запасов
    /// </summary>
    /// <param name="count"></param>
    public void Fill(int count)
    {
        SupplyCount += count;

        SupplyCount = Mathf.Clamp(SupplyCount, 0, SupplyCapacity);
    }

  
}
