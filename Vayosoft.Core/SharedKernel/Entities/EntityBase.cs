namespace Vayosoft.Core.SharedKernel.Entities
{
    public abstract class EntityBase<T> : IEntity<T>
    {
        protected EntityBase() { }

        protected EntityBase(T id) => Id = id;

        protected int? RequestedHashCode;

        public T Id { get; set; } = default!;

        public bool IsTransient()
        {
            return Id == null || Id.Equals(default(T));
        }

        object IEntity.Id => Id;

        public override bool Equals(object? obj)
        {
            if (obj is not EntityBase<T> item)
                return false;
            if (object.ReferenceEquals(this, item))
                return true;
            if (this.GetType() != item.GetType())
                return false;
            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id!.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                RequestedHashCode ??= this.Id!.GetHashCode() ^ 31;
                // XOR for random distribution. See:
                // https://docs.microsoft.com/archive/blogs/ericlippert/guidelines-and-rules-for-gethashcode
                return RequestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }
        public static bool operator ==(EntityBase<T> left, EntityBase<T> right)
        {
            return left?.Equals(right) ?? object.Equals(right, null);
        }
        public static bool operator !=(EntityBase<T> left, EntityBase<T> right)
        {
            return !(left == right);
        }
    }
}
