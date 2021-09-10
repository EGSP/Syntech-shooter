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
    // Максимальный угол подъёма
    [SerializeField] private float MaxStepAngle; 
    // Длинна при которой нельзя идти в сторону направления
    [SerializeField] private float DampLength;
    [Range(0,1)]
    // Максимальное значение DotProduct при котором не будет ограничения по движению
    [SerializeField] private float DampStep;

    [Space]
    // Слой земли
    [SerializeField] private LayerMask layerMask;
    // Дистанция пуска луча 
    [SerializeField] private float rayDist;

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

    
    public MoveSystemOutput Update(MoveSystemInput IN)
    {
        // Новое вращение тела
        newBodyRotation *= Quaternion.Euler(0, IN.rotationY, 0);

        // Поворот тела
        rig.MoveRotation( Quaternion.Lerp(Body.rotation, newBodyRotation, BodyLerp));

        // Перемещение физического тела
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
        

        // Вектор суммарного направления клавиатуры
        Vector3 dir = (v * transform.forward + h * transform.right).normalized;
        var relativePos = transform.position + GetBodyBones().BodyCenterOffset;
        RaycastHit hit;
        // Если впереди есть стена
        // GroundLayer = 9
        if (Physics.Raycast(relativePos, dir, out hit, DampLength,layerMask))
        {
            
            var dot = Vector3.Dot(hit.normal, (relativePos - hit.point).normalized);
            if (dot > DampStep)
                velocity = 0;
            //Vector3 forpl = Vector3.ProjectOnPlane(transform.forward, hit.normal);
        }

       
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
                    rig.MovePosition(rig.position + forpl.normalized * velocity);

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

                rig.position += (WorldSettings.Instance.GravityDirection * WorldSettings.Instance.Gravity) * Time.deltaTime + dir * velocity;

                break;
        }
    }

    /// <summary>
    /// Длинна луча при которой нельзя идти в сторону направления
    /// </summary>
    public float GetDampLength()
    {
        return DampLength;
    }

    public BodyBones GetBodyBones()
    {
        return BodyBones;
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
