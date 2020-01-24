using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRifleWeapon : WeaponComponent
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override WeaponUpdateOutput UpdateComponent(WeaponUpdateInput input)
    {
        var output = new WeaponUpdateOutput();
        output.weaponstate = State;
        output.recoil = RecoilForce * Convert.ToInt32(State == WeaponState.Shooting);
        output.recoilopacity = recoilOpacity;

        #region Обдумать реализацию ещё раз
        // Анимация отдачи
        AnimateRecoil();
        #endregion

        // Совершает ли игрок какое либо действие 
        var activity = input.fire;
        if (activity == false)
        {
            recoilOpacity -= Time.deltaTime * RecoilOpacityDecrease;
            recoilOpacity = Mathf.Clamp(recoilOpacity, 0, 1);

            return output;
        }

        if (input.fire)
        {
            // TRY TO FIRE
            if (State == WeaponState.Done)
            {
                Fire();
            }
            return output;
        }

        throw new System.Exception("WeaponUpdate вызов должен быть завершён ранее");
    }

    protected override void Fire()
    {
        // Начинаем стрельбу если есть боеприпасы в очереди
        if (MagazineComponent.CountIsZero == false)
        {
            recoilOpacity += Time.deltaTime * RecoilOpacityIncrease;
            recoilOpacity = Mathf.Clamp(recoilOpacity, 0, 1);

            fireRoutine = FireCoroutine();
            StartCoroutine(fireRoutine);

            #region Action; 

            int iterations = Mathf.Clamp(BulletPerShoot, 0, MagazineComponent.Count); // Максимум снарядов за выстрел

            for (int j = 0; j < iterations; j++) // Запускаем нужное количество снарядов за выстрел
            {
                Vector3 sprayOffset = new Vector3(
                    UnityEngine.Random.Range(-SpreadX, SpreadX),
                    UnityEngine.Random.Range(-SpreadY,SpreadY),
                    0);
                // Вектор направления выстрела
                Vector3 ShootForward = Quaternion.Euler(sprayOffset.y, sprayOffset.x, 0) * Trunk.forward;

                // Получение пули из пула объектов
                BulletComponent bullet = PoolManager.Instance.Take(MagazineComponent.BulletID) as BulletComponent;

                Vector3 hitPoint;
                RaycastHit hit;
                if (Physics.Raycast(Trunk.transform.position, ShootForward, out hit, BulletFlyDistance, RayMask))
                {
                    // При попадании пуля летит в точку попадания
                    hitPoint = hit.point;

                    // Нанесение урона
                    var life = hit.collider.gameObject.GetComponent<LifeComponent>();
                    if (life != null)
                    {
                        // Доп. урон
                        for (int i = 0; i < MagazineComponent.DamageBehaviours.Length; i++)
                        {
                            var beh = MagazineComponent.DamageBehaviours[i].Clone();
                            beh.Start(life);
                            beh.OnImpactLife(new List<LifeComponent>() { life });
                            life.AddDamageBehaviour(beh);
                        }

                        // Основной урон
                        life.Hurt(Damage);
                    }

                }
                else
                {
                    // При непопадании пуля будет лететь в точку пространства куда направлен ствол
                    var pointInAir = Trunk.transform.position + ShootForward * BulletFlyDistance;
                    hitPoint = pointInAir;
                }

                bullet.transform.position = Trunk.transform.position;
                bullet.transform.LookAt(hitPoint);

                bullet.Push(hitPoint, hit.normal);

                MagazineComponent.Count--;
            }

            #endregion
        }
        else
        {
            print("Magazine not availabile to shoot");
        }
    }

    protected override IEnumerator FireCoroutine()
    {
        State = WeaponState.Shooting;
        RecoilAnimation.Play(FireRate);

        yield return new WaitForSeconds(FireRate);

        CheckMagazine();
    }
}
