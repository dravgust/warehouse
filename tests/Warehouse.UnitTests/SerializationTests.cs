using FluentAssertions;
using Newtonsoft.Json;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Domain.Events;
using Xunit.Abstractions;

namespace Warehouse.UnitTests
{
    public class SerializationTests
    {
        private readonly ITestOutputHelper _logger;

        public SerializationTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Fact]
        public void Serialize_Deserialize_By_Newtonsoft()
        {
            IEvent @eventSrc = new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, Guid.NewGuid().ToString(), 0);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var eventJson = JsonConvert.SerializeObject(@eventSrc, settings);
            var @event = (IEvent)JsonConvert.DeserializeObject(eventJson, settings);

            @event.Should().BeOfType<TrackedItemEntered>();
        }

        [Fact]
        public void Serialize_Deserialize_By_Newtonsoft_WithType()
        {
            IEvent @eventSrc = new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, Guid.NewGuid().ToString(), 0);

            var eventType = @eventSrc.GetType();

            var eventJson = JsonConvert.SerializeObject(@eventSrc);
            var @event = (IEvent)JsonConvert.DeserializeObject(eventJson, eventType);

            @event.Should().BeOfType<TrackedItemEntered>();
        }

        [Fact]
        public void Serialize_Deserialize_By_Microsoft_WithType()
        {
            IEvent @eventSrc = new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, Guid.NewGuid().ToString(), 0);

            var eventType = @eventSrc.GetType();

            var eventJson = System.Text.Json.JsonSerializer.Serialize(@eventSrc, eventType);
            var @event = (IEvent)System.Text.Json.JsonSerializer.Deserialize(eventJson, eventType);

            _logger.WriteLine(eventJson);

            @event.Should().BeOfType<TrackedItemEntered>();
        }
    }
}
