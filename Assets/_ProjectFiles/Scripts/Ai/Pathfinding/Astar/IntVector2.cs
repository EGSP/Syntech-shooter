using System;

[System.Serializable]
public struct IntVector2
{
    public IntVector2(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public int x;
    public int y;

    public static IntVector2 one { get => new IntVector2(1, 1); }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        => new IntVector2(a.x + b.x, a.y + b.y);

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        => new IntVector2(a.x - b.x, a.y - b.y);

    public static bool operator ==(IntVector2 a,IntVector2 b)
    {
        if (a.x == b.x && a.y == b.y)
            return true;

        return false;
    }

    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        if (a.x == b.x && a.y == b.y)
            return false;

        return true;
    }

    public override string ToString()
    {
        return "(" + x.ToString() + " ; " + y.ToString() + ")";
    }


  
}

