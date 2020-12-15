using System.Collections.Generic;

namespace BVH
{
    /// <summary>
    /// Bounding Volume Hierarchy
    /// 
    /// 参考：https://box2d.org/files/ErinCatto_DynamicBVH_GDC2019.pdf
    /// 优化操作：
    /// * 物体移动：提供一个稍大的AABB，如果小AABB没有超出大AABB，则不用进行更新，避免物体移动一点就要更新树
    /// * 启发规则：为了加快射线检测等，可将表面积作为启发规则来加速搜索树
    /// * 提高质量：BVH存在和普通二叉树一样的问题，可以使用类似AVL的旋转操作来构造质量更高的树
    /// </summary>
    public class BVH<T>
    {
        private Node<T> m_root;

        public Node<T> Root { get { return m_root; } }

        public void Insert(Item<T> item)
        {
            var newNode = new Node<T>(item);
            if(m_root == null)
            {
                m_root = newNode;
                return;
            }

            Node<T> sibling = PickBest(newNode);
            CreateNewParent(sibling, newNode);
            RefittingAABB(sibling);
        }

        /// <summary>
        /// 找到最好的插入节点
        /// </summary>
        private Node<T> PickBest(Node<T> newNode)
        {
            if (m_root == null)
                return null;

            Node<T> bestNode = null;
            float bestCost = float.MaxValue;

            Queue<Node<T>> queue = new Queue<Node<T>>();
            queue.Enqueue(m_root);
            while (queue.Count > 0)
            {
                var curt = queue.Dequeue();
                float cost = SA(curt, newNode) + SumDeltaSA(curt.Parent, newNode);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestNode = curt;
                }

                //剪枝
                if (!curt.IsLeaf)
                {
                    float lowCost = newNode.Box.Area() + SumDeltaSA(curt, newNode);
                    if (lowCost < bestCost)
                    {
                        queue.Enqueue(curt.Left);
                        queue.Enqueue(curt.Right);
                    }
                }
            }

            return bestNode;
        }

        /// <summary>
        /// 两个节点组合起来的新矩形表面积
        /// </summary>
        private static float SA(Node<T> a, Node<T> b)
        {
            AABB box = a.Box + b.Box;
            return box.Area();
        }

        /// <summary>
        /// 当前节点以及父链上的面积变化和
        /// </summary>
        private static float SumDeltaSA(Node<T> curt, Node<T> newNode)
        {
            float delta = 0;

            while (curt != null)
            {
                AABB box = curt.Box + newNode.Box;
                delta += box.Area() - curt.Box.Area();
                curt = curt.Parent;
            }

            return delta;
        }

        private void CreateNewParent(Node<T> curt, Node<T> newNode)
        {
            var oldParent = curt.Parent;
            Node<T> newParent = new Node<T>(null)
            {
                Parent = oldParent,
                Box = curt.Box + newNode.Box
            };

            if (oldParent != null)
            {
                if (ReferenceEquals(oldParent.Left, curt))
                    oldParent.Left = newParent;
                else
                    oldParent.Right = newParent;
            }
            else
            {
                m_root = newParent;
            }

            newParent.Left = newNode;
            newParent.Right = curt;
            newNode.Parent = newParent;
            curt.Parent = newParent;
        }

        /// <summary>
        /// 沿父节点链一路上去调整AABB
        /// </summary>
        private void RefittingAABB(Node<T> curt)
        {
            var parent = curt.Parent;
            while (parent != null)
            {
                parent.Box = parent.Left.Box + parent.Right.Box;
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// 不同结构的树中根节点和叶子节点的面积都是一样，所以可以跳过这些计算，只算内部节点
        /// </summary>
        private float ComputeCost(Node<T> node)
        {
            if (node.IsLeaf)
                return 0;

            return node.Box.Area() + ComputeCost(node.Left) + ComputeCost(node.Right);
        }
    }
}