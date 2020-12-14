using System.Collections.Generic;

/// <summary>
/// 八叉树
/// 松散八叉树很容易根据松散四叉树构建（基本逻辑都是一一对应的），这里就懒得实现了
/// </summary>
/// <typeparam name="T">存放的物品类型</typeparam>
public class Octree<T>
{
    //子节点索引含义
    private const int k_FTL = 0; //front top left
    private const int k_FTR = 1;
    private const int k_FBR = 2;
    private const int k_FBL = 3;
    private const int k_RTL = 4; //rear top left
    private const int k_RTR = 5;
    private const int k_RBR = 6;
    private const int k_RBL = 7;

    private OctreeBound m_bound;
    private readonly int m_capacity;
    private readonly float m_minSize;
    private readonly List<OctreeItem> m_items;
    private Octree<T>[] m_children;

    public OctreeBound Bound { get { return m_bound; } }
    public List<OctreeItem> Items { get { return m_items; } }
    public Octree<T>[] Children { get { return m_children; } }

    public Octree(OctreeBound bound, int capacity, float minSize)
    {
        m_bound = bound;
        m_capacity = capacity;
        m_minSize = minSize;
        m_items = new List<OctreeItem>(capacity);
    }

    public void Reset()
    {
        m_items.Clear();

        if(m_children != null)
        {
            for (int i = 0; i < m_children.Length; i++)
                m_children[i].Reset();

            m_children = null;
        }
    }

    public void Insert(T elem, OctreeBound bound)
    {
        if (!m_bound.Overlaps(bound))
            return;

        InsertRecursive(elem, bound);
    }

    private void InsertRecursive(T elem, OctreeBound bound)
    {
        Octree<T> overlapChild = null;
        int overlapCount = BoundOverlapChildren(bound, ref overlapChild);
        if (overlapCount == 0)
        {
            if(m_items.Count < m_capacity || m_bound.size/2 < m_minSize)
            {
                m_items.Add(new OctreeItem(elem, bound));
            }
            else
            {
                Split();
                InsertRecursive(elem, bound);
            }
        }
        else if (overlapCount == 1)
        {
            overlapChild.InsertRecursive(elem, bound);
        }
        else
        {
            m_items.Add(new OctreeItem(elem, bound));
        }
    }

    private void BoundOverlapChildren(OctreeBound bound, List<Octree<T>> treeList)
    {
        if (m_children == null)
            return;

        bool overlapLeft = bound.xMin <= m_bound.center.x;
        bool overlapRight = bound.xMax >= m_bound.center.x;
        bool overlapBottom = bound.yMin <= m_bound.center.y;
        bool overlapTop = bound.yMax >= m_bound.center.y;
        bool overlapFront = bound.zMin <= m_bound.center.z;
        bool overlapRear = bound.zMax >= m_bound.center.z;

        if (overlapFront && overlapBottom && overlapLeft)
            treeList.Add(m_children[k_FBL]);

        if (overlapFront && overlapBottom && overlapRight)
            treeList.Add(m_children[k_FBR]);

        if (overlapFront && overlapTop && overlapLeft)
            treeList.Add(m_children[k_FTL]);

        if (overlapFront && overlapTop && overlapRight)
            treeList.Add(m_children[k_FTR]);

        if (overlapRear && overlapBottom && overlapLeft)
            treeList.Add(m_children[k_RBL]);

        if (overlapRear && overlapBottom && overlapRight)
            treeList.Add(m_children[k_RBR]);

        if (overlapRear && overlapTop && overlapLeft)
            treeList.Add(m_children[k_RTL]);

        if (overlapRear && overlapTop && overlapRight)
            treeList.Add(m_children[k_RTR]);
    }

