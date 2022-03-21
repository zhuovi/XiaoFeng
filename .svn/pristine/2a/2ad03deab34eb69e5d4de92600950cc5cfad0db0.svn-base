using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-08 17:38:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志数据操作类
    /// </summary>
    public class LogData : EntityBase
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public LogData()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 错误ID
        /// </summary>
        [Description("错误ID")]
        public string ErrorID { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        [Description("方法名")]
        public string FunctionName { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        [Description("类名")]
        public string ClassName { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        [Description("信息")]
        public string Message { get; set; }
        /// <summary>
        /// 错误源
        /// </summary>
        [Description("错误源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        [Description("日志类型")]
        public LogType LogType { get; set; }
        /// <summary>
        /// 是否去除前缀
        /// </summary>
        public Boolean IsReplace { get; set; }
        /// <summary>
        /// 错误堆栈
        /// </summary>
        [Description("错误堆栈")]
        public string StackTrace { get; set; }
        /// <summary>
        /// 错误堆栈
        /// </summary>
        [Description("堆栈跟踪")]
        public StackTrace Tracking { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [Description("请求地址")]
        public string RequestUrl { get; set; }
        /// <summary>
        /// 日志时间
        /// </summary>
        [Description("日志时间")]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否记录
        /// </summary>
        [Description("是否记录")] 
        public Boolean IsRecord { get; set; } = false;
        #endregion

        #region 方法

        #endregion
    }
}