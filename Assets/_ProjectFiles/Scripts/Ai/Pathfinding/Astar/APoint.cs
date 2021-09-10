using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public struct APoint
{
    public APoint(IntVector2 _pos, bool _IsWall = false)
    {
        pos = _pos;
        IsWall = _IsWall;
    }

    public IntVector2 pos { get; private set; }

    public bool IsWall;
    
    public static bool operator ==(APoint a, APoint b)
    {
        if (a.pos == b.pos)
            return true;

        return false;
    }

    public static bool operator !=(APoint a, APoint b)
    {
        if (a.pos == b.pos)
            return false;

        return true;
    }

    public override string ToString()
    {
        return pos.ToString() + " : " + IsWall.ToString();
    }

}

