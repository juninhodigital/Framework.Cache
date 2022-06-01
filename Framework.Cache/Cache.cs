using System;
using System.Runtime.Caching;

namespace Framework.Cache
{
    /// <summary>
    /// It provides the base methods and properties for accessing the object cache using Runtime caching capabilities
    /// </summary>
    public static class Cache
    {
        #region| Properties |

        private static readonly RuntimeEngine runtime = null;

        #endregion

        #region| Constructor | 

        /// <summary>
        /// Static constructor
        /// </summary>
        static Cache()
        {
            runtime = new RuntimeEngine();
        } 

        #endregion

        #region| Methods |

        /// <summary>
        /// Set the Redis Connection string
        /// </summary>
        /// <param name="connectionString">Redis ConnectionString</param>
        public static void SetConnection(string connectionString) => RedisEngine.SetConnection(connectionString);

        /// <summary>
        /// inserts a cache entry into the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="cacheType">CacheType (Default: Redis)</param>
        /// <param name="minutes">AbsoluteExpiration in minutes. Default one minute</param>
        public static void Add<T>(string key, T value, CacheType cacheType = CacheType.Redis, double minutes = 1) where T: class
        {
            key.Validate();

            AddString(key, value.ToJSON(), cacheType, minutes);
        }

        /// <summary>
        /// inserts a cache entry into the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="cacheType">CacheType (Default: Redis)</param>
        /// <param name="minutes">AbsoluteExpiration in minutes. Default one minute</param>
        private static void AddString(string key, string value, CacheType cacheType = CacheType.Redis, double minutes = 1)
        {
            key.Validate();

            if (cacheType == CacheType.Runtime)
            {
                lock (runtime)
                {
                    runtime.Add(key, value, minutes);
                }
            }
            else
            {
                var expiration = System.TimeSpan.FromMinutes(minutes);

                RedisEngine.Add(key, value, expiration);
            }
        }

        /// <summary>
        ///  Add an object entry serialized in JSON format in the REDIS CACHE based on a given ke. If key already holds a value, it is overwritten,regardless of its type.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="cacheType">CacheType (Default: Redis)</param>
        /// <param name="expirationTimeout">Timeout in minute to the cache to expire</param>
        public static async Task AddAsync<T>(string key, T value, CacheType cacheType = CacheType.Redis,  TimeSpan? expirationTimeout = null) where T : class
        {
            key.Validate();


            if (cacheType == CacheType.Redis)
            {
                await RedisEngine.AddAsync(key, value.ToJSON(), expirationTimeout);
            }
            else
            {
                // Runtime memory cache does not provide an async method
                await Task.Run(() =>
                {
                    runtime.Add(key, value);
                });
            }
        }

        /// <summary>
        /// inserts a cache entry into the RUNTIME CACHE based on a policy
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert</param>
        /// <param name="policy">CacheItemPolicy</param>
        public static void Add(string key, object value, CacheItemPolicy policy)
        {
            lock (runtime)
            {
                runtime.Add(key, value, policy);
            }
        }

        /// <summary>
        ///  Add an object entry serialized in JSON format in the RUNTIME CACHE based on a given ke. If key already holds a value, it is overwritten,regardless of its type.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="policy">CacheItemPolicy</param>
        public static async Task AddAsync<T>(string key, T value, CacheItemPolicy policy) where T : class
        {
            key.Validate();

            // Runtime memory cache does not provide an async method
            await Task.Run(() =>
            {
                lock (runtime)
                {
                    runtime.Add(key, value, policy);
                }
            });
        }

        /// <summary>
        /// removes the cache entry from the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <param name="cacheType">CacheType</param>
        public static bool Remove(string key, CacheType cacheType = CacheType.Redis)
        {
            key.Validate();

            if (cacheType == CacheType.Runtime)
            {
                lock (runtime)
                {
                    return runtime.Remove(key);
                }
            }
            else
            {
                return RedisEngine.Remove(key);
            }
        }

