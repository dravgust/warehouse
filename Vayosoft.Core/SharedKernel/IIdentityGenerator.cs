using System;

namespace Vayosoft.Core.SharedKernel
{
    public interface IIdentityGenerator : IIdentityGenerator<Guid> { }

    public interface IIdentityGenerator<out T>
    {
        T New();
    }
}
