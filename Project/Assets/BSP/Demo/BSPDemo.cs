using BSP;
using System.Collections.Generic;
using UnityEngine;

public class BSPDemo : MonoBehaviour 
{
    public float m_width = 256;
    public float m_height = 256;
    public float m_gap = 0.1f;

    private Material m_mat;

    Vector2 m_viewPos = new Vector2(135, 135);

    Node m_root;
    List<Node> m_doneDraw = new List<Node>();
    List<Node> m_waitDraw = new List<Node>();
    float m_nextTime;

    void Start()
    {
        m_mat = new Material(Shader.Find("Unlit/Color"));
        m_mat.SetColor("_Color", Color.red);

        List<LineSegment> segs = new List<LineSegment>
        {
            new LineSegment(new Vector2(174, 161), new Vector2(102, 161)),
            new LineSegment(new Vector2(102, 199), new Vector2(102, 134)),
            new LineSegment(new Vector2(163, 119), new Vector2(216, 198)),
            new LineSegment(new Vector2(55, 184), new Vector2(114, 81)),
        };

        m_root = new Node(segs);
        m_root.Render(m_viewPos, m_waitDraw);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_doneDraw.Clear();
            m_waitDraw.Clear();
            m_nextTime = 0;
            m_viewPos = Input.mousePosition;
            m_root.Render(m_viewPos, m_waitDraw);
        }

        if(Time.time >= m_nextTime && m_waitDraw.Count > 0)
        {
            m_nextTime = Time.time + m_gap;
            m_doneDraw.Add(m_waitDraw[0]);
            m_waitDraw.RemoveAt(0);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("运行即可看到渲染顺序");
        GUILayout.Label("左键点击产生观察点位置，鼠标位置：" + Input.mousePosition);

        DrawRect(new Vector2(10, 10), new Vector2(m_width, m_height), Color.white);
        DrawPoint(m_viewPos, Color.red);

        m_mat.SetColor("_Color", Color.white);
        for (int i = 0; i < m_doneDraw.Count; i++)
            DrawLineSegments(m_doneDraw[i].Items);
    }

    private void DrawLineSegments(List<LineSegment> segs)
    {
        for(int i = 0; i< segs.Count; i++)
            GraphicsTool.DrawLine(segs[i].begin, segs[i].end, m_mat, true);
    }

    private void DrawRect(Vector2 min, Vector2 size, Color color)
    {
        m_mat.SetColor("_Color", color);
        GraphicsTool.DrawRect(min, min+size, false, m_mat);
    }

    private void DrawPoint(Vector2 point, Color color)
    {
        m_mat.SetColor("_Color", color);
        GraphicsTool.DrawPoint(new Vector2(point.x, point.y), 2, m_mat);
    }
}