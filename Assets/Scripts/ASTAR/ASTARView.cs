using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;


public class ASTARView : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private APointView PointView;
    [SerializeField] private GameObject PathPoint;
    [Space]
    [Header("Grid width and height")]
    [SerializeField] private IntVector2 WidthHeight;
    [Header("Size of a gameobject representing the PointView")]
    [SerializeField] private float PointViewSize;
    [Space]
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private LineRenderer[] lineRenderers;


    private ASTAR astar;
    private JASTAR jastar;
    private LinkedJASTAR linkedjastar;
    private List<APointView> pointviews;
    private APointView selectedPoint;

    private Queue<GameObject> pathPointPool;
    private Queue<GameObject> activePathPoint;

    private APointView startPoint;
    private APointView goalPoint;

    public void Start()
    {
        WidthHeight += IntVector2.one;
        astar = new ASTAR();
        jastar = new JASTAR();
        linkedjastar = new LinkedJASTAR();
        pointviews = new List<APointView>();
        pathPointPool = new Queue<GameObject>();
        activePathPoint = new Queue<GameObject>();

        DrawGrid();
    }

    public void DrawGrid()
    {
        astar.GenerateGrid(WidthHeight);
        jastar.GenerateGrid(WidthHeight.x, WidthHeight.y);
        linkedjastar.GenerateGrid(WidthHeight.x, WidthHeight.y);

        for (int x = 0; x < WidthHeight.x; x++)
        {
            for (int y = 0; y < WidthHeight.y; y++)
            {
                var pp = Instantiate(PathPoint);
                pp.SetActive(false);
                pathPointPool.Enqueue(pp);
                
                var pv = Instantiate(PointView);
                pv.transform.parent = transform;
                pv.transform.position = new Vector3(x * PointViewSize, 0, y * PointViewSize);
                pv.point = astar.PointGrid[x, y];
                pv.jastarPoint = jastar.Grid[y][x];
                pv.linkedPoint = linkedjastar.Grid[y][x];
                pv.Initialize();

                pointviews.Add(pv);
            }
        }
    }

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    RaycastHit hit;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    if (Physics.Raycast(ray, out hit, layerMask))
        //    {
        //        if(selectedPoint != null)
        //        {
        //            selectedPoint.sprite.color = Color.white;
        //        }

        //        selectedPoint = hit.collider.GetComponent<APointView>();
        //        selectedPoint.sprite.color = Color.blue;

        //        selectedPoint.Log();

        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                SelectPathStart(hit);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                SelectPathGoal(hit);
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                PaintWall(hit,true);
            }
        }

        if (Input.GetKey(KeyCode.C))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                PaintWall(hit, false);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestSearchers();
        }
    }

    public void SelectPathStart(RaycastHit hit)
    {
        if(startPoint != null)
        {
            if (startPoint == hit.collider.GetComponent<APointView>())
            {
                return;
            }

            startPoint.sprite.color = Color.white;
        }

        startPoint = hit.collider.GetComponent<APointView>();
        startPoint.sprite.color = Color.green;

        if (goalPoint != null)
            DrawPath();
    }

    public void SelectPathGoal(RaycastHit hit)
    {
        if (goalPoint != null)
        {
            if(goalPoint == hit.collider.GetComponent<APointView>())
            {
                return;
            }

            goalPoint.sprite.color = Color.white;
        }

        goalPoint = hit.collider.GetComponent<APointView>();
        goalPoint.sprite.color = Color.red;

        if (startPoint != null)
            DrawPath();
    }

    public void PaintWall(RaycastHit hit,bool isWall)
    {
        var point = hit.collider.GetComponent<APointView>();

        if (point.point.IsWall == isWall)
            return;

        var color = (isWall) ? Color.magenta : Color.white;

        point.sprite.color = color;
        astar.PointGrid[point.point.pos.x, point.point.pos.y].IsWall = isWall;
        jastar.Grid[point.point.pos.y][point.point.pos.x].IsWall = isWall;
        linkedjastar.Grid[point.point.pos.y][point.point.pos.x].IsWall = isWall;
        point.point.IsWall = isWall;

        //TryDrawPath();
    }


    public void DrawPath()
    {
        int count = activePathPoint.Count;
        for(int q = 0; q < count; q++)
        {
            var ap = activePathPoint.Dequeue();
            ap.SetActive(false);
            pathPointPool.Enqueue(ap);
        }

        var path = astar.FindPath(startPoint.point, goalPoint.point);

        for(int i = 0; i < path.Count; i++)
        {
            var pp = pathPointPool.Dequeue();
            pp.SetActive(true);
            activePathPoint.Enqueue(pp);

            var pos = path[i].pos;
            pp.transform.position = new Vector3(pos.x*PointViewSize, 0.3f, pos.y* PointViewSize);
        }
    }

    public void TryDrawPath()
    {
        if (startPoint != null&& goalPoint!=null)
            DrawPath();
    }

    public void TestSearchers()
    {
        var timer = new Stopwatch();

        timer.Start();
        // Последовательный поиск пути
        var SequentiallyPath = astar.FindPath(startPoint.point,goalPoint.point);
        timer.Stop();
        var sequentialTime = timer.ElapsedMilliseconds;

        timer.Restart();
        //// Параллельный поиск пути
        //var ParallelPath = astar.FindPathParallel(startPoint.point, goalPoint.point);
        //timer.Stop();
        //var parallelTime = timer.ElapsedMilliseconds;

        timer.Restart();
        // Поиск пути по зазубренным спискам
        var JaggedSequentialPath = jastar.FindPath(startPoint.point, goalPoint.point);
        timer.Stop();
        var jaggedSequentialTime = timer.ElapsedMilliseconds;

        timer.Restart();
        // Поиск пути по связным зазубренным элементам 
        var LinkedjaggedSequentialPath = linkedjastar.FindPath(startPoint.linkedPoint, goalPoint.linkedPoint);
        timer.Stop();
        var linkedJaggedSequentialTime = timer.ElapsedMilliseconds;

        UnityEngine.Debug.Log("<color=olive>Sequential Execute Time:</color>" + sequentialTime.ToString());
        //UnityEngine.Debug.Log("<color=orange>Parallel Execute Time:</color>" + parallelTime.ToString());
        UnityEngine.Debug.Log("<color=maroon>JaggedSequential Execute Time:</color>" + jaggedSequentialTime.ToString());
        UnityEngine.Debug.Log("<color=teal>LinkedJaggedSequential Execute Time:</color>" + linkedJaggedSequentialTime.ToString());



        lineRenderers[0].positionCount = SequentiallyPath.Count;
        for(int i = 0; i < SequentiallyPath.Count; i++)
        {
            var pos = SequentiallyPath[i].pos;
            lineRenderers[0].SetPosition(i, new Vector3(pos.x * PointViewSize, 0.2f,pos.y * PointViewSize));
        }

        //for(int p = 0; p<ParallelPath.Count; p++)
        //{
        //    var path = ParallelPath[p];
            
        //    lineRenderers[p + 1].positionCount = path.Count;
        //    for (int i = 0; i < path.Count; i++)
        //    {
        //        var pos = path[i].pos;
        //        lineRenderers[p+1].SetPosition(i, new Vector3(pos.x*PointViewSize, 0.3f*(p+1), pos.y * PointViewSize));
        //    }
        //}

    }
}

