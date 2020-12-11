using UnityEngine;

public class OctreeDemo : MonoBehaviour
{
    public float m_maxSize = 20;
    public float m_minSize = 0;
    public int m_capacity = 4;
    Octree<BoxCollider> m_octree;

    void Start()
    {
        m_octree = new Octree<BoxCollider>(new OctreeBound(0, 0, 0, m_maxSize), m_capacity, m_minSize);

        for (int i = 0; i < 10; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float x = Random.Range(0, m_maxSize / 2);
            float y = Random.Range(0, m_maxSize / 2);
            float z = Random.Range(0, m_maxSize / 2);
            cube.transform.position = new Vector3(x, y, z);
            cube.transform.SetParent(transform);
        }

        for(int i = 0;  i < 10; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float x = Random.Range(m_maxSize / 2, m_maxSize);
            float y = Random.Range(m_maxSize / 2, m_maxSize);
            float z = Random.Range(m_maxSize / 2, m_maxSize);
            cube.transform.position = new Vector3(x, y, z);
            cube.transform.SetParent(transform);
        }
    }

    void Update()
    {
        m_octree.Reset();

        for(int i = 0; i < transform.childCount; i++)
        {
            var e = transform.GetChild(i).GetComponent<BoxCollider>();
            if (e == null)
                continue;

            var bl = e.transform.position - e.size / 2;
            float size = Mathf.Max(e.bounds.size.x, Mathf.Max(e.bounds.size.y, e.bounds.size.z));
            OctreeBound bound = new OctreeBound(bl, size);
            m_octree.Insert(e, bound);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("在Octree物体下添加Cube来查看八叉树的构建");
        GUILayout.Label("注意Cube要带BoxCollider");
    }

    void OnDrawGizmos()
    {
        DrawOctree(m_octree);
    }

    private static void DrawOctree(Octree<BoxCollider> octree)
    {
        if (octree == null)
            return;

        DrawBox(octree.Bound);
        if(octree.Children != null)
        {
            for (int i = 0; i < octree.Children.Length; i++)
                DrawOctree(octree.Children[i]);
        }
    }

    private static void DrawBox(OctreeBound box)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(box.center, new Vector3(box.size, box.size, box.size));
    }
}