using System.Runtime.Caching;

namespace Framework.Cache
{
    //TODO: Add comment here
    public interface ICache
    {
        void Add(string key, object value, double minutes = 1);
        void Add(string key, object value, CacheItemPolicy policy);
        void Remove(string key);
        bool Exists(string key);
        T Get<T>(string key);
        void Clear();
    }
}
