using System;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-12-18 上午 12:57:54          *
 *  Version : v 1.0.0                               *
 ***************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 接口说明
    /// Version : 1.0.0
    /// CrateTime : 2020-12-18 上午 12:57:54
    /// Author : Jacky
    /// 更新说明
    /// </summary>
    public interface IMemoryCacheX : ICacheManager
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="path">缓存文件</param>
        bool Set(string key, object value, string path);
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte);
    }
}