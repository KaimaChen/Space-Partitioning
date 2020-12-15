namespace BVH
{
    public class Item<T>
    {
        public T elem;
        public AABB bound;

        public Item(T elem, AABB bound)
        {
            this.elem = elem;
            this.bound = bound;
        }
    }
}
