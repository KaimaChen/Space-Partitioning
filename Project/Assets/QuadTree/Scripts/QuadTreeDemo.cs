using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This demo can watch how quad tree is change when the point is moving
/// </summary>
public class QuadTreeDemo : MonoBehaviour {
    public const int WIDTH = 1024;
    public const int HEIGHT = 768;
    public const int MOUSE_RECT_WIDTH = 100;
    public const int MOUSE_RECT_HEIGHT = 100;

    public int mPointsCount = 100;
    public float mPointSpeed = 0.5f;

    private Material mMat;
    
    Point[] mPoints;
    AABB mMouseRect;
    QuadTree mQuadTree;
    
	void Start () {
        mMat = new Material(Shader.Find("Unlit/Color"));
        
        mQuadTree = new QuadTree(new AABB(WIDTH / 2f, HEIGHT / 2f, WIDTH, HEIGHT), 4);

        mPoints = new Point[mPointsCount];
        for(int i = 0; i < mPointsCount; i++)
        {
            mPoints[i] = new Point(Random.Range(0, WIDTH), Random.Range(0, HEIGHT), mPointSpeed);
            mQuadTree.Insert(mPoints[i]);
        }

        mMouseRect = new AABB(0, 0, MOUSE_RECT_WIDTH, MOUSE_RECT_HEIGHT);
	}
	
	void Update () {
        mMouseRect.mCenterX = Input.mousePosition.x;
        mMouseRect.mCenterY = Input.mousePosition.y;

        mQuadTree.Reset();

        for(int i = 0; i < mPoints.Length; i++)
        {
            mPoints[i].mColor = Color.blue;

            mPoints[i].mX += mPoints[i].mMoveDir.x;
            if (mPoints[i].mX < 0)
            {
                mPoints[i].mX = 0;
                mPoints[i].mMoveDir.x *= -1;
            }
            else if (mPoints[i].mX > WIDTH)
            {
                mPoints[i].mX = WIDTH;
                mPoints[i].mMoveDir.x *= -1;
            }

            mPoints[i].mY += mPoints[i].mMoveDir.y;
            if (mPoints[i].mY < 0)
            {
                mPoints[i].mY = 0;
                mPoints[i].mMoveDir.y *= -1;
            }
            else if (mPoints[i].mY > HEIGHT)
            {
                mPoints[i].mY = HEIGHT;
                mPoints[i].mMoveDir.y *= -1;
            }

            mQuadTree.Insert(mPoints[i]);
        }

        List<Point> inRect = new List<Point>();
        mQuadTree.Query(mMouseRect, inRect);
        for(int i = 0; i < inRect.Count; i++)
        {
            inRect[i].mColor = Color.red;
        }
    }

    void OnGUI()
    {
        mMat.SetColor("_Color", Color.white);
        DrawRect(mQuadTree, mMat);

        mMat.SetColor("_Color", Color.green);
        DrawRect(mMouseRect, mMat);

        for (int i = 0; i < mPoints.Length; i++)
        {
            DrawPoint(mPoints[i], mMat);
        }
    }

    public static void DrawRect(AABB aabb, Material mat)
    {
        GraphicsTool.DrawRect(new Vector2(aabb.Left, aabb.Top), new Vector2(aabb.Right, aabb.Bottom), false, mat);
    }

    public static void DrawRect(QuadTree quad, Material mat)
    {
        DrawRect(quad.mAABB, mat);
        
        if(quad.mChildren != null)
        {
            for(int i = 0; i < quad.mChildren.Length; i++)
            {
                DrawRect(quad.mChildren[i], mat);
            }
        }
    }

    public static void DrawPoint(Point point, Material mat)
    {
        mat.SetColor("_Color", point.mColor);
        GraphicsTool.DrawPoint(new Vector2(point.mX, point.mY), 3, mat);
    }
}
