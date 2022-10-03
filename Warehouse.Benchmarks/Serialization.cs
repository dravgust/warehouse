using System.Text;
using BenchmarkDotNet.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Entities.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Warehouse.Benchmarks
{
    [MemoryDiagnoser]
    public class Serialization
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //WriteIndented = true
        };

        private readonly JsonSerializerSettings _newtonsoftOptions = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private List<Person> _persons = new();

        [GlobalSetup]
        public void Setup()
        {
            Randomizer.Seed = new Random(200);
            _persons = new Faker<Person>()
                .RuleFor(p => p.Id, faker => faker.IndexFaker)
                .RuleFor(p => p.Name, faker => faker.Person.FullName)
                .Generate(1000);
        }

        //[BenchmarkCategory("Stream"), Benchmark]
        //public MemoryStream GeneratedSerializer()
        //{
        //    var memoryStream = new MemoryStream();
        //    var jsonWriter = new Utf8JsonWriter(memoryStream);
        //    JsonSerializer.Serialize(jsonWriter, _persons, PersonJsonContext.Default.IEnumerablePerson);
        //    return memoryStream;
        //}

        //[BenchmarkCategory("String"), Benchmark]
        //public string GeneratedSerializer_AsString()
        //{
        //    var memoryStream = GeneratedSerializer();
        //    return Encoding.UTF8.GetString(memoryStream.ToArray());
        //}

        //[BenchmarkCategory("Stream"), Benchmark(Baseline = true)]
        //public MemoryStream ClassicSerializer()
        //{
        //    var memoryStream = new MemoryStream();
        //    var jsonWriter = new Utf8JsonWriter(memoryStream);
        //    JsonSerializer.Serialize(jsonWriter, _persons, _options);
        //    return memoryStream;
        //}

        //[BenchmarkCategory("String"), Benchmark]
        //public string ClassicSerializer_AsString()
        //{
        //    var memoryStream = ClassicSerializer();
        //    return Encoding.UTF8.GetString(memoryStream.ToArray());
        //}

        //[Benchmark]
        //public string Serialize_TextJson_SourceGenerator()
        //{
        //    return JsonSerializer.Serialize(_persons, PersonJsonContext.Default.IEnumerablePerson);
        //}

        //[Benchmark]
        //public string Serialize_TextJson_Options()
        //{
        //    return JsonSerializer.Serialize(_persons, _options);
        //}

        //[Benchmark]
        //public string Serialize_NewtonsoftJson()
        //{
        //    return JsonConvert.SerializeObject(_persons, _newtonsoftOptions);
        //}

        private static readonly IEvent EventSrc = new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, Guid.NewGuid().ToString(), 0);

        [Benchmark]
        public IEvent Serialize_Deserialize_By_Newtonsoft()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var eventJson = JsonConvert.SerializeObject(EventSrc, settings);
            return (IEvent)JsonConvert.DeserializeObject(eventJson, settings);
        }

        [Benchmark]
        public IEvent Serialize_Deserialize_By_Newtonsoft_WithType()
        {
            var eventType = EventSrc.GetType();

            var eventJson = JsonConvert.SerializeObject(EventSrc);
            return (IEvent)JsonConvert.DeserializeObject(eventJson, eventType);
        }

        [Benchmark]
        public IEvent Serialize_Deserialize_By_Microsoft_WithType()
        {
            var eventType = EventSrc.GetType();

            var eventJson = JsonSerializer.Serialize(EventSrc);
            return (IEvent)JsonSerializer.Deserialize(eventJson, eventType);
        }
    }

    [JsonSerializable(typeof(IEnumerable<Person>))]
    [JsonSourceGenerationOptions(
        GenerationMode = JsonSourceGenerationMode.Default,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class PersonJsonContext : JsonSerializerContext
    { }

}
