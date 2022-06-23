using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Extensions;
using Vayosoft.Data.Redis;

namespace Vayosoft.Caching.Redis
{
    public class RedisMemoryCache : MemoryCacheWrapper
    {
        private static string InstanceId { get; } = $"{Environment.MachineName}_{Guid.NewGuid():N}";
        private readonly IRedisSubscriberProvider _bus;
        private readonly CachingOptions _cachingOptions;
        private readonly RedisCachingOptions _redisCachingOptions;
        private readonly IRedisConnectionProvider _connectionProvider;
        private readonly ILogger<MemoryCacheWrapper> _log;
        private bool _isSubscribed;
        private readonly object _lock = new();
        private bool _disposed;

        public RedisMemoryCache(
            IMemoryCache memoryCache,
            IRedisConnectionProvider connectionProvider,
            IRedisSubscriberProvider pubSub,
            IOptions<CachingOptions> cachingOptions,
            IOptions<RedisCachingOptions> redisCachingOptions,
            ILogger<MemoryCacheWrapper> log) : base(memoryCache, cachingOptions, log)
        {
            _connectionProvider = connectionProvider;
            _bus = pubSub;
            _log = log;

            _cachingOptions = cachingOptions.Value;
            _redisCachingOptions = redisCachingOptions.Value;

            CancellableCacheRegion.OnTokenCancelled = CacheCancellableTokensRegistry_OnTokenCancelled;
        }

        public override bool TryGetValue(object key, out object value)
        {
            //We can't do subscription in the ctor due to the fact that it can be called multiple times despite the fact that it registered as a singleton.
            //So we have delayed the connection and subscription to the Redis server until the first cache call.
            EnsureRedisServerConnection();
            return base.TryGetValue(key, out value);
        }

        private void CacheCancellableTokensRegistry_OnTokenCancelled(TokenCancelledEventArgs e)
        {
            var message = new RedisCachingMessage { InstanceId = InstanceId, IsToken = true, CacheKeys = new[] { e.TokenKey } };
            Publish(message);
            _log.LogTrace($"Published token cancellation message {message.ToString()}");
        }

        protected virtual void OnConnectionFailed(object? sender, ConnectionFailedEventArgs e)
        {
            _log.LogError($"Redis disconnected from instance {InstanceId}. Endpoint is {e.EndPoint}, failure type is {e.FailureType}");

            // If we have no connection to Redis, we can't invalidate cache on another platform instances,
            // so the better idea is to disable cache at all for data consistence
            CacheEnabled = false;
            // We should fully clear cache because we don't know
            // what's changed until platform found Redis is unavailable
            GlobalCacheRegion.ExpireRegion();
        }

        protected virtual void OnConnectionRestored(object? sender, ConnectionFailedEventArgs e)
        {
            _log.LogTrace($"Redis backplane connection restored for instance {InstanceId}");

            // Return cache to the same state as it was initially.
            // Don't set directly true because it may be disabled in app settings
            CacheEnabled = _cachingOptions.CacheEnabled;
            // We should fully clear cache because we don't know
            // what's changed in another instances since Redis became unavailable
            GlobalCacheRegion.ExpireRegion();
        }

        protected virtual void OnMessage(RedisChannel channel, RedisValue redisValue)
        {
            var message = JsonSerializer.Deserialize<RedisCachingMessage>(redisValue!);

            if (!string.IsNullOrEmpty(message?.InstanceId) && !message.InstanceId.EqualsInvariant(InstanceId))
            {
                _log.LogTrace($"Received message {message}");

                foreach (var key in message.CacheKeys?.OfType<string>() ?? Array.Empty<string>())
                {
                    if (message.IsToken)
                    {
                        _log.LogTrace($"Trying to cancel token with key: {key}");
                        CancellableCacheRegion.CancelForKey(key, propagate: false);
                    }
                    else
                    {
                        _log.LogTrace($"Trying to remove cache entry with key: {key} from in-memory cache");
                        base.Remove(key);
                    }
                }
            }
        }

        protected override void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = new RedisCachingMessage { InstanceId = InstanceId, CacheKeys = new[] { key } };
            Publish(message);
            _log.LogTrace($"Published message {message} to the Redis backplane");

            base.EvictionCallback(key, value, reason, state);
        }

        private void Publish(RedisCachingMessage message)
        {
            EnsureRedisServerConnection();
            _bus.Subscriber.Publish(_redisCachingOptions.ChannelName, JsonSerializer.Serialize(message), CommandFlags.FireAndForget);
        }

        private void EnsureRedisServerConnection()
        {
            if (!_isSubscribed)
            {
                lock (_lock)
                {
                    if (!_isSubscribed)
                    {
                        _connectionProvider.Connection.ConnectionFailed += OnConnectionFailed;
                        _connectionProvider.Connection.ConnectionRestored += OnConnectionRestored;

                        _bus.Subscriber.Subscribe(_redisCachingOptions.ChannelName, OnMessage, CommandFlags.FireAndForget);

                        _log.LogTrace($"Successfully subscribed to Redis backplane channel {_redisCachingOptions.ChannelName} with instance id:{InstanceId}");
                        _isSubscribed = true;
                    }
                }
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _bus.Subscriber.Unsubscribe(_redisCachingOptions.ChannelName, null, CommandFlags.FireAndForget);
                    _connectionProvider.Connection.ConnectionFailed -= OnConnectionFailed;
                    _connectionProvider.Connection.ConnectionRestored -= OnConnectionRestored;
                }
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
