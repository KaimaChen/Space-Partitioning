using UnityEngine;

public struct QuadTreeBound
{
    public Vector2 pos;
    public float size;

    public Vector2 center { get { return pos + new Vector2(size / 2, size / 2); } }
    public float xMin { get { return pos.x; } }
    public float xMax { get { return pos.x + size; } }
    public float yMin { get { return pos.y; } }
    public float yMax { get { return pos.y + size; } }

    public QuadTreeBound(float bottomLeftX, float bottomLeftY, float size)
    {
        pos = new Vector2(bottomLeftX, bottomLeftY);
        this.size = size;
    }

    public QuadTreeBound(Vector2 bottomLeft, float size)
    {
        pos = bottomLeft;
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
}