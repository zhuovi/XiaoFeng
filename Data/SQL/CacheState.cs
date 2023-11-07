using System.ComponentModel;
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
namespace XiaoFeng.Data.SQL
{
    #region 是否缓存状态类型
    /// <summary>
    /// 是否缓存状态类型
    /// </summary>
    public enum CacheState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        [Description("无状态")]
        Null = 0,
        /// <summary>
        /// 缓存
        /// </summary>
        [Description("缓存")]
        Yes = 1,
        /// <summary>
        /// 不缓存
        /// </summary>
        [Description("不缓存")]
        No = 2,
        /// <summary>
        /// 清除缓存
        /// </summary>
        [Description("清除缓存")]
        Clear = 3
    }
    #endregion
}
