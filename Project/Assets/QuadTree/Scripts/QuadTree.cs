using System.Collections.Generic;

public class QuadTree {
    public AABB mAABB;
    public List<Point> mPoints = new List<Point>();
    public QuadTree[] mChildren = null;
    private int mCapacity;
    
    public QuadTree(AABB aabb, int capacity)
    {
        mAABB = aabb;
        mCapacity = capacity;
    }

    public void Reset()
    {
        mPoints.Clear();
        mChildren = null;
    }

    public bool Insert(Point point)
    {
        if (!mAABB.Contains(point.mX, point.mY))
            return false;

        if(mPoints.Count < mCapacity)
        {
            mPoints.Add(point);
            return true;
        }

        if(mChildren == null)
        {
            Subdivide();
        }

        for(int i = 0; i < 4; i++)
        {
            if (mChildren[i].Insert(point))
                return true;
        }

        return false;
    }

    public void Subdivide()
    {
        float x = mAABB.mCenterX;
        float y = mAABB.mCenterY;
        float w = mAABB.mWidth / 2;
        float h = mAABB.mHeight / 2;

        mChildren = new QuadTree[4];

        AABB aabb = new AABB(x + w / 2, y + h / 2, w, h);
        mChildren[0] = new QuadTree(aabb, mCapacity);

        aabb = new AABB(x - w / 2, y + h / 2, w, h);
        mChildren[1] = new QuadTree(aabb, mCapacity);

        aabb = new AABB(x - w / 2, y - h / 2, w, h);
        mChildren[2] = new QuadTree(aabb, mCapacity);

        aabb = new AABB(x + w / 2, y - h / 2, w, h);
        mChildren[3] = new QuadTree(aabb, mCapacity);
    }

    public void Query(AABB aabb, List<Point> result)
    {
        if (!mAABB.Intersect(aabb))
            return;

        for(int i = 0; i < mPoints.Count; i++)
        {
            if (aabb.Contains(mPoints[i]))
                result.Add(mPoints[i]);
        }

        if(mChildren != null)
        {
            for(int i = 0; i < mChildren.Length; i++)
            {
                mChildren[i].Query(aabb, result);
            }
        }
    }
}
