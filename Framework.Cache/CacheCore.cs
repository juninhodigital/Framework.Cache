using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Caching;

namespace Framework.Cache
{
    [ExcludeFromCodeCoverage]
    public class CacheCore : ICache
    {
        #region| Properties |

        ObjectCache cache = MemoryCache.Default;

        public object this[string key]
        {
            get
            {
                return cache[key];
            }
            set
            {
                lock (cache)
                {
                    cache[key] = value;
                }
            }
        }


        #endregion

        #region| Methods |

        /// <summary>
        /// inserts a cache entry into the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="minutes">AbsoluteExpiration in minutes. Default one minute</param>
        public void Add(string key, object value, double minutes=1)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(minutes)
            };

            lock (cache)
            {
                Add(key, value, policy);
            }
        }

        /// <summary>
        /// inserts a cache entry into the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="policy">CacheItemPolicy</param>
        public void Add(string key, object value, CacheItemPolicy policy)
        {
            lock (cache)
            {
                cache.Add(key, value, policy);
            }
        }


        /// <summary>
        /// removes the cache entry from the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        public void Remove(string key)
        {
            lock (cache)
            {
                cache.Remove(key);
            }
        }

        /// <summary>
        /// checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <returns>true if the cache contains a cache entry with the same key value as key; otherwise, false.</returns>
        public bool Exists(string key)
        {
            return cache.Contains(key);
        }

        /// <summary>
        /// Gets an entry from the System.Runtime.Caching.ObjectCache class
        /// </summary>
        /// <typeparam name="T">param type</typeparam>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)cache[key];
        }

        /// <summary>
        /// Clear all cache entries from the System.Runtime.Caching.ObjectCache
        /// </summary>
        public void Clear()
        {
            var cacheKeys = cache.Select(kvp => kvp.Key).ToList();

            foreach (string cacheKey in cacheKeys)
            {
                cache.Remove(cacheKey);
            }
        }

        #endregion
    }
}
