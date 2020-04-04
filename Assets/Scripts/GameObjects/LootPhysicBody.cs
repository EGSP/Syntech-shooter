using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LootPhysicBody : PooledObject, IPhysicBody
{
    /// <summary>
    /// Слой объектов, с которыми работают столкновения
    /// </summary>
    [SerializeField] private LayerMask CollisionLayer;

    /// <summary>
    /// Масса объекта
    /// </summary>
    [SerializeField] private float Mass = 1;

    /// <summary>
    /// Сопротивление при скольжении по плоскости XZ (объект имеет 0 скорость по Y)
    /// </summary>
    [Range(0,1)]
    [SerializeField] private float XZDrag;

    /// <summary>
    /// Радиус объекта
    /// </summary>
    [SerializeField] private float CollisionRadius;

    /// <summary>
    /// Вектор скорости объекта
    /// </summary>
    public Vector3 Velocity
    {
        get => velocity;
        set => velocity = Vector3.ClampMagnitude(value, ws.MaxVelocityMagnitude);
    }
    private Vector3 velocity;

    /// <summary>
    /// Вектор скорости в плоскости XZ
    /// </summary>
    public Vector3 XZvelocity { get => new Vector3(Velocity.x, 0, Velocity.z); }
    
    /// <summary>
    /// Мировые настройки сцены
    /// </summary>
    private WorldSettings ws;

    protected override void Awake()
    {
        ws = WorldSettings.Instance;

        Velocity = Vector3.zero;
    }

    protected virtual void Update()
    {
        float deltaTime = Time.deltaTime;

        // Учитываем гравитацию сцены и массу объекта
        Velocity += ws.GravityVector * Mass * deltaTime;

        // Проверка поверхности под объектом
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.up * -1,out hit, CollisionRadius, CollisionLayer))
        {
            // Обнуляем скорость по Y
            var temp = Velocity;
            temp.y = 0;
            Velocity = temp;

            Velocity = Vector3.Lerp(Velocity, Vector3.zero, XZDrag);
        }

        // Проверка поверхности в направлении движения
        if(Physics.Raycast(transform.position, XZvelocity.normalized, out hit, CollisionRadius, CollisionLayer))
        {
            // Обнуляем скорость по XZ
            var temp = Velocity;
            temp.z = 0;
            temp.x = 0;
            Velocity = temp;
        }

        // Меняем положение
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Velocity.normalized, Velocity.magnitude * deltaTime);
    }

    /// <summary>
    /// Добавление импульса вектору скорости объекта
    /// </summary>
    /// <param name="direction">Направление импульса</param>
    /// <param name="magnitude">Длинна импульса</param>
    public void AddImpulse(Vector3 direction, float magnitude)
    {
        Velocity += direction * magnitude;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, CollisionRadius);
    }
}

/// <summary>
/// Тело взаимодействующее с физикой
/// </summary>
public interface IPhysicBody
{
    /// <summary>
    /// Добавление импульса вектору скорости объекта
    /// </summary>
    /// <param name="direction">Направление импульса</param>
    /// <param name="magnitude">Длинна импульса</param>
    void AddImpulse(Vector3 direction, float magnitude);
}