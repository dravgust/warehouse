using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisConsumer : IRedisConsumer
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

                var thread = new Thread(() => CreateSubscriber(topic, groupName, consumerName, handler, tokenSource.Token))
                {
                    IsBackground = true,
                };
                thread.Start();

                _logger.LogInformation("[{GroupName}.{ConsumerName}] Subscribed to stream {Topic}", groupName, consumerName, topic);
            }
        }

        public void Close()
        {
            _disposable.Dispose();
            _disposable.Clear();
        }

        private void CreateSubscriber(string streamName, string groupName, string consumerName,
            IObserver<ConsumeResult<string, string>> handler, CancellationToken token)
        {
            _ = CreateMessageSubscriber(streamName, groupName, consumerName, handler, token);
        }

        private async Task CreateMessageSubscriber(string streamName, string groupName, string consumerName,
            IObserver<ConsumeResult<string, string>> handler, CancellationToken token)
        {
            try
            {
                if (!(await _database.KeyExistsAsync(streamName)) ||
                    (await _database.StreamGroupInfoAsync(streamName)).All(x => x.Name != groupName))
                {
                    await _database.StreamCreateConsumerGroupAsync(streamName, groupName, "0-0");
                }

                while (!token.IsCancellationRequested)
                {
                    var streamEntries = await _database.StreamReadGroupAsync(streamName, groupName, consumerName, ">", 1);
                    if (!streamEntries.Any())
                        await Task.Delay(IntervalMilliseconds, token);
                    else
                    {
                        //_logger.LogWarning("*******************************************************");
                        //var pendingInfo = await redisDb.StreamPendingAsync(streamName, groupName);

                        //_logger.LogWarning("COUNT: {Count}\r\nLOW: {Low}\r\nHIGH: {High}\r\nConsumers: {Count} Name: {Name}. Pending mess: {Mess}", 
                        //    pendingInfo.PendingMessageCount.ToString(), pendingInfo.LowestPendingMessageId, pendingInfo.HighestPendingMessageId, pendingInfo.Consumers.Length, pendingInfo.Consumers.First().Name, pendingInfo.Consumers.First().PendingMessageCount.ToString());

                        //var pendingMessages = await redisDb.StreamPendingMessagesAsync(streamName,
                        //    groupName, count: 1, consumerName: consumerName, minId: pendingInfo.LowestPendingMessageId);

                        //_logger.LogWarning("Pending - MessageId: {MessId}, Idle: {Idle}", 
                        //    pendingMessages.Single().MessageId, pendingMessages.Single().IdleTimeInMilliseconds.ToString());
                        //_logger.LogWarning("*******************************************************");

                        var streamEntry = streamEntries.Last();
                        foreach (var nameValueEntry in streamEntry.Values)
                        {
                            try
                            {
                                handler.OnNext(new ConsumeResult<string, string>(streamName, nameValueEntry.Name, nameValueEntry.Value));
                                await _database.StreamAcknowledgeAsync(streamName, groupName, streamEntry.Id);
                            }
                            catch (Exception e) { handler.OnError(e); }
                        }
                    }
                }
            }
            catch (TaskCanceledException) { /*ignore*/}
            catch (Exception e) { handler.OnError(e); }
            finally
            {
                _database.StreamDeleteConsumer(streamName, groupName, consumerName);
                handler.OnCompleted();
            }
        }
    }
}
