#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Linq;

public abstract class WeaponComponent : MonoBehaviour
{
    [SerializeField] protected Transform Trunk;
    [SerializeField] protected LayerMask RayMask;

    [SerializeField] protected float _damage;
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

    [Range(0, 5)]
    // Длинна шага пробития
    [SerializeField] protected float PenetrationStepLength;
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
    [Space]
    [SerializeField] protected CurveAnimator RecoilAnimation;
    [Range(0, 1)]
    [SerializeField] protected float ReacoilAnimationIntensity;


    // Свойства
    public float Damage { get => _damage; protected set => _damage = value; }
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
    
    // Состояние оружия (Пустой магазин, Перезарядка, Готово)
    public WeaponState State { get; set; }

    // [0,1] - действует ли на данный момент отдача
    protected float recoilOpacity;
    // Начальная позиция
    protected Vector3 startPosition;
    
    protected IEnumerator fireRoutine;

    public WeaponMagazineComponent MagazineComponent { get => _magazineComponent; protected set => _magazineComponent = value; }
    public WeaponGripComponent GripComponent { get => _gripComponent; protected set => _gripComponent = value; }
    public WeaponBarrelComponent BarrelComponent { get => _barrelComponent; protected set => _barrelComponent = value; }
    public WeaponTrunkComponent TrunkComponent { get => _trunkComponent; protected set => _trunkComponent = value; }
    public WeaponSpringComponent SpringComponent { get => _springComponent; protected set => _springComponent = value; }
    

    protected virtual void Awake()
    {
        CheckComponents();

        startPosition = transform.localPosition;

        fireRoutine = FireCoroutine();
    }

    public abstract WeaponUpdateOutput UpdateComponent(WeaponUpdateInput input);

    // Активация стрельбы
    protected abstract void Fire();

    protected abstract IEnumerator FireCoroutine();

    protected void AnimateRecoil()
    {
        float offsetZ = 0;
        if (RecoilAnimation.IsAnimating)
        {
            offsetZ = RecoilAnimation.UpdateCurve(Time.deltaTime);

            transform.localPosition = startPosition - new Vector3(0, 0, offsetZ * ReacoilAnimationIntensity);
        }
    }


