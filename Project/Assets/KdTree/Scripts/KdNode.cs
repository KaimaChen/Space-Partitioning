using System.Text;
using UnityEngine;

public class KdNode
{
    public int[] mValues;
    public KdNode mLeft;
    public KdNode mRight;

    public KdNode(int[] values)
    {
        mValues = new int[KdTree.K];
        SetValues(values);
    }

    public void SetValues(int[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            mValues[i] = values[i];
        }
    }

    public bool IsSame(int[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (mValues[i] != values[i])
                return false;
        }

        return true;
    }

    public bool IsLess(int[] values, int dim)
    {
        return mValues[dim] < values[dim];
    }
    
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < mValues.Length; i++)
        {
            sb.Append(mValues[i]);
            if (i != mValues.Length - 1)
                sb.Append(",");
        }
        sb.Append("]");
        return sb.ToString();
    }
}