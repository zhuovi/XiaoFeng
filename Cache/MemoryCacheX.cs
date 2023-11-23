using System;
using System.Collections.Generic;
using System.Linq;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-15 23:40:50                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class MemoryCacheX : IMemoryCacheX
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemoryCacheX()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 缓存操作
        /// </summary>
        public static readonly IMemoryCache Cache = new MemoryCache();
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Contains(string key) => Cache.ContainsKey(key);
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
            if (key == null || value == null) return false;
            return Cache.TryAdd(key, value, expiressAbsoulte, expiresSliding);
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
            if (!FileHelper.Exists(path, FileAttribute.File)) return false;
            if (value.IsNullOrEmpty())
            {
                value = FileHelper.OpenText(path);
            }
            return Cache.TryAdd(key, value, expiresIn, isSliding ? expiresIn : TimeSpan.Zero, path);
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
            Cache.Remove(key);
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any()) return;
            keys.Each(key => this.Remove(key));
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
                return default;
            return Cache.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
                return null;
            return Cache.Get(key)?.Value;
        }

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
                return null;

            var dict = new Dictionary<string, object>();
            keys.Each(key => dict.Add(key, this.Get(key)));
            return dict;
        }
        #endregion

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void Clear()
        {
            Cache.Clear();
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
            return Cache.Keys.Where(k => k.IsMatch(pattern)).ToList();
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            return Cache.Keys.ToList();
        }
        #endregion
    }
}