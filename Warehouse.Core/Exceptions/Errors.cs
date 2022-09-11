using ErrorOr;

namespace Warehouse.Core.Exceptions;

public static partial class Errors
{
    public static class General
    {
        public static Error Unexpected = Error.Unexpected("General.Unexpected");
    }
}
