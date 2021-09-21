using System.ComponentModel;

namespace Framework.Cache
{
    /// <summary>
    /// Cache type enumerator
    /// </summary>
    public enum CacheType
    {
        [Description("Runtime Cache")]
        Runtime = 1,

        [Description("Redis Cache")]
        Redis = 2
    }
}
