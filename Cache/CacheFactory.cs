using System;
using XiaoFeng.Config;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-08 11:54:35                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public static class CacheFactory
    {
        #region 构造器

        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        /// <returns>缓存实例</returns>
        public static IMemoryCacheX Create(CacheType cacheType = CacheType.Default)
        {
            switch (cacheType)
            {
                case CacheType.Disk:
                    return Activator.CreateInstance(typeof(FileCache)) as IMemoryCacheX;
                case CacheType.Redis:
                    return Activator.CreateInstance(typeof(RedisCache)) as IMemoryCacheX;
                case CacheType.Memory:
                    return Activator.CreateInstance(typeof(MemoryCacheX)) as IMemoryCacheX;
                case CacheType.Default:
                default:
                    var cType = CacheConfig.Current.CacheType;
                    if (cType == CacheType.Default) cType = CacheType.Memory;
                    return Create(cType);
            }
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static IMemoryCacheX Create<T>() where T : IMemoryCacheX
        {
            return Activator.CreateInstance<T>();
        }
        #endregion
    }
}