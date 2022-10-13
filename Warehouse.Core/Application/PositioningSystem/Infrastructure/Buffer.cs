namespace Warehouse.Core.Application.PositioningSystem.Infrastructure
{
    [Serializable]
    public class Buffer<T> : LinkedList<T>
    {
        private readonly int _capacity;

        public Buffer(int capacity) => _capacity = capacity;

        public void Enqueue(T item)
        {
            // todo: add synchronization mechanism
            if (Count == _capacity) RemoveFirst();
            AddLast(item);
        }

        public T Dequeue()
        {
            if (First is null) return default;

            // todo: add synchronization mechanism
            var first = First.Value;
            RemoveFirst();
            return first;
        }
    }
}
