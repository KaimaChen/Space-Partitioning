public class AABB {
    public float mCenterX;
    public float mCenterY;
    public float mWidth;
    public float mHeight;

    public float Left
    {
        get
        {
            return mCenterX - mWidth / 2;
        }
    }

    public float Right
    {
        get
        {
            return mCenterX + mWidth / 2;
        }
    }

    public float Bottom
    {
        get
        {
            return mCenterY - mHeight / 2;
        }
    }

    public float Top
    {
        get
        {
            return mCenterY + mHeight / 2;
        }
    }

    public AABB(float cx, float cy, float w, float h)
    {
        mCenterX = cx;
        mCenterY = cy;
        mWidth = w;
        mHeight = h;
    }

    public bool Contains(float x, float y)
    {
        return (x >= Left && x <= Right && y >= Bottom && y <= Top);
    }

    public bool Contains(Point point)
    {
        return Contains(point.mX, point.mY);
    }
    
    public bool Intersect(AABB other)
    {
        return !(Left > other.Right || Right < other.Left || Bottom > other.Top || Top < other.Bottom);
    }
}
