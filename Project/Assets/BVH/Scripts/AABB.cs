using UnityEngine;

namespace BVH
{
    public struct AABB
    {
        public Vector3 lowerBound;
        public Vector3 upperBound;

        public Vector3 center
        {
            get { return (lowerBound + upperBound) / 2; }
        }

        public Vector3 size
        {
            get { return upperBound - lowerBound; }
        }

        /// <summary>
        /// 表面积
        /// </summary>
        public float Area()
        {
            Vector3 d = upperBound - lowerBound;
            return 2 * (d.x * d.y + d.y * d.z + d.z * d.x);
        }

        public static AABB operator +(AABB a, AABB b)
        {
            return new AABB()
            {
                lowerBound = Vector3.Min(a.lowerBound, b.lowerBound),
                upperBound = Vector3.Max(a.upperBound, b.upperBound)
            };
        }
    }
}