public class LinkedAPoint
{
    public LinkedAPoint(IntVector2 _pos, bool _IsWall = false)
    {
        pos = _pos;
        IsWall = _IsWall;
    }

    public IntVector2 pos { get; private set; }

    public bool IsWall;
    
    // Соседи
    public LinkedAPoint Left { get; set; }
    public LinkedAPoint Right { get; set; }
    public LinkedAPoint Top { get; set; }
    public LinkedAPoint Bottom { get; set; }

   

    public override string ToString()
    {
        return pos.ToString() + " : " + IsWall.ToString();
    }
}

