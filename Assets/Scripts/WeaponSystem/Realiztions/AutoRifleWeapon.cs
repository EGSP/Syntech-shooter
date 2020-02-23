﻿using System;
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
                ShootForward.Normalize();
               
                var raycastAllHits = Physics.RaycastAll(Trunk.transform.position, ShootForward, BulletFlyDistance, GameManager.Instance.ShootRayMask);
                OrderRaycastHits(ref raycastAllHits);

                // Точки выхода пробития
                List<Vector3> outputs;
                var acceptedHits = ComputePenetration(raycastAllHits, ShootForward, out outputs);
                // Получение списка LifeComponent
                var lifeList = LifeComponentsFromRayhits(acceptedHits);

                // Наносим базовый урон
                for(int i = 0; i < lifeList.Count;i++)
                {
                    lifeList[i].Hurt(Damage);
                }

                // Эффекты урона магазина
                for (int i = 0; i < MagazineComponent.DamageBehaviours.Length; i++)
                {
                    var beh = MagazineComponent.DamageBehaviours[i].Clone();

                    if (lifeList.Count != 0)
                    {
                        beh.Start(lifeList[0], GameManager.Instance.LifeLayerMask);
                        beh.OnImpactLife(lifeList);
                    }

                    // Регистрация попаданий в стены
                    beh.OnImpactAll(acceptedHits);
                    // Регистрация пробитий стен
                    beh.OnPenetrate(outputs, ShootForward);
                }

                // Спавн эффектов
                for (int i = 0; i < acceptedHits.Count; i++)
                {
                    var hit = acceptedHits[i];
                    var impact = EffectManager.Instance.Take("IS");

                    impact.PlayEffect(hit.point, hit.normal);
                }

                // Получение пули из пула объектов
                BulletComponent bullet = PoolManager.Instance.Take(MagazineComponent.BulletID) as BulletComponent;
                // Спавн пули
                if (acceptedHits.Count != 0)
                {
                    var hit = acceptedHits[acceptedHits.Count - 1];
                    bullet.transform.position = Trunk.transform.position;
                    bullet.transform.LookAt(hit.point);

                    bullet.Push(hit.point, hit.normal);
                }
                else
                {
                    bullet.transform.position = Trunk.transform.position;
                    bullet.transform.LookAt(ShootForward);

                    bullet.Push(Trunk.transform.position + ShootForward * BulletFlyDistance, ShootForward);
                }

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
