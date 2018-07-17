
namespace Kapsch.Core.Caching
{
    public interface ICache<T> where T : class
    {
        T Get(string key, bool remove = false);
        void Set(string key, T data, long cacheTimeMinutes = 1440);
        bool IsSet(string key);
        void Remove(string key);
        void Clear();
    }
}
