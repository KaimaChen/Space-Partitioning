using System.Collections.Generic;

public class QuadTree<T>
{
    //子节点索引含义
    private const int k_TL = 0;
    private const int k_TR = 1;
    private const int k_BR = 2;
    private const int k_BL = 3;

    private QuadTreeBound m_bound;

    /// <summary>
    /// 最多能存放多少物体（不能再细分时才会超过这个数量）
    /// </summary>
    private readonly int m_capacity;

    /// <summary>
    /// 不能划分出小于该大小的节点（啥时候停止）
    /// </summary>
    private readonly float m_minSize;

    /// <summary>
    /// 存放的物品
    /// </summary>
    private readonly List<QuadTreeItem> m_items;

    private QuadTree<T>[] m_children;

    public QuadTreeBound Bound { get { return m_bound; } }
    public List<QuadTreeItem> Items { get { return m_items; } }
    public QuadTree<T>[] Children { get { return m_children; } }
    
    public QuadTree(QuadTreeBound bound, int capacity, float minSize)
    {
        m_bound = bound;
        m_capacity = capacity;
        m_minSize = minSize;
        m_items = new List<QuadTreeItem>(capacity);
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

    public void Insert(T elem, QuadTreeBound bound)
    {
        if (!m_bound.Overlaps(bound))
            return;

        InsertRecursive(elem, bound);
    }

    private void InsertRecursive(T elem, QuadTreeBound bound)
    {
        QuadTree<T> overlapChild = null;
        int overlapCount = BoundOverlapChildren(bound, ref overlapChild);
        if(overlapCount == 0) //没有子节点，尝试进行划分
        {
            bool canSplit = m_bound.size > m_minSize;
            if(!canSplit || m_items.Count < m_capacity)
            {
                m_items.Add(new QuadTreeItem(elem, bound));
            }
            else
            {
                Split();
                InsertRecursive(elem, bound);
            }
        }
        else if(overlapCount == 1) //该区域完全在某个子节点上
        {
            overlapChild.InsertRecursive(elem, bound);
        }
        else //覆盖了多个子节点，所以要加到自己的列表中
        {
            m_items.Add(new QuadTreeItem(elem, bound));
        }
    }

    /// <summary>
    /// 找到矩形交叠的所有子节点
    /// </summary>
    /// <param name="bound">查找的范围</param>
    /// <param name="tree">所有交叠的子节点</param>
    private void BoundOverlapChildren(QuadTreeBound bound, List<QuadTree<T>> treeList)
    {
        if (m_children == null)
            return;

        bool overlapLeft = bound.xMin <= m_bound.center.x;
        bool overlapRight = bound.xMax >= m_bound.center.x;
        bool overlapBottom = bound.yMin <= m_bound.center.y;
        bool overlapTop = bound.yMax >= m_bound.center.y;

        if (overlapLeft && overlapBottom)
            treeList.Add(m_children[k_BL]);

        if (overlapLeft && overlapTop)
            treeList.Add(m_children[k_TL]);

        if (overlapRight && overlapTop)
            treeList.Add(m_children[k_TR]);

        if (overlapRight && overlapBottom)
            treeList.Add(m_children[k_BR]);
    }

    /// <summary>
    /// 找到矩形交叠的子节点
    /// </summary>
    /// <param name="bound">查找的范围</param>
    /// <param name="tree">其中一个交叠的子节点</param>
    /// <returns>多少个交叠的子节点</returns>
    private int BoundOverlapChildren(QuadTreeBound bound, ref QuadTree<T> tree)
    {
        if (m_children == null)
            return 0;

        int count = 0;
        bool overlapLeft = bound.xMin <= m_bound.center.x;
        bool overlapRight = bound.xMax >= m_bound.center.x;
        bool overlapBottom = bound.yMin <= m_bound.center.y;
        bool overlapTop = bound.yMax >= m_bound.center.y;

        if (overlapLeft && overlapBottom)
        {
            count++;
            tree = m_children[k_BL];
        }

        if (overlapLeft && overlapTop)
        {
            count++;
            tree = m_children[k_TL];
        }

        if (overlapRight && overlapTop)
        {
            count++;
            tree = m_children[k_TR];
        }

        if (overlapRight && overlapBottom)
        {
            count++;
            tree = m_children[k_BR];
        }

        return count;
    }

    private void Split()
    {
        //划分子节点
        float x = m_bound.center.x;
        float y = m_bound.center.y;
        float s = m_bound.size / 2;
        m_children = new QuadTree<T>[4];
        m_children[k_TR] = new QuadTree<T>(new QuadTreeBound(x, y, s), m_capacity, m_minSize);
        m_children[k_TL] = new QuadTree<T>(new QuadTreeBound(x - s, y, s), m_capacity, m_minSize);
        m_children[k_BL] = new QuadTree<T>(new QuadTreeBound(x - s, y - s, s), m_capacity, m_minSize);
        m_children[k_BR] = new QuadTree<T>(new QuadTreeBound(x, y - s, s), m_capacity, m_minSize);

        //看看能不能把自己身上的物体分给子节点
        List<QuadTreeItem> overlapItems = new List<QuadTreeItem>();
        for(int i = 0; i < m_items.Count; i++)
        {
            QuadTree<T> subTree = null;
            int overlapCount = BoundOverlapChildren(m_items[i].m_bound, ref subTree);
            if (overlapCount == 1)
                subTree.m_items.Add(m_items[i]);
            else
                overlapItems.Add(m_items[i]);
        }
        m_items.Clear();
        m_items.AddRange(overlapItems);
    }

    public void Query(QuadTreeBound bound, List<T> result)
    {
        if (!m_bound.Overlaps(bound))
            return;

        for(int i = 0; i < m_items.Count; i++)
        {
            if (bound.Overlaps(m_items[i].m_bound))
                result.Add(m_items[i].m_elem);
        }

        List<QuadTree<T>> subTrees = new List<QuadTree<T>>();
        BoundOverlapChildren(bound, subTrees);
        for (int i = 0; i < subTrees.Count; i++)
            subTrees[i].Query(bound, result);
    }

    public bool Remove(T elem, QuadTreeBound bound)
    {
        for(int i = 0; i < m_items.Count; i++)
        {
            if(m_items[i].m_elem.Equals(elem))
            {
                m_items.RemoveAt(i);
                return true;
            }
        }

        List<QuadTree<T>> subTrees = new List<QuadTree<T>>();
        BoundOverlapChildren(bound, subTrees);
        for(int i = 0; i < subTrees.Count; i++)
        {
            if (subTrees[i].Remove(elem, bound))
                return true;
        }

        return false;
    }

    public struct QuadTreeItem
    {
        public T m_elem;
        public QuadTreeBound m_bound;
        
        public QuadTreeItem(T elem, QuadTreeBound bound)
        {
            m_elem = elem;
            m_bound = bound;
        }
    }
}