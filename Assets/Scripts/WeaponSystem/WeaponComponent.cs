#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Linq;

using WeaponSystem;

public abstract class WeaponComponent : MonoBehaviour
{
    /// <summary>
    /// Идентификатор, по которому будут искаться все ресурсы связанные с этим оружием
    /// </summary>
    public string ID;

    /// <summary>
    /// Просто имя, ни на что не влияет
    /// </summary>
    [Tooltip("Просто имя, ни на что не влияет")]
    public string Name;

    /// <summary>
    /// Редкость оружия
    /// </summary>
    public WeaponRarity Rarity;

    [SerializeField] protected Transform Trunk;
    

    [SerializeField] protected DamageData _damage;
    [SerializeField] protected float _reloadTime;
    [SerializeField] protected float _fireRate;
    [Range(0, 90)]
    [SerializeField] protected float SpreadY, SpreadX;
    [SerializeField] protected float BulletFlyDistance;
    // Вес оружия
    [SerializeField] protected float _weight;
    // Количество рук при использовании оружия
    [SerializeField] protected bool _twoHanded;
    [SerializeField] protected float RecoilForce;
    /// <summary>
    /// Время действия отдачи. Не влияет на функциональность оружия
    /// </summary>
    [SerializeField] protected float RecoilEffectTime;

    
    // Максимальное количество шагов пробития
    [SerializeField] protected int PenetrationsCount;
    // Максимальное количетсво рикошетов
    [SerializeField] protected int RicochetsCount;
    // Количество выпускаемых снарядов
    [SerializeField] protected int BulletPerShoot;
    [Space]
    [Range(0, 10)] // Как быстро будет увеличиваться отдача
    [SerializeField] protected float RecoilOpacityIncrease;
    [Range(0, 10)] // Как быстро будет проходить отдача
    [SerializeField] protected float RecoilOpacityDecrease;
    [Range(0,1)]
    [SerializeField] protected float RecoilOpacityLerp;
    /// <summary>
    /// Дистанция смещения оружия в зависимости от opacity. Суммируется с recoilAnimation
    /// </summary>
    [SerializeField] protected float RecoilOpacityOffsetZ;
    /// <summary>
    /// Угол поворота в зависимости от opacity
    /// </summary>
    [SerializeField] protected float RecoilOpacityRotationZ;
    [Space]
    [SerializeField] protected CurveAnimator RecoilAnimation;
    [Range(0, 1)]
    [SerializeField] protected float ReacoilAnimationIntensity;


    // Свойства
    public DamageData Damage { get => _damage; protected set => _damage = value; }
    public float ReloadTime { get => _reloadTime; protected set => _reloadTime = value; }
    public float FireRate { get => _fireRate; protected set => _fireRate = value; }
    public float Weight { get => _weight; protected set => _weight = value; }
    public bool  TwoHanded { get => _twoHanded; }

    [Header("Components")]
    [SerializeField] protected WeaponMagazineComponent _magazineComponent;
    [SerializeField] protected WeaponGripComponent     _gripComponent;
    [SerializeField] protected WeaponBarrelComponent   _barrelComponent;
    [SerializeField] protected WeaponTrunkComponent    _trunkComponent;
    [SerializeField] protected WeaponSpringComponent   _springComponent;

    /// <summary>
    /// Вызывается в начале выстрела
    /// </summary>
    public Action OnFireStarted = delegate { };
    
    // Состояние оружия (Пустой магазин, Перезарядка, Готово)
    public WeaponState State
    {
        get => state;
        set
        {
            state = value;
            OnStateChanged(state);
        }
    }
    private WeaponState state;
    // [0,1] - действует ли на данный момент отдача
    protected float recoilOpacityUnlerped;
    protected float recoilOpacityLerped;
    // Начальная позиция
    protected Vector3 startPosition;

    // Начальное вращение
    protected Quaternion startRotation;

    /// <summary>
    /// Действует ли эффект отдачи. 0-1
    /// </summary>
    protected int recoilEffectActive;
    protected TimerCallbacker recoilEffectCallbacker;
    
    protected IEnumerator fireRoutine;

    public WeaponMagazineComponent MagazineComponent { get => _magazineComponent; protected set => _magazineComponent = value; }
    public WeaponGripComponent GripComponent { get => _gripComponent; protected set => _gripComponent = value; }
    public WeaponBarrelComponent BarrelComponent { get => _barrelComponent; protected set => _barrelComponent = value; }
    public WeaponTrunkComponent TrunkComponent { get => _trunkComponent; protected set => _trunkComponent = value; }
    public WeaponSpringComponent SpringComponent { get => _springComponent; protected set => _springComponent = value; }

    /// <summary>
    /// Изменение состояния оружия
    /// </summary>
    public event Action<WeaponState> OnStateChanged = delegate { };

    protected virtual void Awake()
    {
        CheckComponents();

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        fireRoutine = FireCoroutine();

        recoilEffectCallbacker = new TimerCallbacker(RecoilEffectTime);
        recoilEffectCallbacker.OnEmmitionEndCallback += () => recoilEffectActive = 0;
        recoilEffectCallbacker.OnResetCallback += () => recoilEffectActive = 1;
    }

    public abstract WeaponUpdateOutput UpdateComponent(WeaponUpdateInput input);

    /// <summary>
    /// Активация стрельбы. Возвращает bool: произошел выстрел или нет
    /// </summary>
    /// <returns></returns>
    protected abstract bool Fire();

    protected abstract IEnumerator FireCoroutine();

