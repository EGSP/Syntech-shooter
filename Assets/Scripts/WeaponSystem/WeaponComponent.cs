#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] private Transform Trunk;
    [SerializeField] private LayerMask RayMask;

    // Состояние оружия (Пустой магазин, Перезарядка, Готово)
    public WeaponState State { get; private set; }

    [Space]
    // Настройки характеристик данного оружия
    [SerializeField] private WeaponMode Mode;
    // Информация текущего магазина
    [SerializeField] private WeaponMagazineData MagazineData;

    [Space]
    [Range(0, 10)] // Как быстро будет увеличиваться отдача
    [SerializeField] private float RecoilOpacityIncrease;
    [Range(0, 10)] // Как быстро будет проходить отдача
    [SerializeField] private float RecoilOpacityDecrease;

    [Space]
    [SerializeField] private CurveAnimator RecoilAnimation;
    [Range(0, 1)]
    [SerializeField] private float ReacoilAnimationIntensity;
    

    // [0,1] - действует ли на данный момент отдача
    private float recoilOpacity;
    // Начальная позиция
    private Vector3 startPosition; 
    
    void Start()
    {
        startPosition = transform.localPosition;
        
        Reload();
    }

    // Update is called once per frame
    public WeaponUpdateOutput UpdateComponent(WeaponUpdateInput input)
    {
        var output = new WeaponUpdateOutput();
        output.weaponstate = State;
        output.recoil = Mode.RecoilForce * Convert.ToInt32(State==WeaponState.Shooting);
        output.recoilopacity = recoilOpacity;

        #region Обдумать реализацию ещё раз
        // Анимация отдачи
        AnimateRecoil();
        #endregion

        // Совершает ли игрок какое либо действие 
        var activity = input.fire | input.reload;
        if (activity == false)
        {
            recoilOpacity -= Time.deltaTime * RecoilOpacityDecrease;
            recoilOpacity = Mathf.Clamp(recoilOpacity, 0, 1);

            return output;
        }
            

        // Перезарядка преимущественнее стрельбы
        if (input.reload)
        {
            // TRY TO RELOAD
            if (State != WeaponState.Reloading)
            {
                Reload();
            }
            return output;
        }

        if (input.fire)
        {
            // TRY TO FIRE
            if(State == WeaponState.Done)
            {
                Fire();
            }
            return output;
        }

        
        throw new System.Exception("WeaponUpdate вызов должен быть завершён ранее");

    }

    // Активация стрельбы
    private void Fire()
    {
        // Начинаем стрельбу если есть боеприпасы в очереди
        if (MagazineData.Availability)
        {
            recoilOpacity += Time.deltaTime * RecoilOpacityIncrease;
            recoilOpacity = Mathf.Clamp(recoilOpacity, 0, 1);

            StartCoroutine(FireCoroutine());
        }
    }

    // Активация перезарядки
    private void Reload()
    {
        // При перезарядке вся отдача пропадает
        recoilOpacity = 0;

        bool successReload = MagazineData.Reload();

        // Если перезарядка была с помощью запасника магазина или очередь не опутсела
        if (successReload)
        {
            StartCoroutine(ReloadCoroutine());
            return;
        }
        else
        {
            // Try to find ammo in Inventory
            // If ammo exist -> int takenAmmo = Mathf.Min(ammo.Count, magazineData.SpaceCapacity);
            // ammo.Take(takenAmmo);
            // MagazineData.Fill(takenAmmo);
        }


        // Если ни одна из перезарядок не произошла
        State = WeaponState.Empty;
        
    }
    

    private IEnumerator FireCoroutine()
    {
        State = WeaponState.Shooting;
        RecoilAnimation.Play(Mode.FireRate);

        #region Action; 

        int iterations = Mathf.Clamp(Mode.BulletPerShoot, 0, MagazineData.ActiveCount); // Максимум снарядов за выстрел

        for (int j = 0; j < iterations; j++) // Запускаем нужное количество снарядов за выстрел
        {
            Vector3 sprayOffset = new Vector3(
                UnityEngine.Random.Range(-Mode.SpreadX, Mode.SpreadX),
                UnityEngine.Random.Range(-Mode.SpreadY, Mode.SpreadY),
                0);
            // Вектор направления выстрела
            Vector3 ShootForward = Quaternion.Euler(sprayOffset.y,sprayOffset.x,0)*Trunk.forward;

            // Получение пули из пула объектов
            BulletComponent bullet = PoolManager.Instance.Take(MagazineData.BulletID) as BulletComponent;

            Vector3 hitPoint;
            RaycastHit hit;
            if (Physics.Raycast(Trunk.transform.position, ShootForward, out hit, Mode.BulletFlyDistance, RayMask))
            {
                // При попадании пуля летит в точку попадания
                hitPoint = hit.point;
                print(hit.transform.name);
            }
            else
            {
                // При непопадании пуля будет лететь в точку пространства куда направлен ствол
                var pointInAir = Trunk.transform.position + ShootForward * Mode.BulletFlyDistance;
                hitPoint = pointInAir;
            }

            bullet.transform.position = Trunk.transform.position;
            bullet.transform.LookAt(hitPoint);

            bullet.Push(hitPoint,hit.normal);

            MagazineData.ActiveCount--;
        }

        #endregion

        yield return new WaitForSeconds(Mode.FireRate);

        State = WeaponState.Done;
    }

    private IEnumerator ReloadCoroutine()
    {
        State = WeaponState.Reloading;

        yield return new WaitForSeconds(Mode.ReloadTime);

        State = WeaponState.Done;
    }

    private void AnimateRecoil()
    {
        float offsetZ = 0;
        if (RecoilAnimation.IsAnimating)
        {
            offsetZ = RecoilAnimation.UpdateCurve(Time.deltaTime);

            transform.localPosition = startPosition - new Vector3(0, 0, offsetZ* ReacoilAnimationIntensity);
        }
    }

    
}

public struct WeaponUpdateInput
{
    // Нажата ли кнопка стрельбы
    public bool fire;
    // Нажата ли кнопка перезарядки
    public bool reload;
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
