using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
#if NETFRAMEWORK
using System.Runtime.Caching;
#else
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Collections;
#endif
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    public class MemoryCacheHelper : IMemoryCacheX
    {
#if NETCORE
        /// <summary>
        /// 缓存操作
        /// </summary>
        public static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
#endif
        
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            if (key == null)return false;
#if NETFRAMEWORK
            return System.Runtime.Caching.MemoryCache.Default.Contains(key);
#else
            return Cache.TryGetValue(key, out _);
#endif
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null || value == null)return false;
#if NETFRAMEWORK
            var Policy = new CacheItemPolicy();
            if (expiresSliding.TotalMilliseconds > 0) Policy.SlidingExpiration = expiresSliding;
            if(expiressAbsoulte.TotalMilliseconds>0)Policy.AbsoluteExpiration= new DateTimeOffset(DateTime.Now.AddMilliseconds(expiressAbsoulte.TotalMilliseconds).ToUniversalTime());
            System.Runtime.Caching.MemoryCache.Default.Set(key, value, Policy); 
#else
            var options = new MemoryCacheEntryOptions();
            if (expiresSliding.TotalMilliseconds > 0) options.SlidingExpiration = expiresSliding;
            if (expiressAbsoulte.TotalMilliseconds > 0) options.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMilliseconds(expiressAbsoulte.TotalMilliseconds).ToUniversalTime());
            Cache.Set(key, value, options);
#endif
            return Contains(key);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null || value == null)
                return false;
            return isSliding ? this.Set(key, value, expiresIn, TimeSpan.Zero) : this.Set(key, value, TimeSpan.Zero, expiresIn);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="cacheTime">缓存时长 单位为秒</param>
        public bool Set(string key, object value, long cacheTime)
        {
            return this.Set(key, value, new TimeSpan(cacheTime * 1000), true);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        public bool Set(string key, object value)
        {
            return this.Set(key, value, TimeSpan.Zero, TimeSpan.Zero);
            //Cache.Set(key, value);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="path">缓存文件</param>
        /// <param name="expiresIn">缓存时长 单位为秒</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        public bool Set(string key, object value, string path, TimeSpan expiresIn, bool isSliding = false)
        {
            if (!File.Exists(path)) return false;
            if (value.IsNullOrEmpty())
            {
                using (var sr = File.OpenText(path))
                {
                    value = sr.ReadToEnd();
                }
            }
#if NETFRAMEWORK
            this.Set(key, value, expiresIn, isSliding);
#else
            Cache.GetOrCreate(key, entry =>
            {
                entry.AddExpirationToken(
                    new Microsoft.Extensions.FileProviders.Physical.PollingFileChangeToken(new FileInfo(path)));
                if (expiresIn.TotalMilliseconds > 0)
                    if (isSliding)
                        entry.SlidingExpiration = expiresIn;
                    else
                        entry.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMilliseconds(expiresIn.TotalMilliseconds).ToUniversalTime());
                return value;
            });
#endif
            return true;
            /*var cache = Cache.CreateEntry(key);
            var FileProvider = new PhysicalFileProvider(path.Substring(0, path.LastIndexOf('\\')));
            cache.Value = value;
            if (expiresIn.TotalMilliseconds > 0)
                if (isSliding)
                    cache.SlidingExpiration = expiresIn;
                else
                    cache.AbsoluteExpiration = new DateTimeOffset(DateTime.Now, expiresIn);*/
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="path">缓存文件</param>
        public bool Set(string key, object value, string path)
        {
            return this.Set(key, value, path, new TimeSpan(0));
        }

#region 删除缓存
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
#if NETFRAMEWORK
            System.Runtime.Caching.MemoryCache.Default.Remove(key);
#else
            Cache.Remove(key);
#endif
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any()) return;
            keys.ToList().ForEach(key => this.Remove(key));
        }
#endregion

#region 获取缓存
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return (T)this.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
#if NETFRAMEWORK
            return System.Runtime.Caching.MemoryCache.Default.Get(key);
#else
            return Cache.Get(key);
#endif
        }

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var dict = new Dictionary<string, object>();
            keys.ToList().ForEach(key => dict.Add(key, this.Get(key)));
            return dict;
        }
#endregion

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void Clear()
        {
            var l = GetCacheKeys();
            foreach (var s in l)
            {
                this.Remove(s);
            }
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern">正则匹配</param>
        /// <returns></returns>
        public void RemoveCacheRegex(string pattern)
        {
            IList<string> l = SearchCacheRegex(pattern);
            foreach (var s in l)
            {
                Remove(s);
            }
        }

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> SearchCacheRegex(string pattern)
        {
            var cacheKeys = GetCacheKeys();
            var l = cacheKeys.Where(k => k.IsMatch(pattern)).ToList();
            return l.AsReadOnly();
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            var keys = new List<string>();
#if NETFRAMEWORK
            var list = System.Runtime.Caching.MemoryCache.Default.ToArray();
            if (list.Length == 0) return keys;
            list.Each(kv =>
            {
                keys.Add(kv.Key);
            });
#else
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
            if (!(entries is IDictionary cacheItems)) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
#endif
            return keys;
        }
    }
}