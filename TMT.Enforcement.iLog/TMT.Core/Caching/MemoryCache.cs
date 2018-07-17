﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Core.Caching
{
    public class MemoryCache : ICache
    {
        private string _name;

        public MemoryCache(string name)
        {
            this._name = name;
        }

        public object Get(string key, bool remove = false)
        {
            object val = Cache[GetCompositeKey(key)];
            return val;
        }

        public void Set(string key, object data, long cacheTimeMinutes = 1440)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(cacheTimeMinutes)
            };

            Cache.Add(new CacheItem(GetCompositeKey(key), data), policy);
        }

        public bool IsSet(string key)
        {
            return (Cache[GetCompositeKey(key)] != null);
        }

        public void Remove(string key)
        {
            Cache.Remove(GetCompositeKey(key));
        }

        public void Clear()
        {
            var allKeys = Cache.Select(f => f.Key).Where(f => f.StartsWith(_name)).ToList();
            Parallel.ForEach(allKeys, key => Cache.Remove(key));
        }

        private string GetCompositeKey(string key)
        {
            return string.Format("{0}_{1}", _name, key);
        }

        private static ObjectCache Cache
        {
            get { return System.Runtime.Caching.MemoryCache.Default; }
        }  
    }
}