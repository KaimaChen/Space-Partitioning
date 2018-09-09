using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Left Click can add a point
/// This demo can watch how the quad tree is built
/// </summary>
public class QuadTreeClickDemo : MonoBehaviour {
    public const int WIDTH = 1024;
    public const int HEIGHT = 768;
    public const int MOUSE_RECT_WIDTH = 100;
    public const int MOUSE_RECT_HEIGHT = 100;
    
    private Material mMat;
    
    List<Point> mPoints = new List<Point>();
    QuadTree mQuadTree;
    
    void Start()
    {
        mMat = new Material(Shader.Find("Unlit/Color"));

        mQuadTree = new QuadTree(new AABB(WIDTH / 2f, HEIGHT / 2f, WIDTH, HEIGHT), 1);
    }
    
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Point point = new Point(Input.mousePosition.x, Input.mousePosition.y, 0);
            mPoints.Add(point);

            mQuadTree.Reset();

            for (int i = 0; i < mPoints.Count; i++)
            {
                mPoints[i].mColor = Color.blue;

                mQuadTree.Insert(mPoints[i]);
            }
        }
    }

    void OnGUI()
    {
        mMat.SetColor("_Color", Color.white);
        QuadTreeDemo.DrawRect(mQuadTree, mMat);
        
        for (int i = 0; i < mPoints.Count; i++)
        {
            QuadTreeDemo.DrawPoint(mPoints[i], mMat);
        }
    }
}
