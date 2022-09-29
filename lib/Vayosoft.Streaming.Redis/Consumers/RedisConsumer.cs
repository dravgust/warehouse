using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public class RedisConsumer : IRedisConsumer
    {
        private const int IntervalMilliseconds = 500;

        private readonly ILogger<RedisConsumer> _logger;
        private readonly RedisStreamConsumerConfig _config;
        private readonly IDatabase _database;
        private readonly CompositeDisposable _disposable = new();
  
        public RedisConsumer(IRedisDatabaseProvider connection, IConfiguration configuration, ILogger<RedisConsumer> logger)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _config = configuration.GetRedisConsumerConfig();

            _logger = logger;

            _database = connection.Database;
        }

        public void Subscribe(string[] topics, Action<ConsumeResult<string, string>> action, CancellationToken cancellationToken)
        {
            var consumerName = _config?.ConsumerId ?? Guid.NewGuid().ToString();
            var groupName = _config?.GroupId ?? consumerName;
 
            foreach (var topic in topics)
            {
                var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _disposable.Add(Disposable.Create(() =>
                {
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                }));

                var handler = new AnonymousObserver<ConsumeResult<string, string>>(
                onNext: action,
                onCompleted: () => _logger.LogInformation("[{GroupName}.{ConsumerName}] Unsubscribed from stream {Topics}", groupName, consumerName, topics),
                onError: (e) => _logger.LogError("{Message}\r\n{StackTrace}", e.Message, e.StackTrace));

                CreateMessageSubscriber(_database, topic, groupName, consumerName, handler, tokenSource.Token, IntervalMilliseconds);

                _logger.LogInformation("[{GroupName}.{ConsumerName}] Subscribed to stream {Topic}", groupName, consumerName, topic);
            }
        }

        public void Close()
        {
            _disposable.Dispose();
            _disposable.Clear();
        }

        private static void CreateMessageSubscriber(IDatabase redisDb, string streamName,
            string groupName, string consumerName, IObserver<ConsumeResult<string, string>> o, CancellationToken cancellationToken, int intervalMilliseconds = 500)
        {
            Task.Factory.StartNew(async () =>
            {
                //try
                //{ redisDb.StreamCreateConsumerGroup(streamName, groupName); }
                //catch (RedisServerException e) when (e.Message.StartsWith("BUSYGROUP"))
                //{ /*BUSYGROUP Consumer Group name already exists*/ }

                try
                {
                    if (!(await redisDb.KeyExistsAsync(streamName)) ||
                        (await redisDb.StreamGroupInfoAsync(streamName)).All(x => x.Name != groupName))
                    {
                        await redisDb.StreamCreateConsumerGroupAsync(streamName, groupName, "0-0", true);
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var streamEntries = await redisDb.StreamReadGroupAsync(streamName, groupName, consumerName);
                        if (!streamEntries.Any())
                            await Task.Delay(intervalMilliseconds, cancellationToken);
                        else
                        {
                            foreach (var streamEntry in streamEntries)
                            foreach (var nameValueEntry in streamEntry.Values)
                            {
                                try
                                {
                                    o.OnNext(new ConsumeResult<string, string>(streamName, nameValueEntry.Name, nameValueEntry.Value));
                                    await redisDb.StreamAcknowledgeAsync(streamName, groupName, streamEntry.Id);
                                }
                                catch (Exception e) { o.OnError(e); }
                            }
                        }
                    }
                }
                catch (TaskCanceledException) { /*ignore*/}
                catch (Exception e) { o.OnError(e); }
                finally
                {
                    redisDb.StreamDeleteConsumer(streamName, groupName, consumerName);
                }

                o.OnCompleted();
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }
    }
}
