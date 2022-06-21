namespace IpsWeb.Lib
{
    //https://stackoverflow.com/questions/31138179/asynchronous-locking-based-on-a-key
    /// <summary>
    ///Asynchronous locking based on a string key
    /// </summary>
    public sealed class AsyncLock
    {
        private readonly string _key;

        public AsyncLock(string key) => _key = key;

        private static readonly Dictionary<string, RefCounted<SemaphoreSlim>> SemaphoreSlims = new();

        private static SemaphoreSlim GetOrCreate(string key)
        {
            RefCounted<SemaphoreSlim> item;
            lock (SemaphoreSlims)
            {
                if (SemaphoreSlims.TryGetValue(key, out item))
                {
                    ++item.RefCount;
                }
                else
                {
                    item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    SemaphoreSlims[key] = item;
                }
            }
            return item.Value;
        }

        public static AsyncLock Create(string key) => new(key);

        public async Task<IDisposable> GetLockAsync()
        {
            await GetOrCreate(_key).WaitAsync().ConfigureAwait(false);
            return new ReleaseToken(_key);
        }

        public static async Task UseLockAsync(string key, Func<Task> func)
        {
            using (await Create(key).GetLockAsync())
            {
                await func();
            }
        }
        
        private readonly struct ReleaseToken : IDisposable
        {
            private readonly string _key;

            public ReleaseToken(string key) => _key = key;

            public void Dispose()
            {
                RefCounted<SemaphoreSlim> item;
                lock (SemaphoreSlims)
                {
                    item = SemaphoreSlims[_key];
                    --item.RefCount;
                    if (item.RefCount == 0)
                    {
                        SemaphoreSlims.Remove(_key);
                    }
                }
                item.Value.Release();
            }
        }

        private sealed class RefCounted<T>
        {
            public RefCounted(T value)
            {
                RefCount = 1;
                Value = value;
            }

            public int RefCount { get; set; }
            public T Value { get; }
        }
    }
}
