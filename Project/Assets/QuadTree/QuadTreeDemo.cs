using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过左键点击来生成点来查看四叉树的构建过程
/// </summary>
public class QuadTreeDemo : MonoBehaviour 
{
    public int m_maxSize = 512;
    public float m_minSize = 0;
    public int m_capacity = 1;
    public int m_mouseRectWidth = 100;
    public int m_mouseRectHeight = 100;
    
    private Material mMat;
    
    List<Vector2> mPoints = new List<Vector2>();
    QuadTree<Vector2> mQuadTree;
    
    void Start()
    {
        mMat = new Material(Shader.Find("Unlit/Color"));

        mQuadTree = new QuadTree<Vector2>(new QuadTreeBound(0, 0, m_maxSize), m_capacity, m_minSize);
    }
    
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Vector2 point = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            mPoints.Add(point);

            mQuadTree.Reset();

            for (int i = 0; i < mPoints.Count; i++)
                mQuadTree.Insert(mPoints[i], new QuadTreeBound(mPoints[i], 1));
        }
    }

    void OnGUI()
    {
        GUILayout.Label("左键点击产生点来查看四叉树的构建");

        mMat.SetColor("_Color", Color.white);
        DrawRect(mQuadTree, mMat);
        
        for (int i = 0; i < mPoints.Count; i++)
        {
            DrawPoint(mPoints[i], mMat);
        }
    }

    public static void DrawRect(QuadTreeBound bound, Material mat)
    {
        GraphicsTool.DrawRect(new Vector2(bound.xMin, bound.yMax), new Vector2(bound.xMax, bound.yMin), false, mat);
    }

    void DrawRect<T>(QuadTree<T> tree, Material mat)
    {
        DrawRect(tree.Bound, mat);

        if (tree.Children != null)
        {
            for (int i = 0; i < tree.Children.Length; i++)
            {
                DrawRect(tree.Children[i], mat);
            }
        }
    }

    void DrawPoint(Vector2 point, Material mat)
    {
        mat.SetColor("_Color", Color.blue);
        GraphicsTool.DrawPoint(new Vector2(point.x, point.y), 3, mat);
    }
}
