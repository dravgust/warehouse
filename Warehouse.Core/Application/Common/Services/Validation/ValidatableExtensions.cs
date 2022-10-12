using System.Text.RegularExpressions;
using Throw;

namespace Warehouse.Core.Application.Common.Services.Validation
{
    public static class ValidatableExtensions
    {
        private static readonly Regex Pattern = new(@"^([0-9A-Fa-f]{2}[:-]?){5}([0-9A-Fa-f]{2})$");
        public static ref readonly Validatable<string> IfNotMacAddress(this in Validatable<string> validatable)
        {
            if (!Pattern.IsMatch(validatable.Value))
            {
                ExceptionThrower.Throw(
                    validatable.ParamName,
                    validatable.ExceptionCustomizations,
                    "Invalid MAC Address format");
            }
            return ref validatable;
        }
    }
}
