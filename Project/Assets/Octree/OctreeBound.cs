using UnityEngine;

public struct OctreeBound
{
    public Vector3 pos;
    public float size;

    public Vector3 center { get { return pos + new Vector3(size / 2, size / 2, size / 2); } }
    public float xMin { get { return pos.x; } }
    public float xMax { get { return pos.x + size; } }
    public float yMin { get { return pos.y; } }
    public float yMax { get { return pos.y + size; } }
    public float zMin { get { return pos.z; } }
    public float zMax { get { return pos.z + size; } }

    public OctreeBound(float bottomLeftX, float bottomLeftY, float bottomLeftZ, float size)
    {
        pos = new Vector3(bottomLeftX, bottomLeftY, bottomLeftZ);
        this.size = size;
    }

    public OctreeBound(Vector3 bottomLeft, float size)
    {
        this.pos = bottomLeft;
        this.size = size;
    }

    public bool Overlaps(OctreeBound other)
    {
        if (other.xMax < xMin || other.xMin > xMax)
            return false;
        if (other.yMax < yMin || other.yMin > yMax)
            return false;
        if (other.zMax < zMin || other.zMin > zMax)
            return false;

        return true;
    }
}