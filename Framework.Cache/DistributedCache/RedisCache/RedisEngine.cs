using System;
using System.Threading.Tasks;

using Framework.Core;
using StackExchange.Redis;

namespace Framework.Cache
{
    /// <summary>
    /// This class contain useful methods to get or set items at the Redis Distributed Cache
    /// </summary>
    internal static class RedisEngine
    {
        #region| Fields |

        private static string RedisConnection = "";
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>();

        #endregion

        #region| Properties |

        /// <summary>
        /// Redis Connection
        /// </summary>
        private static ConnectionMultiplexer CacheConnection
        {
            get
            {
                if (RedisConnection.IsNull())
                {
                    throw new ArgumentNullException("The redis connection is null or empty");
                }

                return lazyConnection.Value;
            }
        }

        #endregion

        #region| Methods |

        /// <summary>
        /// Set the Redis Connection string
        /// </summary>
        /// <param name="connectionString">Redis ConnectionString</param>
        public static void SetConnection(string connectionString)
        {
            RedisConnection = connectionString;

            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(RedisConnection);
            });
        }

        /// <summary>
        /// Check whether the redis connection string has been initialized
        /// </summary>
        /// <returns>true if the connection string is properly set up</returns>
        public static bool HasConnectionString() => RedisConnection.IsNotNull();

        /// <summary>
        ///  Obtain an interactive connection to a database inside redis
        /// </summary>
        /// <returns></returns>
        private static IDatabase DB
        {
            get 
            {
                RedisConnection.ThrowIfNull("The redis connection is null or empty");

                return CacheConnection.GetDatabase();
            } 
        }

        /// <summary>
        ///  Set key to hold the string value. If key already holds a value, it is overwritten,
        //   regardless of its type.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expirationTimeout">Timeout in minute to the cache to expire</param>
        public static async Task AddAsync(string key, string value, TimeSpan? expirationTimeout = null)
        {
            await DB.StringSetAsync(key, value, expirationTimeout);
        }

        /// <summary>
        ///  Set key to hold the string value. If key already holds a value, it is overwritten,
        //   regardless of its type.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expirationTimeout">Timeout in minute to the cache to expire</param>
        public static void Add(string key, string? value, TimeSpan? expirationTimeout = null)
        {
            DB.StringSet(key, value, expirationTimeout);
        }

        /// <summary>
        ///  Get the value of key. If the key does not exist the special value nil is returned.
        //   An error is returned if the value stored at key is not a string, because GET
        //   only handles string values.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns> the value of key, or null when key does not exist.
        public static async Task<string> GetAsync(string key)
        {
            return await DB.StringGetAsync(key);
        }

        /// <summary>
        ///  Get the value of key. If the key does not exist the special value nil is returned.
        //   An error is returned if the value stored at key is not a string, because GET
        //   only handles string values.
        /// </summary>
        /// <typeparam name="T">param type</typeparam>
        /// <param name="key">key</param>
        /// <returns> the value of key, or null when key does not exist.
        public static async Task<T> GetAsync<T>(string key) where T:class
        {
            var content = await GetAsync(key);

            return content.FromJSON<T>();
        }

        /// <summary>
        ///   Get the value of key. If the key does not exist the special value nil is returned.
        //     An error is returned if the value stored at key is not a string, because GET
        //     only handles string values.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns> the value of key, or null when key does not exist.
        public static string Get(string key)
        {
            return DB.StringGet(key);
        }

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>1 if the key exists. 0 if the key does not exist</returns>
        public static async Task<bool> ExistsAsync(string key)
        {
            return await DB.KeyExistsAsync(key);
        }

        /// <summary>
        /// Returns if key exists.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>1 if the key exists. 0 if the key does not exist</returns>
        public static bool Exists(string key)
        {
            return DB.KeyExists(key);
        }

        /// <summary>
        ///  Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True if the key was removed.</returns>
        public static async Task<bool> RemoveAsync(string key)
        {
            return await DB.KeyDeleteAsync(key);
        }

        /// <summary>
        ///  Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True if the key was removed.</returns>
        public static bool Remove(string key)
        {
            return DB.KeyDelete(key);
        }

        /// <summary>
        /// Clear the redis cache
        /// </summary>
        public static void Clear()
        {
            var endpoints = CacheConnection.GetEndPoints(true);

            foreach (var endpoint in endpoints)
            {
                var server = CacheConnection.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        #endregion
    }
}
