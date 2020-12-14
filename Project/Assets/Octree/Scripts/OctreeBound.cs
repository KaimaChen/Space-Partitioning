using UnityEngine;

public struct OctreeBound
{
    public Vector3 center;
    public float size;

    public float xMin { get { return center.x - size / 2; } }
    public float xMax { get { return center.x + size / 2; } }
    public float yMin { get { return center.y - size / 2; } }
    public float yMax { get { return center.y + size / 2; } }
    public float zMin { get { return center.z - size / 2; } }
    public float zMax { get { return center.z + size / 2; } }

    public OctreeBound(float cx, float cy, float cz, float size)
    {
        center = new Vector3(cx, cy, cz);
        this.size = size;
    }

    public OctreeBound(Vector3 center, float size)
    {
        this.center = center;
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