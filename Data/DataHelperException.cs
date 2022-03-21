using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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