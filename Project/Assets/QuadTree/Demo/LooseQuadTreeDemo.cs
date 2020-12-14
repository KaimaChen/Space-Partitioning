using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过左键点击来生成点来查看四叉树的构建过程
/// 勾选查询后可以查看特定矩形范围内的点
/// </summary>
public class LooseQuadTreeDemo : MonoBehaviour 
{
    public int m_maxSize = 256;
    public float m_minSize = 0;
    public int m_capacity = 1;
    public float m_loose = 2;

    public int m_showLevel = -1;
    public bool m_containsChildLevel = false;
    public int m_showChild = -1;
    public bool m_isQuery = false;
    public int m_mouseRectWidth = 100;
    public int m_mouseRectHeight = 100;
    public int m_pointSize = 8;

    private Material m_mat;
    
    List<Vector2> m_Points = new List<Vector2>();
    LooseQuadTree<Vector2> m_quadTree;
    
    void Start()
    {
        m_mat = new Material(Shader.Find("Unlit/Color"));

        m_quadTree = new LooseQuadTree<Vector2>(new QuadTreeBound(m_maxSize, m_maxSize, m_maxSize), m_maxSize, m_loose, m_capacity, m_minSize);
    }

    void Update()
    {
        if (m_isQuery == false && Input.GetMouseButtonUp(0))
        {
            Vector2 point = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var bound = new QuadTreeBound(point, m_pointSize);
            if (m_quadTree.Bound.Contains(bound))
            {
                m_Points.Add(point);
                m_quadTree.Insert(point, bound);
            }
            else
            {
                Debug.LogError("不能在区域外加点");
            }
        }
    }

    void OnGUI()
    {
        m_isQuery = GUILayout.Toggle(m_isQuery, "是否进行查询");
        if (m_isQuery)
        {
            m_showLevel = -1;
            m_showChild = -1;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("查询框宽度");
                int.TryParse(GUILayout.TextField(m_mouseRectWidth.ToString()), out m_mouseRectWidth);
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("查询框高度");
                int.TryParse(GUILayout.TextField(m_mouseRectHeight.ToString()), out m_mouseRectHeight);
            }
            GUILayout.EndHorizontal();

            DrawRect(m_quadTree, 0, 0);

            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            QuadTreeBound bound = new QuadTreeBound(x, y, m_mouseRectHeight);
            DrawRect(bound, Color.red);
            List<Vector2> queryResult = new List<Vector2>();
            m_quadTree.Query(bound, queryResult);
            for (int i = 0; i < queryResult.Count; i++)
                DrawPoint(queryResult[i], Color.red);
        }
        else
        {
            GUILayout.Label("左键点击产生点来查看四叉树的构建");

            if (m_showLevel >= 0)
            {
                float size = m_maxSize / Mathf.Pow(2, m_showLevel);
                GUILayout.Label("当前层大小=" + size + ", 松散大小=" + size * m_loose);
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("显示哪一层");
                int.TryParse(GUILayout.TextField(m_showLevel.ToString(), GUILayout.Width(20)), out m_showLevel);

                GUILayout.Space(5);

                GUILayout.Label("是否包含其以下层级");
                m_containsChildLevel = GUILayout.Toggle(m_containsChildLevel, string.Empty);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("显示哪个子节点");
                int.TryParse(GUILayout.TextField(m_showChild.ToString()), out m_showChild);
            }
            GUILayout.EndHorizontal();

            DrawRect(m_quadTree, 0, 0);
        }
    }

    void DrawRect(LooseQuadTree<Vector2> tree, int level, int childIndex)
    {
        if((m_showLevel < 0 || level == m_showLevel || (m_containsChildLevel && level >= m_showLevel)) &&
            (m_showChild < 0 || m_showChild == childIndex))
        {
            DrawRect(tree.Bound, Color.white);

            for (int i = 0; i < tree.Items.Count; i++)
                DrawPoint(tree.Items[i].elem, Color.blue);
        }

        if (tree.Children != null)
        {
            level++;
            for (int i = 0; i < tree.Children.Length; i++)
            {
                DrawRect(tree.Children[i], level, i);
            }
        }
    }

    void DrawRect(QuadTreeBound bound, Color color)
    {
        m_mat.SetColor("_Color", color);
        GraphicsTool.DrawRect(new Vector2(bound.xMin, bound.yMax), new Vector2(bound.xMax, bound.yMin), false, m_mat);
    }

    void DrawPoint(Vector2 point, Color color)
    {
        m_mat.SetColor("_Color", color);
        GraphicsTool.DrawPoint(new Vector2(point.x, point.y), m_pointSize / 2, m_mat);
    }
}
