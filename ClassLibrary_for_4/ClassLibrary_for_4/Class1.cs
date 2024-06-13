using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

namespace ApplicationCache
{
    public class ApplicationCache<T>
    {
        private readonly TimeSpan _expirationTime;
        private readonly uint _maxSize;
        private readonly ConcurrentDictionary<string, CacheItem<T>> _cache = new ConcurrentDictionary<string, CacheItem<T>>();
        private readonly Timer _expirationTimer;

        public ApplicationCache(TimeSpan expirationTime, uint maxSize)
        {
            _expirationTime = expirationTime;
            _maxSize = maxSize;
            _expirationTimer = new Timer(OnExpirationTimerCallback, null, expirationTime, expirationTime);
        }

        public void Save(string key, T data)
        {
            if (_cache.Count >= _maxSize)
            {
                RemoveOldestItem();
            }

            if (!_cache.TryAdd(key, new CacheItem<T>(data)))
            {
                throw new ArgumentException("Key already exists in the cache.");
            }
        }

        public T Get(string key)
        {
            if (_cache.TryGetValue(key, out CacheItem<T> item))
            {
                return item.Data;
            }
            else
            {
                throw new KeyNotFoundException("Key not found in the cache.");
            }
        }

        private void RemoveOldestItem()
        {
            var oldestItemKey = _cache.OrderBy(par => par.Value.CreationTime).First().Key;
            _cache.TryRemove(oldestItemKey, out _);
        }

        private void OnExpirationTimerCallback(object state)
        {
            foreach (var item in _cache)
            {
                if (DateTime.Now - item.Value.CreationTime > _expirationTime)
                {
                    _cache.TryRemove(item.Key, out _);
                }
            }
        }

        private class CacheItem<TValue>
        {
            public TValue Data { get; }
            public DateTime CreationTime { get; }

            public CacheItem(TValue data)
            {
                Data = data;
                CreationTime = DateTime.Now;
            }
        }
    }
}
