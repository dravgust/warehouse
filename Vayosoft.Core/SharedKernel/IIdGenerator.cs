using System;

namespace Vayosoft.Core.SharedKernel
{
    public interface IIdGenerator : IIdentityGenerator<Guid> { }

    public interface IIdentityGenerator<out T>
    {
        T New();
    }
}
