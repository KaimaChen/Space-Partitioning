using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 松散四叉树
/// </summary>
/// <typeparam name="T">要存放的物品类型</typeparam>
public class LooseQuadTree<T>
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
    /// 没有应用松散值前的基础长度（即普通四叉树的长度）
    /// </summary>
    private readonly float m_baseSize;

    /// <summary>
    /// 松散值，用于调整大小
    /// 值为1时表示普通的四叉树
    /// </summary>
    private readonly float m_loose;

    /// <summary>
    /// 存放的物品
    /// </summary>
    private readonly List<QuadTreeItem> m_items;

    private LooseQuadTree<T>[] m_children;

    public float BaseSize { get { return m_baseSize; } }
    public QuadTreeBound Bound { get { return m_bound; } }
    public List<QuadTreeItem> Items { get { return m_items; } }
    public LooseQuadTree<T>[] Children { get { return m_children; } }
    
    public LooseQuadTree(QuadTreeBound bound, float baseSize, float loose, int capacity, float minSize)
    {
        m_bound = bound;
        m_baseSize = baseSize;
        m_loose = loose;
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
        if(m_children == null) //没有子节点，尝试进行划分
        {
            if(m_items.Count < m_capacity || m_bound.size/2 < m_minSize)
            {
                m_items.Add(new QuadTreeItem(elem, bound));
            }
            else
            {
                Split();
                Redistribute();
                InsertRecursive(elem, bound);
            }
        }
        else
        {
            int bestFit = BestFitChild(bound.center);
            if(m_children[bestFit].Bound.Contains(bound))
                m_children[bestFit].InsertRecursive(elem, bound);
            else
                m_items.Add(new QuadTreeItem(elem, bound));
        }
    }

    private int BestFitChild(Vector2 center)
    {
        bool isLeft = center.x <= m_bound.center.x;
        bool isBottom = center.y <= m_bound.center.y;
        if (isLeft && isBottom) return k_BL;
        else if (isLeft && !isBottom) return k_TL;
        else if (!isLeft && isBottom) return k_BR;
        else return k_TR;
    }

    private void Split()
    {
        float x = m_bound.center.x;
        float y = m_bound.center.y;
        float s = m_baseSize / 2;
        float h = s /2;
        float actLen = s * m_loose;
        m_children = new LooseQuadTree<T>[4];
        m_children[k_TR] = new LooseQuadTree<T>(new QuadTreeBound(x + h, y + h, actLen), s, m_loose, m_capacity, m_minSize);
        m_children[k_TL] = new LooseQuadTree<T>(new QuadTreeBound(x - h, y + h, actLen), s, m_loose, m_capacity, m_minSize);
        m_children[k_BL] = new LooseQuadTree<T>(new QuadTreeBound(x - h, y - h, actLen), s, m_loose, m_capacity, m_minSize);
        m_children[k_BR] = new LooseQuadTree<T>(new QuadTreeBound(x + h, y - h, actLen), s, m_loose, m_capacity, m_minSize);
    }

    /// <summary>
    /// 看看能不能把自己身上的物体分给子节点
    /// </summary>
    private void Redistribute()
    {
        List<QuadTreeItem> overlapItems = new List<QuadTreeItem>();
        for (int i = 0; i < m_items.Count; i++)
        {
            int bestFit = BestFitChild(m_items[i].bound.center);
            if(m_children[bestFit].Bound.Contains(m_items[i].bound))
                m_children[bestFit].m_items.Add(m_items[i]);
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
            if (bound.Overlaps(m_items[i].bound))
                result.Add(m_items[i].elem);
        }

        if(m_children != null)
            for (int i = 0; i < m_children.Length; i++)
                m_children[i].Query(bound, result);
    }

    public bool Remove(T elem, QuadTreeBound bound)
    {
        for(int i = 0; i < m_items.Count; i++)
        {
            if(m_items[i].elem.Equals(elem))
            {
                m_items.RemoveAt(i);
                return true;
            }
        }

        if (m_children != null)
            for (int i = 0; i < m_children.Length; i++)
                if (m_children[i].Remove(elem, bound))
                    return true;

        return false;
    }

    public struct QuadTreeItem
    {
        public T elem;
        public QuadTreeBound bound;
        
        public QuadTreeItem(T elem, QuadTreeBound bound)
        {
            this.elem = elem;
            this.bound = bound;
        }
    }
}