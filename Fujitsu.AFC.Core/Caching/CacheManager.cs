using System;
using System.Runtime.Caching;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Properties;

namespace Fujitsu.AFC.Core.Caching
{
    public class CacheManager : ICacheManager, IDisposable
    {
        private readonly ObjectCache _cache;
        private readonly int _cacheDuration;
        private static volatile object _cacheLock = new object();

        public CacheManager()
        {
            _cache = MemoryCache.Default;
            _cacheDuration = Settings.Default.DefaultCacheDurationSeconds;
        }

        public TResult ExecuteAndCache<TResult>(string cacheItemKey, Func<TResult> underlyingGet)
        {
            var cacheItem = _cache.GetCacheItem(cacheItemKey);
            if (cacheItem != null)
            {
                return (TResult)cacheItem.Value;
            }

            lock (_cacheLock)
            {
                cacheItem = _cache.GetCacheItem(cacheItemKey);
                if (cacheItem != null)
                {
                    return (TResult)cacheItem.Value;
                }
                return (TResult)CacheItem(cacheItemKey, underlyingGet);
            }
        }

        public void Remove(string key)
        {
            if (_cache.Contains(key))
            {
                lock (_cacheLock)
                {
                    if (_cache.Contains(key))
                    {
                        _cache.Remove(key);
                    }
                }
            }
        }

        private object CacheItem<TResult>(string key, Func<TResult> underlyingGet)
        {
            var result = underlyingGet();
            _cache.Add(key,
                result,
                new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(_cacheDuration)
                });

            return result;
        }

        public void Dispose()
        {
            // Nothing to do.
        }
    }
}
