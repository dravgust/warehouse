using System;

namespace Vayosoft.Core.Helpers
{
    public interface IIdentityGenerator : IIdentityGenerator<Guid> { }

    public interface IIdentityGenerator<out T>
    {
        T New();
    }
}
