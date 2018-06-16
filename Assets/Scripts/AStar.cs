using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    public const int mapWidth = 15;
    public const int mapHeight = 15;

    private Point[,] map = new Point[mapWidth, mapHeight];

    // Use this for initialization
    void Start() {
        InitMap();

        Point start = map[2, 3];
        Point end = map[6, 3];
        FindPath(start, end);
        ShowPath(start, end);
    }

    /// <summary>
    /// 初始化地图
    /// </summary>
    private void InitMap() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                map[x, y] = new Point(x, y);
            }
        }

        map[4, 2].IsWall = true;
        map[4, 3].IsWall = true;
        map[4, 4].IsWall = true;
        map[4, 5].IsWall = true;
        map[5, 6].IsWall = true;
        map[5, 3].IsWall = true;
        map[5, 2].IsWall = true;

    }

    
    /// <summary>
    /// 寻找路径
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void FindPath(Point start, Point end) {
        List<Point> openList = new List<Point>();
        List<Point> closeList = new List<Point>();
        openList.Add(start);
        while (openList.Count > 0) {
            Point point = FindMinFOfPoint(openList);
            openList.Remove(point);
            closeList.Add(point);

            List<Point> surroundPoints = GetSurroundPoints(point);
            FilterPoints(surroundPoints, closeList);

            foreach (Point surroundPoint in surroundPoints) {
                if (openList.IndexOf(surroundPoint) > -1)
                {
                    float nowG = CalcG(surroundPoint, point);
                    if (nowG < surroundPoint.G)
                    {
                        surroundPoint.UpdateParent(point, nowG);
                    }
                }
                else {
                    surroundPoint.Parent = point;
                    CalcF(surroundPoint, end);
                    openList.Add(surroundPoint);
                }
            }

            if (openList.IndexOf(end) > -1) {
                break;
            }
        }
    }

    /// <summary>
    /// 显示路径
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void ShowPath(Point start, Point end) {
        Point temp = end;
        while (true) {
            Debug.Log(temp.X + "," + temp.Y);

            Color c = Color.gray;
            if (temp == start)
            {
                c = Color.green;
            }
            else if (temp == end) {
                c = Color.red;
            }

            CreateCube(temp.X, temp.Y, c);

            if (temp.Parent == null) {
                break;
            }
            
            temp = temp.Parent;
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y].IsWall == true) {
                    CreateCube(x, y, Color.blue);
                }
            }
        }
    }

    private void CreateCube(int x, int y, Color c)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(x, y, 0);
        go.GetComponent<Renderer>().material.color = c;
    }

    private void  FilterPoints(List<Point> src, List<Point> closeList){
        foreach (Point p in closeList) {
            if (src.IndexOf(p) > -1) {
                src.Remove(p);
            }
        }
    }

    private List<Point> GetSurroundPoints(Point point) {
        Point up = null, down = null, left = null, right = null;
        Point lu = null, ld = null, ru = null, rd = null;
        if (point.Y < mapHeight - 1)
        {    
            up = map[point.X, point.Y + 1];
        }

        if (point.Y > 0)
        {        

            down = map[point.X, point.Y - 1];
        }

        if (point.X > 0)
        {
            left = map[point.X - 1, point.Y];
        }

        if (point.X < mapWidth - 1)
        {
            right = map[point.X+1, point.Y];
        }

        if (up != null && left != null) {
            lu = map[point.X - 1, point.Y + 1];
        }

        if (down != null && left != null)
        {
            ld = map[point.X - 1, point.Y - 1];
        }

        if (up != null && right != null)
        {
            ru = map[point.X + 1, point.Y + 1];
        }

        if (down != null && right != null)
        {
            rd = map[point.X + 1, point.Y - 1];
        }

        List<Point> pointList = new List<Point>();
        if (up != null && up.IsWall ==false) {
            pointList.Add(up);
        }
        if (down != null && down.IsWall == false)
        {
            pointList.Add(down);
        }
        if (left != null && left.IsWall == false)
        {
            pointList.Add(left);
        }
        if (right != null && right.IsWall == false)
        {
            pointList.Add(right);
        }

        if (lu != null && lu.IsWall == false && left.IsWall ==false && up.IsWall == false) {
            pointList.Add(lu);
        }

        if (ld != null && ld.IsWall == false && left.IsWall == false && down.IsWall == false)
        {
            pointList.Add(ld);
        }

        if (ru != null && ru.IsWall == false && right.IsWall == false && up.IsWall == false)
        {
            pointList.Add(ru);
        }

        if (rd != null && rd.IsWall == false && right.IsWall == false && down.IsWall == false)
        {
            pointList.Add(rd);
        }

        return pointList;
    }

    private Point FindMinFOfPoint(List<Point> openList) {
        float f = float.MaxValue;
        Point temp = null;
        foreach (Point p in openList) {
            if (p.F < f) {
                f = p.F;
                temp = p;
            }
        }

        return temp;
    }

    private float CalcG(Point now, Point parent) {

        float g = Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(parent.X, parent.Y)) + parent.G;

        return g;
    }

    private void CalcF(Point now, Point end) {
        //F = G + H
        float h = Mathf.Abs(end.X - now.X)+ Mathf.Abs(end.Y - now.Y);
        float g = 0;
        if (now.Parent == null)
        {
            g = 0;
        }
        else {
            g = Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(now.Parent.X, now.Parent.Y)) + now.Parent.G;
        }

        float f = g + h;
        now.F = f;
        now.G = g;
        now.H = h;
    }
}
