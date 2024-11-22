using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-11-20 16:41:32                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 时间段接口
    /// </summary>
    public interface ITimePeriod
    {
        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 时间是否在当前时间段
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="deviation">时间偏差 单位毫秒 默认为1秒</param>
        /// <returns></returns>
        Boolean IsBetween(DateTime time, long deviation = 1000);
        #endregion
    }
}