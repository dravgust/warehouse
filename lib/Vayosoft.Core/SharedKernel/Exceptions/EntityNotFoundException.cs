using System;

namespace Vayosoft.Core.SharedKernel.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string typeName, object id)
            : base($"Entity \"{typeName}\" ({id}) was not found.")
        {
        }

        public static EntityNotFoundException For<T>(object id)
        {
            return new EntityNotFoundException(typeof(T).Name, id);
        }
    }
}
