using System.Text.RegularExpressions;
using FluentAssertions;

namespace Warehouse.UnitTests
{
    public class ValidationRulesTest
    {
        [Fact]
        public void Check_MacAddress()
        {
            const string pattern = @"^([0-9A-Fa-f]{2}[:-]?){5}([0-9A-Fa-f]{2})$";
            var matches = Regex.Match("DD340206CB76", pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            matches.Success.Should().Be(true);
        }
    }
}