        /// <summary>
        ///  Removes the specified key from the REDIS CACHE. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns>True if the key was removed.</returns>
        public static async Task<bool> RemoveAsync(string key, CacheType cacheType = CacheType.Redis)
        {
            key.Validate();

            if(cacheType == CacheType.Redis)
            {
                return await RedisEngine.RemoveAsync(key);
            }
            else
            {
                // Runtime memory cache does not provide an async method
                return await Task.Run(()=>
                {
                    return runtime.Remove(key);
                });
            }
        }

        /// <summary>
        /// Checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns>true if the cache contains a cache entry with the same key value as key; otherwise, false.</returns>
        public static bool Exists(string key, CacheType cacheType = CacheType.Redis)
        {
            key.Validate();

            if (cacheType == CacheType.Runtime)
            {
                lock (runtime)
                {
                    return runtime.Exists(key);
                }
            }
            else
            {
                return RedisEngine.Exists(key);
            }
        }

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns>1 if the key exists. 0 if the key does not exist</returns>
        public static async Task<bool> ExistsAsync(string key, CacheType cacheType = CacheType.Redis)
        {
            key.Validate();

            if (cacheType == CacheType.Redis)
            {
                return await RedisEngine.ExistsAsync(key);
            }
            else
            {
                // Runtime memory cache does not provide an async method
                return await Task.Run(() =>
                {
                    return runtime.Exists(key);
                });
            }
        }

        /// <summary>
        /// Gets an entry from the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns>cached item</returns>
        public static string Get(string key, CacheType cacheType = CacheType.Redis)
        {
            key.Validate();

            if (cacheType == CacheType.Runtime)
            {
                lock (runtime)
                {
                    return runtime.Get<string>(key);
                }
            }
            else
            {
                return RedisEngine.Get(key);
            }
        }

        /// <summary>
        /// Gets an entry from the cache
        /// </summary>
        /// <typeparam name="T">param type</typeparam>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns>cached item</returns>
        public static T Get<T>(string key, CacheType cacheType = CacheType.Redis) where T:class
        {
            return Get(key, cacheType).FromJSON<T>();
        }

        /// <summary>
        ///  Get the value of key from the REDIS CACHE. If the key does not exist the special value nil is returned.An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns> the value of key, or null when key does not exist.
        public static async Task<string> GetAsync(string key, CacheType cacheType = CacheType.Redis)
        {
            key.Validate();

            if (cacheType == CacheType.Redis)
            {
                return await RedisEngine.GetAsync(key);
            }
            else
            {
                // Runtime memory cache does not provide an async method
                return await Task.Run(() =>
                {
                    return runtime.Get<string>(key);
                });
            }
        }

        /// <summary>
        ///  Get the value of key from the REDIS CACHE. If the key does not exist the special value nil is returned.An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <typeparam name="T">param type</typeparam>
        /// <param name="key">key</param>
        /// <param name="cacheType">CacheType</param>
        /// <returns> the value of key, or null when key does not exist.
        public static async Task<T> GetAsync<T>(string key, CacheType cacheType = CacheType.Redis) where T:class
        {
            key.Validate();

            if (cacheType == CacheType.Redis)
            {
                return await RedisEngine.GetAsync<T>(key);
            }
            else
            {
                // Runtime memory cache does not provide an async method
                return await Task.Run(() =>
                {
                    return runtime.Get<T>(key);
                });
            }
        }

        /// <summary>
        /// Clear all cache entries from the System.Runtime.Caching.ObjectCache
        /// </summary>
        /// <param name="cacheType">CacheType</param>
        public static void Clear(CacheType cacheType = CacheType.Redis)
        {
            if (cacheType == CacheType.Runtime)
            {
                lock (runtime)
                {
                    runtime.Clear();
                }
            }
            else
            {
                RedisEngine.Clear();
            }
        }

        #endregion
    }
}
