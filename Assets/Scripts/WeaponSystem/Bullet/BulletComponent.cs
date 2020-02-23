#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletComponent : PooledObject
{
    [Range(1,150)]
    // Скорость полёта пули
    [SerializeField] private float Speed; 
    // Текущее время жизни
    private float lifeTime = 0;
    private Vector3 destination;
    // Нормаль попадания
    private Vector3 normal;

    
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
       

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            // Return to Pool
            InsertToPool();
        }
    }

    /// <summary>
    /// Запуск пули в нужную точку
    /// </summary>
    /// <param name="_destination"></param>
    public void Push(Vector3 _destination,Vector3 _normal)
    {
        // Время всего пути от transform.position до _destination
        var time = (_destination - transform.position).magnitude / Speed;
        
        lifeTime = time;
        
        destination = _destination;
        normal = _normal;

        
    }
}
