#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using UnityEngine;

[System.Serializable]
public class CurveAnimator
{
    [SerializeField] private AnimationCurve Curve; // Значение смещения при отдаче

    public bool IsAnimating { get; private set; } // Действует ли сейчас анимация
    public float CurvePoint { get; private set; } // Текущая позиция по оси Х 

    private float playTime; // Время за которое нужно проиграть анимацию

    /// <summary>
    /// Скорость проигрывания анимации
    /// </summary>
    private float playSpeed;

    public void Play(float _playTime)
    {
        playTime = _playTime;
        CurvePoint = 0;
        IsAnimating = true;

        playSpeed = 1 / playTime;
    }

    public float UpdateCurve(float deltaTime)
    {
        CurvePoint += playSpeed * deltaTime;

        if(CurvePoint >= 1)
        {
            IsAnimating = false;
        }

        return Curve.Evaluate(CurvePoint);
    }

    /// <summary>
    /// Получение числа по точке
    /// </summary>
    /// <param name="point">Точка на кривой</param>
    public float GetCurveValue(float point)
    {
        return Curve.Evaluate(point);
    }

    /// <summary>
    /// Переходит в начальное состояние
    /// </summary>
    public void Reset()
    {
        CurvePoint = 0;
        IsAnimating = false;
    }


}