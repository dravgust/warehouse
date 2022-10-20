using System;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Threading.Attributes;
using Vayosoft.Threading.Utilities;

namespace Vayosoft.Threading.Channels.Handlers
{
    public abstract class AsyncChannelHandlerBase<T> : IDisposable
    {
        protected const int DefaultIdleTimeout = 1000 * 60 * 30;// 30 min
        private const int MaxIdleTimeout = 1000 * 60 * 60 * 8;  // 8 hours
        private volatile int _idleTimeout = DefaultIdleTimeout;
        private DateTime _lastActivityTime = DateTime.Now;

        protected abstract ValueTask Handle(T item, CancellationToken token = default);

        public async ValueTask HandleAction(T item, CancellationToken token = default)
        {
            try
            {
                _lastActivityTime = DateTime.Now;
                await Handle(item, token);
            }
            catch (Exception e)
            {
                if (this.GetType().HasAttribute<ExceptionAttribute>(out var customAttribute))
                {
                    customAttribute.OnException(e);
                }
            }
        }

        protected int IdleTimeout
        {
            set
            {
                if (value == _idleTimeout) return;

                if (value < DefaultIdleTimeout)
                    _idleTimeout = DefaultIdleTimeout;

                if (value > MaxIdleTimeout)
                    _idleTimeout = MaxIdleTimeout;

                _idleTimeout = value;
            }
            get => _idleTimeout;
        }

        public bool CanBeCleared => (DateTime.Now - _lastActivityTime).TotalMilliseconds > IdleTimeout;

        public virtual void Dispose()
        { }
    }
}
