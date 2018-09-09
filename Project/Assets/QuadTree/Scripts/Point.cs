using UnityEngine;

public class Point {
    public float mX;
    public float mY;
    public Color mColor;
    public Vector2 mMoveDir;

    public Point(float x, float y, float speed)
    {
        mX = x;
        mY = y;
        mColor = Color.blue;
        mMoveDir = new Vector2(Random.Range(0, speed), Random.Range(0, speed));
    }
}
