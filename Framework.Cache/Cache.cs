
using System.Runtime.Caching;

namespace Framework.Cache
{
    /// <summary>
    ///  Represents an object cache and provides the base methods and properties for accessing the object cache.
    /// </summary>
    public static class Cache
    {
        #region| Properties |

        static CacheCore core = new CacheCore();

        #endregion

        #region| Methods |

        /// <summary>
        /// inserts a cache entry into the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="minutes">AbsoluteExpiration in minutes. Default one minute</param>
        public static void Add(string key, object value, double minutes = 1)
        {
            lock (core)
            {
                core.Add(key, value, minutes);
            }
        }

        /// <summary>
        /// inserts a cache entry into the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="policy">CacheItemPolicy</param>
        public static void Add(string key, object value, CacheItemPolicy policy)
        {
            lock (core)
            {
                core.Add(key, value, policy);
            }
        }

        /// <summary>
        /// removes the cache entry from the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        public static void Remove(string key)
        {
            lock (core)
            {
                core.Remove(key);
            }
        }

        /// <summary>
        /// Checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <returns>true if the cache contains a cache entry with the same key value as key; otherwise, false.</returns>
        public static bool Exists(string key)
        {
            lock (core)
            {
                return core.Exists(key);
            }
        }

        /// <summary>
        /// Updates an entry in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Update(string key, object value)
        {
            lock (core)
            {
                core[key] = value;
            }
        }

        /// <summary>
        /// Gets an entry from the System.Runtime.Caching.ObjectCache class
        /// </summary>
        /// <typeparam name="T">param type</typeparam>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            lock (core)
            {
                return core.Get<T>(key);
            }
        }

        /// <summary>
        /// Clear all cache entries from the System.Runtime.Caching.ObjectCache
        /// </summary>
        public static void Clear()
        {
            lock (core)
            {
                core.Clear();
            }
        }

        #endregion
    }
}
