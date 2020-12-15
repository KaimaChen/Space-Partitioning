namespace BVH
{
    public class Node<T>
    {
        public Node<T> Parent { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Item<T> Item { get; private set; }
        public AABB Box { get; set; }

        public bool IsLeaf { get { return Left == null; } }

        public Node(Item<T> item)
        {
            Item = item;

            if(item != null)
                Box = item.bound;
        }
    }
}
