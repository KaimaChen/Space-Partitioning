using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 二分空间
/// 这里为了简单用BSP来分割2D直线，推广到三维空间也不难，概念都是一一对应的：直线对应三角形，根据三角形所在平面分割成前面和后面
/// BSP还有很多其它用途，比如生成迷宫等
/// 
/// 参考：
/// https://zhuanlan.zhihu.com/p/53388395
/// </summary>
namespace BSP
{
    public class Node
    {
        private Node m_frontChild;
        private Node m_rearChild;
        private readonly List<LineSegment> m_items = new List<LineSegment>();

        public bool IsLeaf { get { return m_frontChild == null && m_rearChild == null; } }
        public List<LineSegment> Items { get { return m_items; } }
        public Node FrontChild { get { return m_frontChild; } }
        public Node RearChild { get { return m_rearChild; } }

        public Node(List<LineSegment> segs)
        {
            if(segs == null || segs.Count <= 0)
            {
                Debug.LogError("创建BSP.Node出问题");
                return;
            }

            //这里可以考虑随机选
            m_items.Add(segs[0]);
            segs.RemoveAt(0);
            if(segs.Count > 0)
                Divide(m_items[0], segs);
        }

        private void Divide(LineSegment divider, List<LineSegment> dividees)
        {
            List<LineSegment> collinears = new List<LineSegment>();
            List<LineSegment> fronts = new List<LineSegment>();
            List<LineSegment> rears = new List<LineSegment>();

            for(int i = 0; i < dividees.Count; i++)
            {
                LineSegment frontSeg, rearSeg;
                float d1, d2;
                if(dividees[i].DivideByLine(divider.begin, divider.end, out frontSeg, out rearSeg, out d1, out d2))
                {
                    fronts.Add(frontSeg);
                    rears.Add(rearSeg);
                }
                else if(d1 != 0 && d2 != 0)
                {
                    if (d1 > 0) fronts.Add(dividees[i]);
                    else rears.Add(dividees[i]);
                }
                else
                {
                    collinears.Add(dividees[i]);
                }
            }

            m_items.AddRange(collinears);

            if (fronts.Count > 0)
                m_frontChild = new Node(fronts);
            if (rears.Count > 0)
                m_rearChild = new Node(rears);
        }

        /// <summary>
        /// 按从后向前的顺序进行渲染
        /// </summary>
        public void Render(Vector2 viewPos, List<Node> result)
        {
            if (IsLeaf)
            {
                result.Add(this);
                return;
            }

            float d = m_items[0].SignedDistance(viewPos);
            if(d > 0)
            {
                if (m_rearChild != null) m_rearChild.Render(viewPos, result);
                result.Add(this);
                if (m_frontChild != null) m_frontChild.Render(viewPos, result);
            }
            else if(d < 0)
            {
                if (m_frontChild != null) m_frontChild.Render(viewPos, result);
                result.Add(this);
                if (m_rearChild != null) m_rearChild.Render(viewPos, result);
            }
            else
            {
                if (m_frontChild != null) m_frontChild.Render(viewPos, result);
                if (m_rearChild != null) m_rearChild.Render(viewPos, result);
            }
        }
    }
}
