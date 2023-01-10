using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-08 17:55:12                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcached 错误类
    /// </summary>
    public class MemcachedException : Exception
    {
        #region 构造器
        /// <summary>
        /// 标识名
        /// </summary>
        const string FlagName = "XiaoFeng.Memcached:";
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedException() : base() { }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">信息</param>
        public MemcachedException(string message) : base(FlagName + message) { }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="exception">错误</param>
        public MemcachedException(string message, Exception exception) : base(FlagName + message, exception) { }
        #endregion

        #region 属性

        #endregion

        #region 方法
        #endregion
    }
}