using UnityEngine;
using System.Text;

public class KdTree
{
    public const int K = 2;

    public KdNode mRoot;
    
    KdNode InsertHelper(KdNode root, int[] values, int dim)
    {
        if (root == null)
        {
            return new KdNode(values);
        }

        int index = dim % K;

        if(root.IsLess(values, index))
        {
            root.mRight = InsertHelper(root.mRight, values, dim + 1);
        }
        else
        {
            root.mLeft = InsertHelper(root.mLeft, values, dim + 1);
        }

        return root;
    }

    public void Insert(int[] values)
    {
        mRoot = InsertHelper(mRoot, values, 0);
    }

    KdNode FindHelper(KdNode root, int[] values, int dim)
    {
        if (root == null)
            return null;

        if (root.IsSame(values))
            return root;

        int index = dim % K;
        if (root.IsLess(values, index))
            return FindHelper(root.mRight, values, dim + 1);
        else
            return FindHelper(root.mLeft, values, dim + 1);
    }

    public KdNode Find(int[] values)
    {
        return FindHelper(mRoot, values, 0);
    }

    KdNode MinNode(KdNode a, KdNode b, KdNode c, int targetDim)
    {
        KdNode cur = a;
        if (cur == null || (b != null && b.mValues[targetDim] < cur.mValues[targetDim]))
            cur = b;

        if (cur == null || (c != null && c.mValues[targetDim] < cur.mValues[targetDim]))
            cur = c;

        return cur;
    }

    KdNode FindMinHelper(KdNode root, int targetDim, int dim)
    {
        if (root == null)
            return null;

        int index = dim % K;

        if(index == targetDim)
        {
            KdNode leftMin = FindMinHelper(root.mLeft, targetDim, dim + 1);
            return MinNode(root, leftMin, null, targetDim);
        }
        else
        {
            KdNode leftMin = FindMinHelper(root.mLeft, targetDim, dim + 1);
            KdNode rightMin = FindMinHelper(root.mRight, targetDim, dim + 1);
            return MinNode(root, leftMin, rightMin, targetDim);
        }
    }

    public KdNode FindMin(int targetDim)
    {
        return FindMinHelper(mRoot, targetDim, 0);
    }

    KdNode DeleteHelper(KdNode root, int[] values, int dim)
    {
        if (root == null)
            return null;

        int index = dim % K;

        if(root.IsSame(values))
        {
            if(root.mRight != null)
            {
                KdNode minNode = FindMinHelper(root.mRight, index, dim + 1);
                root.SetValues(minNode.mValues);
                root.mRight = DeleteHelper(root.mRight, minNode.mValues, dim + 1);
            }
            else if(root.mLeft != null)
            {
                KdNode minNode = FindMinHelper(root.mLeft, index, dim + 1);
                root.SetValues(minNode.mValues);
                root.mRight = DeleteHelper(root.mLeft, minNode.mValues, dim + 1);
                root.mLeft = null;
            }
            else //left node, direct delete
            {
                return null;
            }
        }
        else if(root.IsLess(values, index))
        {
            root.mRight = DeleteHelper(root.mRight, values, dim + 1);
        }
        else
        {
            root.mLeft = DeleteHelper(root.mLeft, values, dim + 1);
        }

        return root;
    }

    public KdNode Delete(int[] values)
    {
        return DeleteHelper(mRoot, values, 0);
    }

    #region Print
    void PrintHelper(KdNode root, int level, bool isLeft = false)
    {
        if (root == null)
            return;

        StringBuilder sb = new StringBuilder();
        
        for (int i = 0; i < level; i++)
            sb.Append("-");

        if(level > 0)
        {
            if (isLeft)
                sb.Append("L:");
            else
                sb.Append("R:");
        }

        sb.Append(root);

        Debug.Log(sb.ToString());

        PrintHelper(root.mLeft, level + 1, true);
        PrintHelper(root.mRight, level + 1, false);
    }

    public void Print()
    {
        if (mRoot == null)
            Debug.Log("Root is null");

        PrintHelper(mRoot, 0);
    }

    int LINE_WIDTH = 5;
    void ShowGraphHelper(KdNode root, int dim, Material mat, int minX, int maxX, int minY, int maxY)
    {
        if (root == null)
            return;

        mat.SetColor("_Color", Color.red);
        GraphicsTool.DrawPoint(new Vector2(root.mValues[0], root.mValues[1]), 5, mat);
        
        mat.SetColor("_Color", Color.green);

        if (dim %  2 == 0)
        {
            int x = root.mValues[0];
            
            Vector2 begin = new Vector2(x, maxY);
            Vector2 end = new Vector2(x, minY);
            GraphicsTool.DrawLine(begin, end, mat);

            ShowGraphHelper(root.mLeft, dim + 1, mat, minX, x, minY, maxY);
            ShowGraphHelper(root.mRight, dim + 1, mat, x, maxX, minY, maxY);
        }
        else
        {
            int y = root.mValues[1];
            
            Vector2 begin = new Vector2(minX, y);
            Vector2 end = new Vector2(maxX, y);
            GraphicsTool.DrawLine(begin, end, mat);

            ShowGraphHelper(root.mLeft, dim + 1, mat, minX, maxX, minY, y);
            ShowGraphHelper(root.mRight, dim + 1, mat, minX, maxX, y, maxY);
        }
    }

    public void ShowGraph(Material mat)
    {
        if (mRoot == null)
            return;

        ShowGraphHelper(mRoot, 0, mat, 0, 1000, 0, 1000);
    }
    #endregion
}