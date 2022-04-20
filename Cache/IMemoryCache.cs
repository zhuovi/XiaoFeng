using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-14 17:15:22                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface IMemoryCache
    {
        #region 属性
        /// <summary>
        /// 键的集合
        /// </summary>
        ICollection<string> Keys { get; }
        /// <summary>
        /// 缓存值的集合
        /// </summary>
        ICollection<CacheEntity> EntityValues { get; }
        /// <summary>
        /// 值的集合
        /// </summary>
        ICollection<object> Values { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        Boolean ContainsKey(string key);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        CacheEntity Get(string key);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        Boolean TryGetValue(string key, out CacheEntity entity);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        Boolean TryGetValue(string key, out object value);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="expiresTime">过期时长</param>
        /// <param name="expiresSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="path">文件路径或目录</param>
        /// <returns></returns>
        Boolean TryAdd(string key, object value, TimeSpan? expiresTime = null, TimeSpan? expiresSliding = null, string path = "");
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="expiresIn">超时时长 单位为毫秒</param>
        /// <param name="IsSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        Boolean TryAdd(string key, object value, double expiresIn, Boolean IsSliding = false);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <param name="path">文件路径或目录</param>
        /// <returns></returns>
        Boolean TryAdd(string key, object value, string path);
        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        Boolean TryUpdate(string key, object value);
        /// <summary>
        /// 添加或更新缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        Boolean TryAddOrUpdate(string key, object value);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        Boolean Set(string key, object value);
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <returns></returns>
        Boolean Remove(string key);
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键</param>
        /// <param name="value">要插入缓存中的对象</param>
        /// <returns></returns>
        Boolean TryRemove(string key, out CacheEntity value);
        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        void Clear();
        #endregion
    }
}