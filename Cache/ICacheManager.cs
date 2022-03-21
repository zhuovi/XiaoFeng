using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="cacheTime">过期时间 单位为毫秒</param>
        bool Set(string key, object value, long cacheTime);
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        bool Set(string key, object value);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Contains(string key);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键</param>
        void Remove(string key);
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
    }
}