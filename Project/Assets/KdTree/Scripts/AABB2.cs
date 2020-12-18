using UnityEngine;

namespace KdTree
{
    public struct AABB2
    {
        public Vector2 min;
        public Vector2 max;

        public AABB2(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public Vector2 ClosestPoint(Vector2 p)
        {
            float x = p.x;
            if (x < min.x) x = min.x;
            else if (x > max.x) x = max.x;

            float y = p.y;
            if (y < min.y) y = min.y;
            else if (y > max.y) y = max.y;

            return new Vector2(x, y);
        }

        public float SqrDistance(Vector2 p)
        {
            return (p - ClosestPoint(p)).sqrMagnitude;
        }

        public static AABB2 TrimRight(AABB2 box, float v, int dim)
        {
            AABB2 result = box;
            if(dim % 2 == 0)
            {
                if (v >= result.min.x && v <= result.max.x)
                    result.min.x = v;
            }
            else
            {
                if (v >= result.min.y && v <= result.max.y)
                    result.min.y = v;
            }

            return result;
        }

        public static AABB2 TrimLeft(AABB2 box, float v, int dim)
        {
            AABB2 result = box;

            if(dim % 2 == 0)
            {
                if (v >= result.min.x && v <= result.max.x)
                    result.max.x = v;
            }
            else
            {
                if (v >= result.min.y && v <= result.max.y)
                    result.max.y = v;
            }

            return result;
        }
    }
}
