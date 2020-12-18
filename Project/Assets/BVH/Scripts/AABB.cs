using UnityEngine;

namespace BVH
{
    public struct AABB
    {
        public Vector3 min;
        public Vector3 max;

        public Vector3 center
        {
            get { return (min + max) / 2; }
        }

        public Vector3 size
        {
            get { return max - min; }
        }

        /// <summary>
        /// 表面积
        /// </summary>
        public float Area()
        {
            Vector3 d = max - min;
            return 2 * (d.x * d.y + d.y * d.z + d.z * d.x);
        }

        public static AABB operator +(AABB a, AABB b)
        {
            return new AABB()
            {
                min = Vector3.Min(a.min, b.min),
                max = Vector3.Max(a.max, b.max)
            };
        }
    }
}