    /// <summary>
    /// Анимация смещения оружия
    /// </summary>
    protected void AnimateRecoil()
    {
        float curveValue = 0;
        if (RecoilAnimation.IsAnimating)
        {
           curveValue = RecoilAnimation.UpdateCurve(Time.deltaTime);
            
        }
        
        transform.localPosition = startPosition - new Vector3(
                0,
                0,
                curveValue * ReacoilAnimationIntensity + RecoilOpacityOffsetZ * recoilOpacityLerped);

        transform.localRotation = startRotation * Quaternion.Euler(0, 0, curveValue * RecoilOpacityRotationZ);
        //transform.RotateAround(
        //    new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z ),
        //    transform.forward,
        //    RecoilOpacityRotationZ * recoilOpacityLerped);
    }

    /// <summary>
    /// Возвращает точки попадания, до которых попал луч с учетом количества пробитий
    /// </summary>
    /// <param name="hits">Точки, которые нужно обработать</param>
    /// <param name="impactDirection">Направление, по которому направлен луч выстрела</param>
    /// <param name="output">Точки выхода, пробитие с обратной стороны пробиваемого объекта</param>
    /// <returns></returns>
    protected List<RaycastHit> ComputePenetration(IOrderedEnumerable<RaycastHit> hits, Vector3 impactDirection, out List<Vector3> output)
    {
        output = new List<Vector3>();

        // Точки в которые точно попали
        List<RaycastHit> impactPoints = new List<RaycastHit>();
       
        // Оставшиеся шаги для пробития
        int steps = PenetrationsCount;

        var pLength = GameManager.Instance.PenetrationStepLength;
        var pColl = GameManager.Instance.PenetrationCollider;

        var count = hits.Count();
        // Проходимся по всем препятствиям
        foreach (var hit in hits)
        {
            impactPoints.Add(hit);

            // Если больше нельзя ничего пробить
            if (steps == 0)
                break;

            // Расчитываем пробитие
            // j - количество затраченных шагов для пробития
            for (int j = 1; steps > 0; j++)
            {
                steps--;
                // Позиция выхода пробития
                var penetrationExit = hit.point + impactDirection * pLength * j;

                float distance;
                Vector3 direction;
                bool isOverlapped = Physics.ComputePenetration(
                    pColl, penetrationExit, transform.rotation,
                    hit.collider, hit.transform.position, hit.transform.rotation,
                    out direction, out distance);

                // Если пробили, то переходим к следующему препятствию
                // И добавляем точку выхода пробития
                if (!isOverlapped)
                {
                    // Добавляем точку выхода пробития
                    output.Add(penetrationExit);
                    break;
                }
            }
        }
        return impactPoints;
    }

    /// <summary>
    /// Сортировка точек луча по дистанции 
    /// </summary>
    /// <param name="raycastHits">Точки попадания</param>
    protected IOrderedEnumerable<RaycastHit> OrderRaycastHits(ref RaycastHit[] raycastHits)
    {
        return raycastHits.OrderBy(x => x.distance);
    }

    /// <summary>
    /// Нахождение компонента LifeComponent в точках луча
    /// </summary>
    protected List<LifeComponent> LifeComponentsFromRayhits(List<RaycastHit> hits)
    {
        var lifeList = new List<LifeComponent>();
        // Вычисляем LifeComponent в точках попадания
        for (int l = 0; l < hits.Count; l++)
        {
            var hit = hits[l];
            // Нанесение урона
            var life = hit.collider.gameObject.GetComponent<LifeComponent>();
            if (life != null)
            {
                lifeList.Add(life);
            }
        }
        return lifeList;
    }

    public virtual void StopShooting()
    {
        recoilOpacityUnlerped = 0;
        recoilOpacityLerped = 0;
        StopCoroutine(fireRoutine);
        RecoilAnimation.Reset();

        CheckMagazine();
    }
    
    /// <summary>
    /// Проверки магазина на наличи боеприпасов
    /// </summary>
    public virtual void CheckMagazine()
    {
        if (MagazineComponent.CountIsZero)
        {
            State = WeaponState.Empty;
        }
        else
        {
            State = WeaponState.Done;
        }
    }


    /// <summary>
    /// Устанавливает новый магазин и возвращает старый
    /// </summary>
    /// <param name="_MagazineComponent"></param>
    /// <returns></returns>
    public WeaponMagazineComponent WeldMagazine(WeaponMagazineComponent _MagazineComponent)
    {
        var oldComponent = MagazineComponent;
        // Присоединение нового компонента
        MagazineComponent = _MagazineComponent;
        MagazineComponent.Constructor();

        CheckMagazine();

        return oldComponent;
    }


    private void CheckComponents()
    {
        if (MagazineComponent == null)
        {
            var mag = GetComponent<WeaponMagazineComponent>();

            if (mag == null)
                throw new System.NullReferenceException();

            WeldMagazine(mag);
        }

    }

    public void SetDefaultLocalPosition(Vector3 newDefaultLocalPos)
    {
        startPosition = newDefaultLocalPos;
    }

    public void SetDefaultLocalRotation(Quaternion newDefaultLocalRotation)
    {
        startRotation = newDefaultLocalRotation;
    }

}

public struct WeaponUpdateInput
{
    // Нажата ли кнопка стрельбы
    public bool fire;
}

public struct WeaponUpdateOutput
{
    // Cостояние оружия
    public WeaponState weaponstate;
    // Сила отдачи 
    public float recoil;
    // Количество силы оттдачи
    public float recoilopacity;
}

public enum WeaponState
{
    Empty, // Пустой магазин
    Reloading, // В стадии перезарядки
    Done, // Можно стрелять
    Shooting // В стадии стрельбы
}



