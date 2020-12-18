using KdTree;
using UnityEngine;

public class KdTreeDemo : MonoBehaviour
{
    public Vector2 m_min = new Vector2(10, 10);
    public Vector2 m_max = new Vector2(260, 260);

    private static Material m_mat;
    KdTree.KdTree m_tree = new KdTree.KdTree();

    // Use this for initialization
    void Start() {
        m_mat = new Material(Shader.Find("Unlit/Color"));

        m_tree.Insert(new Vector2(200, 200));
        m_tree.Insert(new Vector2(50, 25));
        m_tree.Insert(new Vector2(150, 100));
        m_tree.Insert(new Vector2(150, 170));
        m_tree.Insert(new Vector2(70, 100));
        m_tree.Insert(new Vector2(100, 40));
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            int x = (int)Input.mousePosition.x;
            int y = (int)Input.mousePosition.y;
            if (x >= m_min.x && x <= m_max.x && y >= m_min.y && y <= m_max.y)
                m_tree.Insert(new Vector2(x, y));
        }

        if (Input.GetMouseButtonUp(2))
        {
            var node = m_tree.FindMin(0);
            if (node != null)
                m_tree.Delete(node.Pos);
        }

        if (Input.GetMouseButtonUp(1))
        {
            var node = m_tree.NearestNeighbor(Input.mousePosition, new AABB2(m_min, m_max));
            if (node != null)
                m_tree.Delete(node.Pos);
        }
	}

    void OnGUI()
    {
        GUILayout.Label("左键点击新增点");
        GUILayout.Label("中键按住高亮x方向最小点，松开移除该点");
        GUILayout.Label("右键按住高亮x方向最近点，松开移除该点");

        m_mat.SetColor("_Color", Color.white);
        GraphicsTool.DrawRect(m_min, m_max, false, m_mat);

        //预显示当前鼠标位置
        GraphicsTool.DrawPoint(Input.mousePosition, 2, m_mat);

        ShowGraph();

        m_mat.SetColor("_Color", Color.yellow);
        //显示x方向最小点
        if (Input.GetMouseButton(2))
        {
            var node = m_tree.FindMin(0);
            GraphicsTool.DrawPoint(node.Pos, 5, m_mat);
        }

        //显示x方向最近点
        if (Input.GetMouseButton(1))
        {
            var node = m_tree.NearestNeighbor(Input.mousePosition, new AABB2(m_min, m_max));
            GraphicsTool.DrawPoint(node.Pos, 5, m_mat);
        }
    }

    void ShowGraphHelper(Node root, int dim, Material mat, float minX, float maxX, float minY, float maxY)
    {
        if (root == null)
            return;

        mat.SetColor("_Color", Color.red);
        GraphicsTool.DrawPoint(root.Pos, 5, mat);

        mat.SetColor("_Color", Color.green);

        if (dim % 2 == 0)
        {
            float x = root.Pos.x;

            Vector2 begin = new Vector2(x, maxY);
            Vector2 end = new Vector2(x, minY);
            GraphicsTool.DrawLine(begin, end, mat);

            ShowGraphHelper(root.Left, dim + 1, mat, minX, x, minY, maxY);
            ShowGraphHelper(root.Right, dim + 1, mat, x, maxX, minY, maxY);
        }
        else
        {
            float y = root.Pos.y;

            Vector2 begin = new Vector2(minX, y);
            Vector2 end = new Vector2(maxX, y);
            GraphicsTool.DrawLine(begin, end, mat);

            ShowGraphHelper(root.Left, dim + 1, mat, minX, maxX, minY, y);
            ShowGraphHelper(root.Right, dim + 1, mat, minX, maxX, y, maxY);
        }
    }

    private void ShowGraph()
    {
        if (m_tree.Root == null)
            return;

        ShowGraphHelper(m_tree.Root, 0, m_mat, m_min.x, m_max.x, m_min.y, m_max.y);
    }
}
