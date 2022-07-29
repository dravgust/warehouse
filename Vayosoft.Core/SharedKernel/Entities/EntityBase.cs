namespace Vayosoft.Core.SharedKernel.Entities
{
    public abstract class EntityBase<TId> : IEntity<TId>
    {
        protected EntityBase() { }

        protected EntityBase(TId id) => Id = id;

        protected int? RequestedHashCode;

        public TId Id { get; set; } = default!;

        public bool IsTransient()
        {
            return Id == null || Id.Equals(default(TId));
        }

        object IEntity.Id => Id!;

        public override bool Equals(object obj)
        {
            if (obj is not EntityBase<TId> item)
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
        public static bool operator ==(EntityBase<TId> left, EntityBase<TId> right)
        {
            return left?.Equals(right) ?? object.Equals(right, null);
        }
        public static bool operator !=(EntityBase<TId> left, EntityBase<TId> right)
        {
            return !(left == right);
        }
    }
}
