using System;

namespace Vayosoft.Core.SharedKernel.Exceptions
{
    public class AggregateNotFoundException : ApplicationException
    {
        public AggregateNotFoundException(string typeName, object id)
            : base($"{typeName} with id '{id}' was not found")
        {

        }

        public static AggregateNotFoundException For<T>(object id)
        {
            return new AggregateNotFoundException(typeof(T).Name, id);
        }
    }
}
