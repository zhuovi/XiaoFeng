using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Json
{
    /// <summary>
    /// Json异常操作类
    /// </summary>
    public class JsonException : Exception
    {
        /// <summary>
        /// 标识名
        /// </summary>
        const string FlagName = "DataHelper:";
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonException() : base() { }
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public JsonException(string message) : base(FlagName + message) { }
        /// <summary>
        /// 设置错误信息及异常
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="exception">异常</param>
        public JsonException(string message, Exception exception) : base(FlagName + message, exception) { }
    }
}