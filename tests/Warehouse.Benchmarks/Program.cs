using BenchmarkDotNet.Running;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Redis;
using Vayosoft.Streaming.Redis.Producers;
using Warehouse.Benchmarks;
using Warehouse.Core.Domain.Events;

var redisProvider = new RedisProvider("localhost:6379,abortConnect=false,ssl=false");
var consumer = new RedisProducer(redisProvider, new RedisProducerConfig() { MaxLength = 10, Topic = "IPS-EVENTS" });
var cts = new CancellationTokenSource();
var task = Task.Run(async () =>
{
    var counter = 0;
    while (!cts.IsCancellationRequested)
    {
        try
        {
            await consumer.Publish(new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, "", counter++));
            Console.WriteLine("sent" + counter);
            await Task.Delay(1000);
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
        }
    }
}, cts.Token);


Console.ReadKey();
cts.Cancel();
await Task.WhenAll(task);
Console.ReadKey();

//BenchmarkRunner.Run<Serialization>();

