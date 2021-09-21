using System.Runtime.Caching;

namespace Framework.Cache
{
    public interface ICache
    {
        void Add(string key, object value, double minutes = 1);
        void Add(string key, object value, CacheItemPolicy policy);
        bool Remove(string key);
        bool Exists(string key);
        T Get<T>(string key);
        void Clear();
    }
}
