using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point  {

    public Point Parent { get; set; }
    public float F { get; set; }
    public float G { get; set; }
    public float H { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsWall { get; set; }

    public Point(int x, int y, Point parent =null) {
        X = x;
        Y = y;
        Parent = parent;
        IsWall = false;
    }

    public void UpdateParent(Point parent, float g) {
        Parent = parent;
        G = g;
        F = G + H;
    }
}
