#pragma warning disable IDE0044 // Добавить модификатор "только для чтения"
using UnityEngine;

[System.Serializable]
public class CurveAnimator
{
    [SerializeField] private AnimationCurve Curve; // Значение смещения при отдаче

    public bool IsAnimating { get; private set; } // Действует ли сейчас анимация
    private float curvePoint; // Текущая позиция по оси Х 

    private float playTime; // Время за которое нужно проиграть анимацию

    public void Play(float _playTime)
    {
        playTime = _playTime;
        curvePoint = 0;
        IsAnimating = true;
    }

    public float UpdateCurve(float deltaTime)
    {
        curvePoint += (deltaTime / playTime);

        if(curvePoint >= 1)
        {
            IsAnimating = false;
        }

        return Curve.Evaluate(curvePoint);
    }

    /// <summary>
    /// Переходит в начальное состояние
    /// </summary>
    public void Reset()
    {
        curvePoint = 0;
        IsAnimating = false;
    }


}