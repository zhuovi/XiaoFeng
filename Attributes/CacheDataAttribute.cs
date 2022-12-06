using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 11:05:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 缓存数据属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CacheDataAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CacheDataAttribute()
        {
            this.CacheType = this.CacheType= CacheType.Default;
            this.TimeOut = -1;
        }
        /// <summary>
        /// 设置过期时间 单位为秒  0为永久缓存
        /// </summary>
        /// <param name="TimeOut">过期时间</param>
        public CacheDataAttribute(int TimeOut)
        {
            this.CacheType = CacheType.Memory;
            this.TimeOut = TimeOut;
        }
        /// <summary>
        /// 设置过期时间 单位为秒  0为永久缓存
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        /// <param name="TimeOut">过期时间</param>
        public CacheDataAttribute(CacheType cacheType, int TimeOut)
        {
            this.CacheType = cacheType;
            this.TimeOut = TimeOut;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否缓存数据
        /// </summary>
        public CacheType CacheType { get; set; }
        /// <summary>
        /// 过期时间 单位为秒 0为永久缓存
        /// </summary>
        public int TimeOut { get; set; } = 300;
        #endregion
    }
}