using System;

namespace Vayosoft.Core.Utilities
{
    public interface IIdentityGenerator : IIdentityGenerator<Guid> { }

    public interface IIdentityGenerator<out T>
    {
        T New();
    }
}
