using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Extensions
{
    /// <summary>
    /// If f > 180, then returns f -= 360
    /// </summary>
    /// <returns>Fixed angle of f</returns>
    public static float FixAngle(float f)
    {
        if (f > 180)
            f -= 360;

        return f;
    }

    /// <summary>
    /// Returns middle value of ab
    /// </summary>
    /// <returns>Middle value</returns>
    public static float Middle(float a,float b)
    {
        
        return (a + b) / 2;
    }


}

/// <summary>
/// Класс расширений для работы с интерфейсом
/// </summary>
public static class UIExtension
{
    /// <summary>
    /// Самый эффективный способ округления числа и перевода его в строку
    /// </summary>
    /// <param name="f">Число</param>
    /// <param name="digits">Количество знаков после запятой</param>
    /// <returns></returns>
    public static string ToString(this float f, int digits)
    {
        return System.Math.Round(f, digits).ToString();
    }
}