using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XiaoFeng;
using XiaoFeng.Config;
using XiaoFeng.OS;
namespace XiaoFeng.Cache
{
    #region 缓存操作类
    /// <summary>
    /// 缓存操作类
    /// Version : V 1.1
    /// Create Date : 2016-12-24
    /// Author : Jacky
    /// QQ : 7092734
    /// Email : jacky@zhuovi.com
    /// </summary>
    public static class CacheHelper
    {
        #region 无参构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        static CacheHelper()
        {
            //Cache = Platform.IsWebForm ? Cache = new MemoryCacheHelper() : new MemoryCacheManage();
            
        }
        #endregion

        #region 属性
        /// <summary>
        /// 缓存配置
        /// </summary>
        private static CacheConfig Config { get; set; } = CacheConfig.Get();
        /// <summary>
        /// 缓存对象
        /// </summary>
        private static IMemoryCacheX _Cache;
        /// <summary>
        /// 缓存对象
        /// </summary>
        public static IMemoryCacheX Cache
        {
            get
            {
                if (_Cache == null)
                    return _Cache = CacheFactory.Create(Config.CacheType);
                return _Cache;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 缓存列表
        /// </summary>
        public static CacheData DataList
        {
            get { return new CacheData(); }
        }
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns>指定的缓存项</returns>
        public static object Get(string key)
        {
            return Cache.Get(key);
        }
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns>指定的缓存项</returns>
        public static T Get<T>(string key) where T : class
        {
            return Cache.Get<T>(key);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        public static void Set(string key, object value)
        {
            if (value == null) return;
            Cache.Set(key, value);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="timeout">超时时长 单位毫秒</param>
        public static void Set(string key, object value, long timeout)
        {
            if (value == null) return;
            Cache.Set(key, value, TimeSpan.FromMilliseconds(timeout));
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="timeout">最后一次访问所插入对象时与该对象到期时之间的时间间隔。 如果该值等效于 20 分钟，则对象在最后一次被访问 20 分钟之后将到期并被从缓存中移除。如果使用可调到期，则 absoluteExpiration 参数必须为 System.Web.Caching.Cache.NoAbsoluteExpiration。</param>
        public static void Set(string key, object value, TimeSpan timeout)
        {
            if (value == null) return;
            Cache.Set(key, value, timeout, true);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="path">文件路径或目录</param>
        public static void Set(string key, object value, string path)
        {
            if (value == null) return;
            Cache.Set(key, value, path);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="absoluteExpiration">所插入对象将到期并被从缓存中移除的时间。 要避免可能的本地时间问题（例如从标准时间改为夏时制），请使用 System.DateTime.UtcNow，而不是 System.DateTime.Now 作为此参数值。 如果使用绝对到期，则 slidingExpiration 参数必须为 System.Web.Caching.Cache.NoSlidingExpiration。</param>
        /// <param name="slidingExpiration">最后一次访问所插入对象时与该对象到期时之间的时间间隔。 如果该值等效于 20 分钟，则对象在最后一次被访问 20 分钟之后将到期并被从缓存中移除。如果使用可调到期，则 absoluteExpiration 参数必须为 System.Web.Caching.Cache.NoAbsoluteExpiration。</param>
        public static bool Set(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (value == null) return false;
            return Cache.Set(key, value, slidingExpiration, absoluteExpiration - DateTime.Now);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="absoluteExpiration">所插入对象将到期并被从缓存中移除的时间。 要避免可能的本地时间问题（例如从标准时间改为夏时制），请使用 System.DateTime.UtcNow，而不是 System.DateTime.Now 作为此参数值。 如果使用绝对到期，则 slidingExpiration 参数必须为 System.Web.Caching.Cache.NoSlidingExpiration。</param>
        public static void Set(string key, object value, DateTime absoluteExpiration)
        {
            if (value == null) return;
            Cache.Set(key, value, absoluteExpiration - DateTime.Now, false);
        }
        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="key">要移除的缓存项的 System.String 标识符。</param>
        public static void Remove(string key)
        {
            Cache.Remove(key);
        }
        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void Clear()
        {
            Cache.Clear();
        }
        #endregion
    }
    #endregion

    #region 缓存数据操作类
    /// <summary>
    /// 缓存数据操作类
    /// </summary>
    public class CacheData : ICacheManager
    {
        #region 构造器
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public CacheData(string key, object value)
        {
            this.Set(key, value);
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="cacheTime">超时时间 单位为毫秒</param>
        public CacheData(string key, object value, long cacheTime)
        {
            this.Set(key, value, cacheTime);
        }
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CacheData() { }
        #endregion

        #region 属性
        /// <summary>
        /// 缓存Key值
        /// </summary>
        private static string Key => "ZW-PublicCacheKey";
        /// <summary>
        /// 缓存数据
        /// </summary>
        public Dictionary<string, object> Data
        {
            get
            {
                var _ = CacheHelper.Get(Key);
                if (_ == null) CacheHelper.Set(Key, new Dictionary<string, object>()); return _ == null ? new Dictionary<string, object>() : (Dictionary<string, object>)_;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public virtual void Add(string key, object value)
        {
            this.Set(key, value);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="cacheTime">过期时间 单位为毫秒</param>
        public virtual bool Set(string key, object value, long cacheTime)
        {
            CacheHelper.Set(Key, this.Data, DateTime.Now.AddMilliseconds(cacheTime));
            return this.Set(key, value);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public virtual bool Set(string key, object value)
        {
            if (this.Data.ContainsKey(key))
            {
                if (!this.Data[key].Equals(value))
                {
                    this.Data[key] = value;
                }
            }
            else { this.Data.Add(key, value); }
            return true;
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public virtual bool Contains(string key)
        {
            return this.Data.ContainsKey(key);
        }
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public virtual object Get(string key)
        {
            return this.Data.ContainsKey(key) ? this.Data[key] : null;
        }
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public virtual T Get<T>(string key) where T:class
        {
            return (T)this.Get(key);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键</param>
        public virtual void Remove(string key)
        {
            if (this.Data.ContainsKey(key))
                this.Data.Remove(key);
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public virtual void Clear()
        {
            this.Data.Clear();
        }
        #endregion
    }
    #endregion
}