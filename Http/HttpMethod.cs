using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Http
{
    /// <summary>
    /// 请求类型
    /// </summary>
    public class HttpMethod : IEquatable<HttpMethod>
    {
        #region 构造器
        /// <summary>
        /// 设置请求类型
        /// </summary>
        /// <param name="method"></param>
        public HttpMethod(string method)
        {
            this.Method = method;
        }
        #endregion

        #region 属性
        /// <summary>
        /// Get
        /// </summary>
        public static HttpMethod Get { get { return new HttpMethod("Get"); } }
        /// <summary>
        /// Post
        /// </summary>
        public static HttpMethod Post { get { return new HttpMethod("Post"); } }
        /// <summary>
        /// Head
        /// </summary>
        public static HttpMethod Head { get { return new HttpMethod("Head"); } }
        /// <summary>
        /// Options
        /// </summary>
        public static HttpMethod Options { get { return new HttpMethod("Options"); } }
        /// <summary>
        /// Delete
        /// </summary>
        public static HttpMethod Delete { get { return new HttpMethod("Delete"); } }
        /// <summary>
        /// Patch
        /// </summary>
        public static HttpMethod Patch { get { return new HttpMethod("Patch"); } }
        /// <summary>
        /// Put
        /// </summary>
        public static HttpMethod Put { get { return new HttpMethod("Put"); } }
        /// <summary>
        /// Trace
        /// </summary>
        public static HttpMethod Trace { get { return new HttpMethod("Trace"); } }
        /// <summary>
        /// 名称
        /// </summary>
        public string Method { get; private set; } = "Get";

        #endregion

        #region 方法
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other">请求类型</param>
        /// <returns>是否相等</returns>
        public bool Equals(HttpMethod other) => this.Method.EqualsIgnoreCase(other.Method);
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="left">请求类型</param>
        /// <param name="right">请求类型</param>
        /// <returns>summary</returns>
        public static bool operator ==(HttpMethod left, HttpMethod right) => left.Equals(right);
        /// <summary>
        /// 是否不相等
        /// </summary>
        /// <param name="left">请求类型</param>
        /// <param name="right">请求类型</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(HttpMethod left, HttpMethod right) => !left.Equals(right);
        /// <summary>
        /// 显示转换
        /// </summary>
        /// <param name="method">请求类型</param>
        public static explicit operator string(HttpMethod method) => method.ToString();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="method">请求类型</param>
        public static implicit operator HttpMethod(string method) => new HttpMethod(method);
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">请求类型</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is HttpMethod method)
                return this.Equals(method);
            return false;
        }
        /// <summary>
        /// HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => this.Method.GetHashCode();
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns>返回请求类型</returns>
        public override string ToString() => this.Method;
        #endregion
    }
}
