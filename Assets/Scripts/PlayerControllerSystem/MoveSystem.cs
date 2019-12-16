#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveSystem 
{
    [Header("Transform")]
    public Transform Body;
    [Header("Smoothness")]
    [Range(0, 1)]
    [SerializeField] private float BodyLerp;

    private Quaternion newBodyRotation;

    [Header("BodySetup")]
    [SerializeField] private BodyBones BodyBones;

    [Space]
    [Header("MoveSettings")]
    [SerializeField] private float HorizontalSpeed;
    [SerializeField] private float VerticalSpeed;
    [SerializeField] private float MaxStepAngle; // Максимальный уголь подъёма
    [SerializeField] private float CrouchSpeedModifier; // Множитель скорости ходьбы при приседании
    [Tooltip("Скорость смены положения")]
    [SerializeField] private float CrouchAlignSpeed;    // Скорость смены положения
    [Space]
    [SerializeField] private LayerMask layerMask; // Слой земли
    [SerializeField] private float rayDist; // Дистанция пуска луча 

    private AreaState areaState;
    private MoveState moveState;
    private Rigidbody rig;


    // Start is called before the first frame update
    public bool Start(Rigidbody _rig)
    {
        rig = _rig;

        newBodyRotation = Body.rotation;

        return true;
    }


    // Update is called once per frame
    public MoveSystemOutput Update(MoveSystemInput IN)
    {
        // Поворот тела
        newBodyRotation *= Quaternion.Euler(0, IN.rotationY, 0); // Новое вращение тела
        Body.rotation = Quaternion.Lerp(Body.rotation, newBodyRotation, BodyLerp);

        Move(IN.horizontalInput, IN.verticalInput, Body, IN.speedModifier);

        return new MoveSystemOutput()
        {
            bodyRotation = Body.rotation,
            bodyLocalRotation = Body.localRotation
        };
    }

    public void Move(float h, float v,Transform transform, Vector2 speedModifier)
    {
        // Знаменатель среднего значения
        float hABS = Mathf.Abs(h);
        float vABS = Mathf.Abs(v);
        float Znam = hABS / 1 + vABS / 1;

        float velocity = 0;

        if (Znam != 0)
        {
            // Среднее значение скоростей
            velocity = ((hABS * HorizontalSpeed*speedModifier.x + vABS * VerticalSpeed*speedModifier.y) / Znam) * Time.deltaTime;
        }

        if (moveState == MoveState.Crouch)
            velocity *= CrouchSpeedModifier;

        // Вектор суммарного направления клавиатуры
        Vector3 dir = (v * transform.forward + h * transform.right).normalized;

        RaycastHit hit;
        switch (areaState)
        {
            // Если земля под ногами
            case AreaState.Ground:
                if (Physics.Raycast(transform.position, transform.up * -1, out hit, rayDist, layerMask))
                {
                    if ((hit.point - transform.position).magnitude > 1f)
                    {
                        areaState = AreaState.Air;
                        break;
                    }
                    //// Вектор всегда будет устремлён по вниз склону
                    //Vector3 surfParall = hit.point - transform.position - hit.normal * Vector3.Dot(hit.point - transform.position, hit.normal);
                    ////print(Vector3.Dot((surfParall + hit.point).normalized, hit.normal));

                    // Forward projection
                    Vector3 forpl = Vector3.ProjectOnPlane(dir, hit.normal);
                    // -1 В гору, 0 - прямо, 1 - спуск 
                    // print(Vector3.Dot(transform.forward, hit.normal));

                    float angle = Vector3.Dot(transform.forward, hit.normal);

                    //if(angle<0 && Mathf.Abs(angle) > (MaxStepAngle / 90))
                    //{
                    //    // Упёрлись в гору
                    //    return;
                    //}

                    rig.position = new Vector3(rig.position.x, hit.point.y + BodyBones.CentreToGround, rig.position.z);
                    //rig.MovePosition(transform.position+forpl.normalized * velocity);
                    rig.position += forpl.normalized * velocity;

                }
                else
                {
                    areaState = AreaState.Air;
                }

                break;

            // Если игрок в воздухе
            case AreaState.Air:
                if (Physics.Raycast(transform.position, transform.up * -1, out hit, rayDist, layerMask))
                {
                    // Если достаточно близко к поверхности
                    if ((hit.point - transform.position).magnitude < 1f)
                    {
                        areaState = AreaState.Ground;
                        break;
                    }

                }

                rig.position += (WorldSettings.GravityDirection * WorldSettings.Gravity) * Time.deltaTime + dir * velocity;

                break;
        }
    }


    public enum AreaState
    {
        Ground,
        Air,
    }
    public enum MoveState
    {
        Walk = 0,
        Crouch = 1
    }
}


public class MoveSystemInput
{
    public float rotationY;
    public float horizontalInput;
    public float verticalInput;

    public Vector2 speedModifier = Vector2.one;
}

public class MoveSystemOutput
{
    public Quaternion bodyRotation;
    public Quaternion bodyLocalRotation;
}
