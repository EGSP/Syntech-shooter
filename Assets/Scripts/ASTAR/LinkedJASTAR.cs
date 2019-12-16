using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LinkedJASTAR
{
    public LinkedAPoint[][] Grid;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public void GenerateGrid(int _Width, int _Height)
    {
        Width = _Width;
        Height = _Height;
        Grid = new LinkedAPoint[Height][];

        for (int y = 0; y < Grid.Length; y++)
        {
            Grid[y] = new LinkedAPoint[Width];
            for(int x = 0; x < Grid[y].Length; x++)
            {
                var point = new LinkedAPoint(new IntVector2(x, y));

                if (x > 0)
                {
                    var gridPoint = Grid[y][x - 1];
                    point.Left = gridPoint;
                    gridPoint.Right = point;
                }

                if (y > 0)
                {
                    var gridPoint = Grid[y-1][x];
                    point.Bottom = gridPoint;
                    gridPoint.Top = point;
                }

                Grid[y][x] = point;
            }
        }
    }

    public void DeleteGrid()
    {
        Grid = null;
    }

    public List<LinkedAPoint> FindPath(LinkedAPoint start, LinkedAPoint goal)
    {
        var closedSet = new List<LinkedPathNode>();
        var openSet = new List<LinkedPathNode>();

        var startNode = new LinkedPathNode()
        {
            point = start,
            ComeFrom = null,
            PathLengthFromStart = 0,
            HeuristicPathLength = GetHeuristicPathLength(start, goal)
        };
        openSet.Add(startNode);

        int exception = 0;
        while (openSet.Count > 0)
        {
            exception++;
            if (exception > 10000)
                throw new System.Exception("Clamp while exception");

            // Заменить на нахождение минимального
            var currentNode = openSet.OrderBy(node => node.FullPathLength).First();

            if (currentNode.point.pos == goal.pos)
            {
                var PATH = GetPathForNode(currentNode);
                PATH.Reverse();
                return PATH;
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            var neighbours = GetNeighbours(currentNode, goal);

            for (int i = 0; i < neighbours.Count; i++)
            {
                var node = neighbours[i];

                // Если данная нода была рассмотрена
                if (closedSet.Count(x => x.point == node.point) > 0)
                    continue;

                // Содержится ли сосед в открытом списке
                var openNode = openSet.FirstOrDefault(x => x.point == node.point);

                // Добавляем если не содержится
                if (openNode == null)
                {
                    openSet.Add(node);
                }
                else
                {
                    if (openNode.PathLengthFromStart > node.PathLengthFromStart)
                    {
                        openNode.ComeFrom = currentNode;
                        openNode.PathLengthFromStart = node.PathLengthFromStart;
                    }
                }
            }

        }

        // Пути не существует
        return null;
    }

    public List<LinkedPathNode> GetNeighbours(LinkedPathNode node, LinkedAPoint goal)
    {
        var result = new List<LinkedPathNode>();

        LinkedAPoint[] Neighbours = new LinkedAPoint[4];
        Neighbours[0] = node.point.Left;
        Neighbours[1] = node.point.Right;
        Neighbours[2] = node.point.Top;
        Neighbours[3] = node.point.Bottom;

        for (int i = 0; i < Neighbours.Length; i++)
        {
            var point = Neighbours[i];

            if (point == null)
                continue;
            
            if (point.IsWall)
                continue;


            LinkedPathNode neighbour = new LinkedPathNode()
            {
                point = point,
                ComeFrom = node,
                PathLengthFromStart = node.PathLengthFromStart + GetDistanceBetweenNeighbours(),
                HeuristicPathLength = GetHeuristicPathLength(point, goal)
            };

            result.Add(neighbour);
        }

        return result;
    }

    public List<LinkedAPoint> GetPathForNode(LinkedPathNode node)
    {
        List<LinkedAPoint> path = new List<LinkedAPoint>();

        while (node != null)
        {
            path.Add(node.point);

            node = node.ComeFrom;
        }

        return path;

    }

    public static int GetHeuristicPathLength(LinkedAPoint from, LinkedAPoint to)
    {
        return System.Math.Abs(from.pos.x - to.pos.x) + System.Math.Abs(from.pos.y - to.pos.y);
    }

    public static int GetDistanceBetweenNeighbours()
    {
        return 1;
    }

}


public class LinkedPathNode
{
    // Точка с координатами на сетке
    public LinkedAPoint point { get; set; }

    // Точка из которой пришли
    public LinkedPathNode ComeFrom { get; set; }

    // Длинна пути от старта "G"
    public int PathLengthFromStart { get; set; }

    // Примерное расстояние до цели "H"
    public int HeuristicPathLength { get; set; }

    // Ожидаемое полное расстояние до цели "F"
    public int FullPathLength
        => PathLengthFromStart + HeuristicPathLength;

    public override string ToString()
    {
        return "PathN: " + point.ToString();
    }
}