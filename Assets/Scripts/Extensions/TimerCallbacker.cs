using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Таймер, который вызывает событие по окончанию времени
/// </summary>
public class TimerCallbacker
{
    public delegate void TimerCallback();

    public event TimerCallback OnEmmitionEndCallback = delegate { };
    public event TimerCallback OnResetCallback = delegate { };

    public TimerCallbacker(float _EmmitTime)
    {
        EmmitTime = _EmmitTime;
        emmitTime = EmmitTime;
        
        IsActive = true;
    }

    // Время жизни счетчика
    public float EmmitTime { get; private set; }
    private float emmitTime;
    
    // Работает ли счетчик
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновление состояния таймера
    /// </summary>
    /// <param name="deltaTime"> Прошедшее время с последнего обновления </param>
    public void Update(float deltaTime)
    {
        if (IsActive == false)
            return;

        emmitTime -= deltaTime;
        

        if (emmitTime < 0)
        {
            IsActive = false;

            // Вызов события окончания счетчика
            OnEmmitionEndCallback();
        }
    }

    /// <summary>
    /// Сброс таймера до состояния начала отсчета
    /// </summary>
    public void Reset()
    {
        emmitTime = EmmitTime;

        IsActive = true;

        OnResetCallback();
    }

    /// <summary>
    /// Изменение времени работы таймера
    /// </summary>
    public void ChangeTime(float _EmmitTime)
    {
        EmmitTime = _EmmitTime;
        emmitTime = EmmitTime;
    }
}

