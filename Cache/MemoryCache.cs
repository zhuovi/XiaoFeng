using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using XiaoFeng.Threading;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-14 16:24:54                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 内存缓存 
    /// </summary>
    public class MemoryCache : IMemoryCache
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemoryCache()
        {
            this.Entitys = new ConcurrentDictionary<string, CacheEntity>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否启动
        /// </summary>
        private Boolean IsRunTime { get; set; } = false;
        /// <summary>
        /// 定时器
        /// </summary>
        private IJob Timer { get; set; }
        /// <summary>
        /// 缓存对象
        /// </summary>
        protected ConcurrentDictionary<string, CacheEntity> Entitys { get; set; }
        /// <summary>
        /// 键的集合
        /// </summary>
        public ICollection<string> Keys => (from e in this.Entitys where (e.Value.ExpiresTime.HasValue && e.Value.ExpiresTime > DateTime.Now) || !e.Value.ExpiresTime.HasValue select e.Key).ToList();
        /// <summary>
        /// 缓存值的集合
        /// </summary>
        public ICollection<CacheEntity> EntityValues => (from e in this.Entitys where (e.Value.ExpiresTime.HasValue && e.Value.ExpiresTime > DateTime.Now) || !e.Value.ExpiresTime.HasValue select e.Value).ToList();
        /// <summary>
        /// 值的集合
        /// </summary>
        public ICollection<object> Values => (from e in this.Entitys where (e.Value.ExpiresTime.HasValue && e.Value.ExpiresTime > DateTime.Now) || !e.Value.ExpiresTime.HasValue select e.Value).Select(x => x.Value).ToList();
        #endregion

        #region 方法

        #region 是否包含key
        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        public Boolean ContainsKey(string key) => !key.IsNullOrEmpty() && this.Entitys.ContainsKey(key);
        #endregion

        #region 获取缓存
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        public CacheEntity Get(string key)
        {
            if (this.TryGetValue(key, out CacheEntity entity))
                return entity;
            return null;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        public Boolean TryGetValue(string key, out CacheEntity entity)
        {
            this.ExpireCache();
            if (this.Entitys.TryGetValue(key, out entity))
            {
                if (entity.ExpiresIn.HasValue)
                    entity.ExpiresTime = DateTime.Now.AddMilliseconds(entity.ExpiresIn.Value.TotalMilliseconds);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (this.TryGetValue(key, out object entity))
                return entity.ToCast<T>();
            return default(T);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        public Boolean TryGetValue(string key, out object value)
        {
            if (this.TryGetValue(key, out CacheEntity entity))
            {
                value = entity.Value;
                return true;
            }
            value = null;
            return false;
        }
        #endregion

        #region 设置缓存
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="expiresTime">过期时长</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="path">文件路径或目录</param>
        /// <returns></returns>
        public Boolean TryAdd(string key, object value, TimeSpan? expiresTime = null, TimeSpan? expiresSliding = null, string path = "")
        {
            this.ExpireCache();
            if(this.ContainsKey(key))
                this.Remove(key);
            var val = new CacheEntity
            {
                Name = key,
                Value = value,
                CallBack = this.CallBack,
                Path = path,
            };
            if (expiresTime.HasValue && expiresTime != TimeSpan.Zero)
            {
                val.ExpiresTime = DateTime.Now.AddMilliseconds(expiresTime.GetValueOrDefault().TotalMilliseconds);
                if (expiresSliding.HasValue && expiresSliding.Value.TotalMilliseconds > 0)
                {
                    val.IsSliding = true;
                    val.ExpiresIn = expiresSliding.GetValueOrDefault();
                }
            }
            if (expiresTime.HasValue && expiresTime.Value.TotalMilliseconds > 0)
                this.Init().ConfigureAwait(false);
            return this.Entitys.TryAdd(key, val);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="expiresIn">超时时长 单位为毫秒</param>
        /// <param name="IsSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public Boolean TryAdd(string key, object value, double expiresIn, Boolean IsSliding = false)
        {
            return this.TryAdd(key, value, TimeSpan.FromMilliseconds(expiresIn));
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="path">文件路径或目录</param>
        /// <returns></returns>
        public Boolean TryAdd(string key, object value, string path)
        {
            return this.TryAdd(key, value, null, null, path);
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        public Boolean TryUpdate(string key, object value)
        {
            this.ExpireCache();
            if (this.ContainsKey(key))
            {
                this.Entitys[key].Value = value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加或更新缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        public Boolean TryAddOrUpdate(string key, object value)
        {
            if (this.ContainsKey(key))
                return this.TryUpdate(key, value);
            else
                return this.TryAdd(key, value);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        public Boolean Set(string key, object value)
        {
            return this.TryAddOrUpdate(key, value);
        }
        #endregion

        #region 移除缓存
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        public Boolean Remove(string key)
        {
            return this.TryRemove(key, out var _);
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        public Boolean TryRemove(string key, out CacheEntity value)
        {
            this.ExpireCache();
            if (this.Entitys.TryRemove(key, out value))
            {
                value.Dispose();
                value = null;
                if (this.Keys.Count == 0) this.StopAsync().ConfigureAwait(false);
                return true;
            }
            return false;
        }
        #endregion

        #region 清空缓存
        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            this.Entitys.Clear();
            this.StopAsync().ConfigureAwait(false);
        }
        #endregion

        #region 遍历过期
        /// <summary>
        /// 遍历过期
        /// </summary>
        private void ExpireCache()
        {
            Task.Factory.StartNew(() =>
            {
                (from e in this.Entitys where e.Value.ExpiresTime.HasValue && e.Value.ExpiresTime <= DateTime.Now select e.Value).Each(a =>
                {
                    if (this.Entitys.TryRemove(a.Name, out var value))
                    {
                        value.Dispose();
                    }
                });
                if (this.Entitys.Values.Where(a => a.ExpiresTime.HasValue).Count() == 0)
                    this.StopAsync().ConfigureAwait(false);
            });
        }
        #endregion

        #region 定时清理过期缓存
        /// <summary>
        /// 定时清理过期缓存
        /// </summary>
        private async Task Init()
        {
            
            Synchronized.Run(() =>
            {
                if (this.IsRunTime) return;
                this.IsRunTime = true;
                this.Timer = new Job
                {
                    Name = "清理缓存",
                    Period = 60 * 1000,
                    TimerType = TimerType.Interval,
                    SuccessCallBack = job =>
                    {
                        this.ExpireCache();
                    }
                };
                this.Timer.Start();
            });
            await Task.CompletedTask;
        }
        #endregion

        #region 停止定时器
        /// <summary>
        /// 停止定时器
        /// </summary>
        /// <returns></returns>
        private async Task StopAsync()
        {
            this.IsRunTime = false;
            if (this.Timer != null)
                this.Timer.Stop();
            await Task.CompletedTask;
        }
        #endregion

        #region 回调事件
        /// <summary>
        /// 回调事件
        /// </summary>
        private Action<WatcherChangeType, string, string> CallBack => (t, p, k) =>
          {
              this.Remove(k);
          };
        #endregion
        #endregion
    }
}