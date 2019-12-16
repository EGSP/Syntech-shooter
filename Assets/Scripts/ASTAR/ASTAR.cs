﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class ASTAR
{
    
    public APoint[,] PointGrid { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
   
    public void GenerateGrid(IntVector2 widthHeight)
    {
        PointGrid = new APoint[widthHeight.x, widthHeight.y];
        Width = widthHeight.x;
        Height = widthHeight.y;

        for(int x = 0; x < widthHeight.x; x++)
        {
            for (int y = 0; y < widthHeight.y; y++) {

                PointGrid[x, y] = new APoint(new IntVector2(x, y));
            }
        }
    }

    public void DeleteGrid()
    {
        PointGrid = null;
    }

    public List<APoint> FindPath(APoint start, APoint goal)
    {
        var closedSet = new List<PathNode>();
        var openSet = new List<PathNode>();

        var startNode= new PathNode()
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

            for(int i = 0; i < neighbours.Count; i++)
            {
                var node = neighbours[i];

                // Если данная нода была рассмотрена
                if (closedSet.Count(x => x.point == node.point) > 0)
                    continue;

                // Содержится ли сосед в открытом списке
                var openNode = openSet.FirstOrDefault(x => x.point == node.point);

                // Добавляем если не содержится
                if(openNode == null)
                {
                    openSet.Add(node);
                }
                else
                {
                    if (openNode.PathLengthFromStart > node.PathLengthFromStart)
                    {
                        Debug.Log("check");
                        openNode.ComeFrom = currentNode;
                        openNode.PathLengthFromStart = node.PathLengthFromStart;
                    }
                }
            }
            
        }

        // Пути не существует
        return null;
    }

    public List<List<APoint>> FindPathParallel(APoint start, APoint goal)
    {
        var startNode = new PathNode()
        {
            point = start,
            ComeFrom = null,
            PathLengthFromStart = 0,
            HeuristicPathLength = GetHeuristicPathLength(start, goal)
        };

        // Получаем соседей стартовой точки
        var startNeighbours = GetNeighbours(startNode, goal);
        // Таски поиска маршрута
        var searchTasks = new Task<List<APoint>>[startNeighbours.Count];


        // Запускаем поиски пути с позиций соседей
        for (int i = 0; i < searchTasks.Length; i++)
        {
            var j = i;
            searchTasks[j] = Task.Run(() => FindPath(startNeighbours[j].point, goal));
        }
        
        // Ожидаем завершения всех задач
        Task.WaitAll(searchTasks);
        
        var allPaths = new List<List<APoint>>();

        for (int r = 0; r < searchTasks.Length; r++)
        {
            var result = searchTasks[r].Result;

            // Если таск нашёл путь
            if (result != null)
            {
                result.Insert(0, startNode.point);
                allPaths.Add(result);
            }
        }

        return allPaths;

    }

    public List<APoint> GetPathForNode(PathNode node)
    {
        List<APoint> path = new List<APoint>();

        while(node != null)
        {
            path.Add(node.point);

            node = node.ComeFrom;
        }

        return path;

    }

    public List<PathNode> GetNeighbours(PathNode node,APoint goal)
    {
        var result = new List<PathNode>();

        APoint[] neighbourPoints = new APoint[4];

        neighbourPoints[0] = new APoint(node.point.pos + new IntVector2(1, 0));
        neighbourPoints[1] = new APoint(node.point.pos + new IntVector2(-1, 0));

        neighbourPoints[2] = new APoint(node.point.pos + new IntVector2(0, 1));
        neighbourPoints[3] = new APoint(node.point.pos + new IntVector2(0, -1));

        for(int i = 0; i < neighbourPoints.Length; i++)
        {
            var point = neighbourPoints[i];

            if (point.pos.x < 0 || point.pos.x >= Width)
                continue;

            if (point.pos.y < 0 || point.pos.y >= Height)
                continue;

            if (PointGrid[point.pos.x, point.pos.y].IsWall)
            {
                continue;
            }
                

            PathNode neighbour = new PathNode()
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


    public static int GetHeuristicPathLength(APoint from,APoint to)
    {
        return System.Math.Abs(from.pos.x - to.pos.x) + System.Math.Abs(from.pos.y - to.pos.y);
    }

    public static int GetDistanceBetweenNeighbours()
    {
        return 1;
    }
}



public class PathNode
{
    // Точка с координатами на сетке
    public APoint point { get; set; }

    // Точка из которой пришли
    public PathNode ComeFrom { get; set; }

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