    private int BoundOverlapChildren(OctreeBound bound, ref Octree<T> tree)
    {
        if (m_children == null)
            return 0;

        int count = 0;
        bool overlapLeft = bound.xMin <= m_bound.center.x;
        bool overlapRight = bound.xMax >= m_bound.center.x;
        bool overlapBottom = bound.yMin <= m_bound.center.y;
        bool overlapTop = bound.yMax >= m_bound.center.y;
        bool overlapFront = bound.zMin <= m_bound.center.z;
        bool overlapRear = bound.zMax >= m_bound.center.z;

        if(overlapFront && overlapBottom && overlapLeft)
        {
            count++;
            tree = m_children[k_FBL];
        }

        if(overlapFront && overlapBottom && overlapRight)
        {
            count++;
            tree = m_children[k_FBR];
        }

        if(overlapFront && overlapTop && overlapLeft)
        {
            count++;
            tree = m_children[k_FTL];
        }

        if(overlapFront && overlapTop && overlapRight)
        {
            count++;
            tree = m_children[k_FTR];
        }

        if(overlapRear && overlapBottom && overlapLeft)
        {
            count++;
            tree = m_children[k_RBL];
        }

        if(overlapRear && overlapBottom && overlapRight)
        {
            count++;
            tree = m_children[k_RBR];
        }

        if(overlapRear && overlapTop && overlapLeft)
        {
            count++;
            tree = m_children[k_RTL];
        }

        if(overlapRear && overlapTop && overlapRight)
        {
            count++;
            tree = m_children[k_RTR];
        }

        return count;
    }

    private void Split()
    {
        float x = m_bound.center.x;
        float y = m_bound.center.y;
        float z = m_bound.center.z;
        float s = m_bound.size / 2;
        float h = s / 2;
        m_children = new Octree<T>[8];
        m_children[k_FBL] = new Octree<T>(new OctreeBound(x - h, y - h, z - h, s), m_capacity, m_minSize);
        m_children[k_FBR] = new Octree<T>(new OctreeBound(x + h, y - h, z - h, s), m_capacity, m_minSize);
        m_children[k_FTL] = new Octree<T>(new OctreeBound(x - h, y + h, z - h, s), m_capacity, m_minSize);
        m_children[k_FTR] = new Octree<T>(new OctreeBound(x + h, y + h, z - h, s), m_capacity, m_minSize);
        m_children[k_RBL] = new Octree<T>(new OctreeBound(x - h, y - h, z + h, s), m_capacity, m_minSize);
        m_children[k_RBR] = new Octree<T>(new OctreeBound(x + h, y - h, z + h, s), m_capacity, m_minSize);
        m_children[k_RTL] = new Octree<T>(new OctreeBound(x - h, y + h, z + h, s), m_capacity, m_minSize);
        m_children[k_RTR] = new Octree<T>(new OctreeBound(x + h, y + h, z + h, s), m_capacity, m_minSize);

        List<OctreeItem> overlapItems = new List<OctreeItem>();
        for(int i = 0; i < m_items.Count; i++)
        {
            Octree<T> subTree = null;
            int overlapCount = BoundOverlapChildren(m_items[i].m_bound, ref subTree);
            if (overlapCount == 1)
                subTree.m_items.Add(m_items[i]);
            else
                overlapItems.Add(m_items[i]);
        }
        m_items.Clear();
        m_items.AddRange(overlapItems);
    }

    public void Query(OctreeBound bound, List<T> result)
    {
        if (!m_bound.Overlaps(bound))
            return;

        for(int i = 0; i < m_items.Count; i++)
        {
            if (bound.Overlaps(m_items[i].m_bound))
                result.Add(m_items[i].m_elem);
        }

        List<Octree<T>> subTrees = new List<Octree<T>>();
        BoundOverlapChildren(bound, subTrees);
        for (int i = 0; i < subTrees.Count; i++)
            subTrees[i].Query(bound, result);
    }

    public bool Remove(T elem, OctreeBound bound)
    {
        for(int i = 0; i < m_items.Count; i++)
        {
            if(m_items[i].m_elem.Equals(elem))
            {
                m_items.RemoveAt(i);
                return true;
            }
        }

        List<Octree<T>> subTrees = new List<Octree<T>>();
        BoundOverlapChildren(bound, subTrees);
        for(int i = 0; i < subTrees.Count; i++)
        {
            if (subTrees[i].Remove(elem, bound))
                return true;
        }

        return false;
    }

    public struct OctreeItem
    {
        public T m_elem;
        public OctreeBound m_bound;

        public OctreeItem(T elem, OctreeBound bound)
        {
            m_elem = elem;
            m_bound = bound;
        }
    }
}