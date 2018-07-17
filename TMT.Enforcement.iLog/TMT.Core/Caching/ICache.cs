
namespace TMT.Core.Caching
{
    public interface ICache
    {
        object Get(string key, bool remove = false);
        void Set(string key, object data, long cacheTimeMinutes = 1440);
        bool IsSet(string key);
        void Remove(string key);
        void Clear();
    }
}
