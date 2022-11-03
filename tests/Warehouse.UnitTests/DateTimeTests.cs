using FluentAssertions;
using NSubstitute;
using Vayosoft.Utilities;

namespace Warehouse.UnitTests
{
    public class DateTimeTests
    {
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        [Fact]
        public void DateTimeProvider_Should_Return_Custom_Value()
        {
            _dateTimeProvider.Now.Returns(new DateTimeOffset(2022, 08, 11, 01, 00, 00, TimeSpan.Zero));

            var dt = _dateTimeProvider.Now;

            dt.Should().Be(new DateTimeOffset(2022, 08, 11, 01, 00, 00, TimeSpan.Zero));
        }
    }
}
