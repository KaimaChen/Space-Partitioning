using BVH;
using UnityEngine;

public class BVHDemo : MonoBehaviour 
{
    private BVH<GameObject> m_bvh = new BVH<GameObject>();

    void OnGUI()
    {
        GUILayout.Label("不用运行，直接在BVH下添加或修改子物体(需要带BoxCollider)即可看到效果");
    }

    void OnDrawGizmos()
    {
        m_bvh = new BVH<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var e = transform.GetChild(i).GetComponent<BoxCollider>();
            if (e == null)
                continue;

            var bounds = e.bounds;
            AABB box = new AABB() { lowerBound = bounds.min, upperBound = bounds.max };
            Item<GameObject> item = new Item<GameObject>(e.gameObject, box);
            m_bvh.Insert(item);
        }

        Gizmos.color = Color.white;
        DrawNode(m_bvh.Root);
    }

    void DrawNode(Node<GameObject> node)
    {
        if (node == null)
            return;

        Gizmos.DrawWireCube(node.Box.center, node.Box.size);

        DrawNode(node.Left);
        DrawNode(node.Right);
    }
}
