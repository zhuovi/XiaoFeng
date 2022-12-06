using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2015) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 15:04:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库操作类异常
    /// </summary>
    public class DataHelperException : DbException
    {
        /// <summary>
        /// 标识名
        /// </summary>
        const string FlagName = "DataHelper:";
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperException() : base() { }
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public DataHelperException(string message) : base(FlagName + message) { }
        /// <summary>
        /// 设置错误信息及异常
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="exception">异常</param>
        public DataHelperException(string message, Exception exception) : base(FlagName + message, exception) { }
    }
}