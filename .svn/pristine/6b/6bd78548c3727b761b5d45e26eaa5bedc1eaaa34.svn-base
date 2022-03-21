using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
#if NETCORE
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
#endif
using XiaoFeng.IO;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-12-17 下午 10:30:53          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cache
{
    /// <summary>
    /// 内存缓存
    /// Version : 1.0.0
    /// CrateTime : 2020-12-17 下午 10:30:53
    /// Author : Jacky
    /// 更新说明
    /// </summary>
    public class MemoryCacheManage : IMemoryCacheX
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemoryCacheManage()
        {
            /*定时移除超时对象
            Task.Run(() =>
            {
                Task.Delay(1 * 60 * 1000).Wait();
                if (this.Data != null && this.Data.Count > 0)
                {
                    var keys = new List<string>();
                    var now = DateTime.Now;
                    this.Data.Each(a =>
                    {
                        if (a.Value.ExpireTime <= now) keys.Add(a.Key);
                    });
                    if (keys.Count > 0)
                    {
                        keys.Each(a => this.Data.TryRemove(a, out _));
                        keys.Clear(); keys = null;
                    }
                }
            });*/
            new XiaoFeng.Threading.Job
            {
                Name = "定时移除超时对象",
                Period = 1 * 60 * 1000,
                SuccessCallBack = j =>
                {
                    if (this.Data != null && this.Data.Count > 0)
                    {
                        var keys = new List<string>();
                        var now = DateTime.Now;
                        this.Data.Each(a =>
                        {
                            if (a.Value.ExpireTime <= now) keys.Add(a.Key);
                        });
                        if (keys.Count > 0)
                        {
                            keys.Each(a => this.Data.TryRemove(a, out _));
                            keys.Clear(); keys = null;
                        }
                    }
                },
                TimerType = Threading.TimerType.Interval
            }.Start();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 缓存数据
        /// </summary>
        private readonly ConcurrentDictionary<string, CacheValue> Data = new ConcurrentDictionary<string, CacheValue>();
        /// <summary>
        /// 监控目录
        /// </summary>
        private readonly ConcurrentDictionary<string, FileProvider> Providers = new ConcurrentDictionary<string, FileProvider>();
        #endregion

        #region 方法

        #region 验证缓存项是否存在
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            if (key == null) return false;
            return this.Data.ContainsKey(key);
        }
        #endregion

        #region 删除缓存
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            if (key == null) return;
            if (!this.Data.ContainsKey(key)) return;
            var val = this.Data[key];
            if (val.Path.IsNullOrEmpty())
            {
                this.Data.TryRemove(key, out _);
            }
            else
            {
                if (this.Providers.TryGetValue(val.Path, out var provider))
                {
                    provider.Remove(key);
                }
            }
        }
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void Clear()
        {
            var keys = this.Data.Keys;
            keys.Each(a =>
            {
                this.Remove(a);
            });
        }
        #endregion

        #region 获取缓存
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            if (!this.Data.ContainsKey(key)) return null;
            var val = this.Data[key];
            var now = DateTime.Now;
            if (val.ExpireTime <= now)
            {
                if (val.Path.IsNotNullOrEmpty())
                {
                    if (this.Providers.TryGetValue(val.Path, out var provider))
                    {
                        provider.Remove(key);
                    }
                }
                this.Data.TryRemove(key, out _);
                return null;
            }
            else
            {
                if (val.IsSliding)
                {
                    val.ExpireTime = now + val.Peroid;
                }
                return val.Value;
            }
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (key == null) return default(T);
            return (T)this.Get(key);
        }
        #endregion

        #region 获取所有缓存键
        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            return this.Data.Keys.ToList();
        }
        #endregion

        #region 设置缓存
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="path">缓存文件</param>
        /// <param name="expiresIn">缓存时长 单位为秒</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        public bool Set(string key, object value, string path, TimeSpan expiresIn, bool isSliding = true)
        {
            Boolean f = path.IsNullOrEmpty() ? false : FileHelper.Exists(path);
            if (key.IsNullOrEmpty()) return false;
            if (f && value.IsNullOrEmpty())
                value = FileHelper.OpenText(path);
            var val = new CacheValue
            {
                Value = value,
                Path = path,
                Peroid = expiresIn,
                IsSliding = isSliding,
                ExpireTime = DateTime.Now + expiresIn
            };
            this.Data.TryAdd(key, val);
            if (f)
            {
#if NETCORE
                var RootPath = Path.GetDirectoryName(path);
                var FileName = Path.GetFileName(path);
                FileProvider provider;
                if (Providers.ContainsKey(RootPath))
                {
                    if (!Providers.TryGetValue(RootPath, out provider))
                        return false;
                }
                else
                {
                    provider = new FileProvider
                    {
                        Provider = new PhysicalFileProvider(RootPath)
                    };
                    Providers.TryAdd(RootPath, provider);
                }
                provider.Add(key, ChangeToken.OnChange(() => provider.Provider.Watch(FileName), a =>
                                {
                                    this.Data.TryRemove(key, out _);
                                }, key));
#endif
            }
            return true;
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="path">缓存文件</param>
        public bool Set(string key, object value, string path)
        {
            return this.Set(key, value, path, TimeSpan.FromMinutes(10));
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
            if (key.IsNullOrEmpty() || value == null)
                return false;
            return this.Set(key, value, "", expiresIn, isSliding);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="cacheTime">缓存时长 单位为秒</param>
        public bool Set(string key, object value, long cacheTime)
        {
            return this.Set(key, value, TimeSpan.FromSeconds(cacheTime), true);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        public bool Set(string key, object value)
        {
            return this.Set(key, value, 10 * 60);
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
            if (expiresSliding.TotalSeconds > 0)
                return this.Set(key, value, "", expiresSliding, true);
            else if (expiressAbsoulte.TotalSeconds > 0)
                return this.Set(key, value, "", expiressAbsoulte);
            return this.Set(key, value);
        }
        #endregion

        #endregion
    }

    #region 文件监控
    /// <summary>
    /// 文件监控
    /// </summary>
    public class FileProvider
    {
        /// <summary>
        /// 是否为空
        /// </summary>
        public Boolean IsEmpty { get; set; } = false;

#if NETCORE
        /// <summary>
        /// 监控目录
        /// </summary>
        public PhysicalFileProvider Provider { get; set; }
#endif

        /// <summary>
        /// 文件列表
        /// </summary>
        public ConcurrentDictionary<string, IDisposable> ChangeToken { get; set; }
        /// <summary>
        /// 添加监控
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="disposable">监控文件</param>
        /// <returns></returns>
        public Boolean Add(string key, IDisposable disposable)
        {
            if (this.ChangeToken == null) this.ChangeToken = new ConcurrentDictionary<string, IDisposable>();
            this.IsEmpty = false;
            return this.ChangeToken.TryAdd(key, disposable);
        }
        /// <summary>
        /// 移除监控文件
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Boolean Remove(string key)
        {
            if (this.ChangeToken.ContainsKey(key))
            {
                var dis = this.ChangeToken[key];
                dis.Dispose();
                this.ChangeToken.TryRemove(key, out _);
                if (this.ChangeToken.Count == 0)
                {
                    this.IsEmpty = true;
                    //this.Provider.TryDispose();
                }
            }
            return true;
        }
    }
    #endregion

    #region 缓存值
    /// <summary>
    /// 缓存值
    /// </summary>
    [Serializable]
    public class CacheValue
    {
        /// <summary>
        /// 值
        /// </summary>
        public object Value;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 缓存间隔
        /// </summary>
        public TimeSpan Peroid { get; set; }
        /// <summary>
        /// 是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）
        /// </summary>
        public Boolean IsSliding { get; set; } = true;
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
    }
    #endregion
}