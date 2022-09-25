using System.Text.RegularExpressions;
using FluentValidation;

namespace Vayosoft.Core.Utilities
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                //.NotEmpty()
                .Length(5, 21).WithMessage("Invalid 'Phone Number' length.")
                .Matches("^[\\d]+$", RegexOptions.Compiled)
                .WithMessage("Invalid Phone Number format");

            return options;
        }

        public static IRuleBuilder<T, string> IMEI<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                //.NotEmpty()
                .Length(15).WithMessage("Invalid 'IMEI' length.")
                .Matches("^[0-9]{15}$", RegexOptions.Compiled)
                .WithMessage("Invalid IMEI format.");

            return options;
        }
        public static IRuleBuilder<T, string> IMEIs<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .Matches("^[0-9]{15}(,[0-9]{15})*$", RegexOptions.Compiled)
                .WithMessage("Invalid 'IMEI' format.");

            return options;
        }

        public static IRuleBuilder<T, string> MacAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .Matches(@"^([0-9A-Fa-f]{2}[:-]?){5}([0-9A-Fa-f]{2})$", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                .WithMessage("Invalid 'Mac Address' format.");

            return options;
        }
    }
}
