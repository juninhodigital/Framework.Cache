using Xunit;
using Xunit.Abstractions;

using Framework.Core;

namespace Framework.Cache.Test
{
    public class Run
    {
        #region| Fields |

        protected ITestOutputHelper output;

        #endregion

        #region| Constructor |

        public Run(ITestOutputHelper testOutputHelper)
        {
            this.output = testOutputHelper;
        }

        #endregion

        private void SetConnection()
        {
            Cache.SetConnection("YOUR_REDIS_URL.redis.cache.windows.net:6380,password=ABCDEFG,ssl=True,abortConnect=False");
        }

        [Fact]
        public void TestRuntimeCache()
        {
            output.WriteLine("The runtime cache test will start");

            if (Cache.Exists("item") == false)
            {
                Cache.Add("item", "cachedItem");
            }

            if (Cache.Exists("item"))
            {
                var cached = Cache.Get("item");

                if (cached.IsNotNull())
                {
                    Assert.Equal("cachedItem", cached);

                    if (Cache.Remove("item"))
                    {
                        if (Cache.Exists("item") == false)
                        {
                            output.WriteLine("The entry was removed from the runtime cache");
                        }
                    }
                }
            }

            output.WriteLine("The method finished");
        }

        [Fact]
        public void TestRedisCache()
        {
            output.WriteLine("The redis cache test will start");

            SetConnection();

            if (Cache.Exists("item", CacheType.Redis) == false)
            {
                Cache.Add("item", "cachedItem", CacheType.Redis);
            }

            if (Cache.Exists("item", CacheType.Redis))
            {
                var cached = Cache.Get("item", CacheType.Redis);

                if (cached.IsNotNull())
                {
                    Assert.Equal("cachedItem", cached);

                    if (Cache.Remove("item", CacheType.Redis))
                    {
                        if (Cache.Exists("item", CacheType.Redis) == false)
                        {
                            output.WriteLine("The entry was removed from the runtime cache");
                        }
                    }
                }
            }

            output.WriteLine("The method finished");
        }

        [Fact]
        public void TestResetRedis()
        {
            output.WriteLine("The redis cache reset test will start");

            SetConnection();

            if (Cache.Exists("item", CacheType.Redis) == false)
            {
                Cache.Add("item", "cachedItem", CacheType.Redis);
            }

            Cache.Clear(CacheType.Redis);            

            output.WriteLine("The method finished");
        }

    }
}
