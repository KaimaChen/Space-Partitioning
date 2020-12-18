using UnityEngine;

namespace KdTree
{
    /// <summary>
    /// kd树，这里实现2维的
    /// 参考：
    /// https://www.cs.cmu.edu/~ckingsf/bioinfo-lectures/kdtrees.pdf
    /// https://courses.cs.washington.edu/courses/cse373/02au/lectures/lecture22l.pdf 这个文章提供了另外一种构建方式
    /// </summary>
    public class KdTree
    {
        public const int K = 2;

        private Node m_root;

        public Node Root { get { return m_root; } }

        public void Insert(Vector2 point)
        {
            m_root = InsertHelper(m_root, point, 0);
        }

        public Node Find(Vector2 point)
        {
            return FindHelper(m_root, point, 0);
        }

        public Node FindMin(int targetDim)
        {
            return FindMinHelper(m_root, targetDim, 0);
        }

        public Node Delete(Vector2 point)
        {
            return DeleteHelper(m_root, point, 0);
        }

        public Node NearestNeighbor(Vector2 src, AABB2 box)
        {
            m_bestSqrDist = float.MaxValue;
            m_bestNode = null;

            NN(m_root, src, 0, box);

            return m_bestNode;
        }

        Node InsertHelper(Node root, Vector2 point, int dim)
        {
            if (root == null)
                return new Node(point);

            int index = dim % K;

            if (root.IsLess(point, index))
            {
                root.Right = InsertHelper(root.Right, point, dim + 1);
            }
            else
            {
                root.Left = InsertHelper(root.Left, point, dim + 1);
            }

            return root;
        }

        Node FindHelper(Node root, Vector2 point, int dim)
        {
            if (root == null)
                return null;

            if (root.Pos == point)
                return root;

            int index = dim % K;
            if (root.IsLess(point, index))
                return FindHelper(root.Right, point, dim + 1);
            else
                return FindHelper(root.Left, point, dim + 1);
        }

        Node MinNode(Node a, Node b, Node c, int targetDim)
        {
            Node cur = a;
            if (cur == null || (b != null && b.GetValue(targetDim) < cur.GetValue(targetDim)))
                cur = b;

            if (cur == null || (c != null && c.GetValue(targetDim) < cur.GetValue(targetDim)))
                cur = c;

            return cur;
        }

        Node FindMinHelper(Node root, int targetDim, int dim)
        {
            if (root == null)
                return null;

            int index = dim % K;

            if (index == targetDim)
            {
                Node leftMin = FindMinHelper(root.Left, targetDim, dim + 1);
                return MinNode(root, leftMin, null, targetDim);
            }
            else
            {
                Node leftMin = FindMinHelper(root.Left, targetDim, dim + 1);
                Node rightMin = FindMinHelper(root.Right, targetDim, dim + 1);
                return MinNode(root, leftMin, rightMin, targetDim);
            }
        }

        Node DeleteHelper(Node root, Vector2 point, int dim)
        {
            if (root == null)
                return null;

            int index = dim % K;

            if (root.Pos == point)
            {
                if (root.Right != null)
                {
                    //把右子树的最小节点作为根节点
                    Node minNode = FindMinHelper(root.Right, index, dim + 1);
                    root.Pos = minNode.Pos;
                    root.Right = DeleteHelper(root.Right, minNode.Pos, dim + 1);
                }
                else if (root.Left != null)
                {
                    //把左子树的最小节点作为根节点，并把剩余左子树移到右子树
                    Node minNode = FindMinHelper(root.Left, index, dim + 1);
                    root.Pos = minNode.Pos;
                    root.Right = DeleteHelper(root.Left, minNode.Pos, dim + 1);
                    root.Left = null;
                }
                else //叶子节点直接移除
                {
                    return null;
                }
            }
            else if (root.IsLess(point, index))
            {
                root.Right = DeleteHelper(root.Right, point, dim + 1);
            }
            else
            {
                root.Left = DeleteHelper(root.Left, point, dim + 1);
            }

            return root;
        }

        private float m_bestSqrDist;
        private Node m_bestNode;
        private void NN(Node curt, Vector2 src, int dim, AABB2 box)
        {
            if (curt == null)
                return;

            if (box.SqrDistance(src) > m_bestSqrDist)
                return;

            float sqrDist = (src - curt.Pos).sqrMagnitude;
            if(sqrDist < m_bestSqrDist)
            {
                m_bestSqrDist = sqrDist;
                m_bestNode = curt;
            }

            //先搜最可能在的地方，从而尽早裁剪掉不可能的地方
            if(GetValue(src, dim) < curt.GetValue(dim))
            {
                NN(curt.Left, src, (dim + 1) % 2, AABB2.TrimLeft(box, curt.GetValue(dim), dim));
                NN(curt.Right, src, (dim + 1) % 2, AABB2.TrimRight(box, curt.GetValue(dim), dim));
            }
            else
            {
                NN(curt.Right, src, (dim + 1) % 2, AABB2.TrimRight(box, curt.GetValue(dim), dim));
                NN(curt.Left, src, (dim + 1) % 2, AABB2.TrimLeft(box, curt.GetValue(dim), dim));
            }
        }

        private static float GetValue(Vector2 p, int dim)
        {
            if (dim % 2 == 0)
                return p.x;
            else
                return p.y;
        }
    }
}