    public virtual void StopShooting()
    {
        recoilOpacity = 0;
        StopCoroutine(fireRoutine);
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



//void Awake()
//{
//    CheckComponents();

//    startPosition = transform.localPosition;

//    fireRoutine = FireCoroutine();

//    CheckMagazine();
//}

//// Update is called once per frame
//public WeaponUpdateOutput UpdateComponent(WeaponUpdateInput input)
//{
//    var output = new WeaponUpdateOutput();
//    output.weaponstate = State;
//    output.recoil = Mode.RecoilForce * Convert.ToInt32(State == WeaponState.Shooting);
//    output.recoilopacity = recoilOpacity;

//    #region Обдумать реализацию ещё раз
//    // Анимация отдачи
//    AnimateRecoil();
//    #endregion

//    // Совершает ли игрок какое либо действие 
//    var activity = input.fire;
//    if (activity == false)
//    {
//        recoilOpacity -= Time.deltaTime * RecoilOpacityDecrease;
//        recoilOpacity = Mathf.Clamp(recoilOpacity, 0, 1);

//        return output;
//    }

//    if (input.fire)
//    {
//        // TRY TO FIRE
//        if (State == WeaponState.Done)
//        {
//            Fire();
//        }
//        return output;
//    }

//    throw new System.Exception("WeaponUpdate вызов должен быть завершён ранее");
//}

//// Активация стрельбы
//private void Fire()
//{
//    // Начинаем стрельбу если есть боеприпасы в очереди
//    if (MagazineData.CountIsZero == false)
//    {
//        recoilOpacity += Time.deltaTime * RecoilOpacityIncrease;
//        recoilOpacity = Mathf.Clamp(recoilOpacity, 0, 1);

//        fireRoutine = FireCoroutine();
//        StartCoroutine(fireRoutine);

//        #region Action; 

//        int iterations = Mathf.Clamp(Mode.BulletPerShoot, 0, MagazineData.Count); // Максимум снарядов за выстрел

//        for (int j = 0; j < iterations; j++) // Запускаем нужное количество снарядов за выстрел
//        {
//            Vector3 sprayOffset = new Vector3(
//                UnityEngine.Random.Range(-Mode.SpreadX, Mode.SpreadX),
//                UnityEngine.Random.Range(-Mode.SpreadY, Mode.SpreadY),
//                0);
//            // Вектор направления выстрела
//            Vector3 ShootForward = Quaternion.Euler(sprayOffset.y, sprayOffset.x, 0) * Trunk.forward;

//            // Получение пули из пула объектов
//            BulletComponent bullet = PoolManager.Instance.Take(MagazineData.BulletID) as BulletComponent;

//            Vector3 hitPoint;
//            RaycastHit hit;
//            if (Physics.Raycast(Trunk.transform.position, ShootForward, out hit, Mode.BulletFlyDistance, RayMask))
//            {
//                // При попадании пуля летит в точку попадания
//                hitPoint = hit.point;
//                print(hit.transform.name);
//            }
//            else
//            {
//                // При непопадании пуля будет лететь в точку пространства куда направлен ствол
//                var pointInAir = Trunk.transform.position + ShootForward * Mode.BulletFlyDistance;
//                hitPoint = pointInAir;
//            }

//            bullet.transform.position = Trunk.transform.position;
//            bullet.transform.LookAt(hitPoint);

//            bullet.Push(hitPoint, hit.normal);

//            MagazineData.Count--;
//        }

//        #endregion
//    }
//    else
//    {
//        print("Magazine not availabile to shoot");
//    }
//}

///// <summary>
///// Проверки магазина на наличи боеприпасов
///// </summary>
//public void CheckMagazine()
//{
//    if (MagazineData.CountIsZero)
//    {
//        State = WeaponState.Empty;
//    }
//    else
//    {
//        State = WeaponState.Done;
//    }
//}


//private IEnumerator FireCoroutine()
//{
//    State = WeaponState.Shooting;
//    RecoilAnimation.Play(Mode.FireRate);

//    yield return new WaitForSeconds(Mode.FireRate);

//    CheckMagazine();
//}

//private void AnimateRecoil()
//{
//    float offsetZ = 0;
//    if (RecoilAnimation.IsAnimating)
//    {
//        offsetZ = RecoilAnimation.UpdateCurve(Time.deltaTime);

//        transform.localPosition = startPosition - new Vector3(0, 0, offsetZ * ReacoilAnimationIntensity);
//    }
//}


//public void StopShooting()
//{
//    recoilOpacity = 0;
//    StopCoroutine(fireRoutine);
//}
///// <summary>
///// Устанавливает новый магазин и возвращает старый
///// </summary>
///// <param name="_MagazineComponent"></param>
///// <returns></returns>
//public WeaponMagazineComponent WeldMagazine(WeaponMagazineComponent _MagazineComponent)
//{
//    var oldComponent = MagazineComponent;
//    // Присоединение нового компонента
//    MagazineComponent = _MagazineComponent;
//    MagazineData = _MagazineComponent.GetData();

//    return oldComponent;
//}


//private void CheckComponents()
//{
//    if (MagazineComponent == null)
//    {
//        var mag = GetComponent<WeaponMagazineComponent>();

//        if (mag == null)
//            throw new System.NullReferenceException();

//        WeldMagazine(mag);
//    }

//}

//}

//public struct WeaponUpdateInput
//{
//    // Нажата ли кнопка стрельбы
//    public bool fire;
//}

//public struct WeaponUpdateOutput
//{
//    // Cостояние оружия
//    public WeaponState weaponstate;
//    // Сила отдачи 
//    public float recoil;
//    // Количество силы оттдачи
//    public float recoilopacity;
//}

//public enum WeaponState
//{
//    Empty, // Пустой магазин
//    Reloading, // В стадии перезарядки
//    Done, // Можно стрелять
//    Shooting // В стадии стрельбы
//}