using UnityEngine;

namespace BSP
{
    public struct LineSegment
    {
        public Vector2 begin;
        public Vector2 end;

        public LineSegment(Vector2 begin, Vector2 end)
        {
            this.begin = begin;
            this.end = end;
        }

        private float PointToLineSignedDistance(Vector2 p, Vector2 line1, Vector2 line2)
        {
            Vector2 dir = line2 - line1;
            Vector2 leftNormal = new Vector2(-dir.y, dir.x).normalized;
            return Vector2.Dot(p - line1, leftNormal);
        }

        public float SignedDistance(Vector2 p)
        {
            return PointToLineSignedDistance(p, begin, end);
        }

        public bool DivideByLine(Vector2 line1, Vector2 line2, out LineSegment frontSeg, out LineSegment rearSeg, out float d1, out float d2)
        {
            d1 = PointToLineSignedDistance(begin, line1, line2);
            d2 = PointToLineSignedDistance(end, line1, line2);
            if (d1 * d2 <= 0 && d1 != d2) //d1 == d2 == 0时是共线，这时不切割
            {
                float len1 = Mathf.Abs(d1);
                float len2 = Mathf.Abs(d2);

                float k = (len1 + len2 == 0) ? 0.5f : len1 / (len1 + len2);
                var intersectPoint = begin + (end - begin) * k;
                
                if(d1 > 0)
                {
                    frontSeg = new LineSegment(begin, intersectPoint);
                    rearSeg = new LineSegment(intersectPoint, end);
                }
                else
                {
                    frontSeg = new LineSegment(intersectPoint, end);
                    rearSeg = new LineSegment(begin, intersectPoint);
                }

                return true;
            }
            else
            {
                frontSeg = rearSeg = new LineSegment();
                return false;
            }
        }
    }
}
