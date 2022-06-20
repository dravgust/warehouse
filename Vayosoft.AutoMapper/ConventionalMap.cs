using System;

namespace Vayosoft.AutoMapper
{
    public enum MapDirection
    {
        EntityToDto, DtoToEntity, Both
    }

    public class ConventionalMapAttribute : Attribute
    {
        public MapDirection Direction { get; set; }

        public Type EntityType { get; }

        public ConventionalMapAttribute(Type entityType, MapDirection direction = MapDirection.Both)
        {
            Direction = direction;
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }
    }
}
