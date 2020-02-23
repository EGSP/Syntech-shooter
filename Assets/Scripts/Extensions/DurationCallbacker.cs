using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Таймер, который вызывает события через интервалы времени до окончания работы
/// </summary>
public class DurationCallbacker
{
    public delegate void DurationCallback();

    // delegate { } - первый элемент события не будет равен null 
    public event DurationCallback OnDurationCallback = delegate { };
    public event DurationCallback OnEmmitionEndCallback = delegate { };

    public DurationCallbacker(float _EmmitTime, float _Duration)
    {
        EmmitTime = _EmmitTime;
        emmitTime = EmmitTime;

        Duration = _Duration;
        duration = Duration;

        IsActive = true;
    }
    
    // Время жизни счетчика
    private readonly float EmmitTime;
    private float emmitTime;

    // Интервал срабатывания
    private readonly float Duration;
    private float duration;

    // Работает ли счетчик
    private bool IsActive;

    /// <summary>
    /// Обновление состояния таймера
    /// </summary>
    /// <param name="deltaTime"> Прошедшее время с последнего обновления </param>
    public void Update(float deltaTime)
    {
        if (IsActive == false)
            return;

        emmitTime -= deltaTime;

        duration -= deltaTime;
        if (duration < 0)
        {
            duration = Duration;

            // Вызов события по интервалу счетчика
            OnDurationCallback();
        }
        
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
        duration = Duration;

        IsActive = true;
    }
}

