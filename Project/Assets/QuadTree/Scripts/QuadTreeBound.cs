using UnityEngine;

public struct QuadTreeBound
{
    public Vector2 center;
    public float size;

    public float xMin { get { return center.x - size / 2; } }
    public float xMax { get { return center.x + size / 2; } }
    public float yMin { get { return center.y - size / 2; } }
    public float yMax { get { return center.y + size / 2; } }

    public QuadTreeBound(float cx, float cy, float size)
    {
        center = new Vector2(cx, cy);
        this.size = size;
    }

    public QuadTreeBound(Vector2 center, float size)
    {
        this.center = center;
        this.size = size;
    }

    public bool Overlaps(QuadTreeBound other)
    {
        if (other.xMax < xMin || other.xMin > xMax)
            return false;
        if (other.yMax < yMin || other.yMin > yMax)
            return false;

        return true;
    }

    public bool Contains(QuadTreeBound other)
    {
        return xMin <= other.xMin && xMax >= other.xMax && yMin <= other.yMin && yMax >= other.yMax;
    }
}