using UnityEngine;

namespace KdTree
{
    public class Node
    {
        public Vector2 Pos { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(Vector2 p)
        {
            Pos = p;
        }

        public bool IsLess(Vector2 p, int dim)
        {
            if (dim == 0)
                return Pos.x < p.x;
            else
                return Pos.y < p.y;
        }

        public float GetValue(int dim)
        {
            if (dim % 2 == 0)
                return Pos.x;
            else
                return Pos.y;
        }
    }
}