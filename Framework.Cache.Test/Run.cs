using System;
using System.Linq;

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

        #region| Methods |

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
        public void TestResetRedis()
        {
            output.WriteLine("The redis cache reset test will start");

            SetConnection();

            if (Cache.Exists("item") == false)
            {
                Cache.Add("item", "cachedItem");
            }

            Cache.Clear();

            output.WriteLine("The method finished");
        }

        private void SetConnection()
        {
            // The connection string below is from a redis emulator running on my own machine
            Cache.SetConnection("localhost:6379,abortConnect=False,keepAlive=180,connectTimeout=1000000,allowAdmin=true");
        } 

        #endregion

    }
}
