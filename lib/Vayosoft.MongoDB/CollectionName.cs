using Pluralize.NET.Core;
using Vayosoft.Core.Mapping;
using Vayosoft.Core.Utilities;

namespace Vayosoft.MongoDB
{
    using static String;

    /// <summary>
    /// Convention based mongoDb collection name.
    /// Returns a camelCase collection from any type while removing some forbidden suffixes:
    /// "Document", "Entity", "View", "Projection", "ProjectionDocument", "ProjectionEntity"
    /// </summary>
    public class CollectionName : IEquatable<CollectionName>
    {
        public static readonly CollectionName Default = new("");
        private static readonly Pluralizer Pluralizer = new();

        private readonly string _value;

        private CollectionName(string value) => _value = value;

        public bool Equals(CollectionName other) => string.Equals(_value, other?._value, StringComparison.Ordinal);

        public static CollectionName For<T>(string prefix = null) => For(typeof(T), prefix);

        public static CollectionName For(Type type, string prefix = null)
        {
            var attributeName = type.GetAttributeValue((CollectionNameAttribute entity) => entity.Name);
            if (!IsNullOrWhiteSpace(attributeName))
                return new CollectionName(attributeName);

            var collectionName = type.Name;
            var suffixes = new[]
                {"Document", "Entity", "View", "Projection", "ProjectionDocument", "ProjectionEntity"};

            foreach (var suffix in suffixes)
                if (collectionName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    collectionName = collectionName[..^suffix.Length];
            collectionName = Pluralizer.Pluralize(collectionName).ToLowerFirstChar();

            if (!IsNullOrWhiteSpace(prefix)) collectionName = $"{prefix}-{collectionName}";

            return new CollectionName(collectionName);
        }

        public override bool Equals(object obj) => obj is CollectionName name && Equals(name);

        public static bool operator ==(CollectionName left, CollectionName right) => Equals(left, right);
        public static bool operator !=(CollectionName left, CollectionName right) => !Equals(left, right);

        public override int GetHashCode() => _value == null ? 0 : _value.GetHashCode();

        public override string ToString() => _value ?? "";

        public static implicit operator string(CollectionName self) => self.ToString();
        public static implicit operator CollectionName(string value) => IsNullOrWhiteSpace(value) ? Default : new CollectionName(value);
    }
}
