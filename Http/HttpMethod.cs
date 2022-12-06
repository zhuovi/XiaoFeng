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
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
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
        /// GET 从指定的资源请求数据。获取数据时使用
        /// </summary>
        public static HttpMethod Get { get { return new HttpMethod("Get"); } }
        /// <summary>
        /// POST 向指定的资源提交要被处理的数据。提交数据时使用
        /// </summary>
        public static HttpMethod Post { get { return new HttpMethod("Post"); } }
        /// <summary>
        /// HEAD 向服务器索要与GET请求相一致的响应，只不过响应体将不会被返回。获取内容头时使用
        /// </summary>
        public static HttpMethod Head { get { return new HttpMethod("Head"); } }
        /// <summary>
        /// OPTIONS  返回服务器针对特定资源所支持的HTTP请求方法。获取服务器所支持的请求类型时使用
        /// </summary>
        public static HttpMethod Options { get { return new HttpMethod("Options"); } }
        /// <summary>
        /// DELETE   请求服务器删除Request-URI所标识的资源。删除数据时使用
        /// </summary>
        public static HttpMethod Delete { get { return new HttpMethod("Delete"); } }
        /// <summary>
        /// PATCH    实体中包含一个表，表中说明与该URI所表示的原内容的区别。验证数据差异时使用
        /// </summary>
        public static HttpMethod Patch { get { return new HttpMethod("Patch"); } }
        /// <summary>
        /// PUT  向指定资源位置上传其最新内容。更新数据时使用
        /// </summary>
        public static HttpMethod Put { get { return new HttpMethod("Put"); } }
        /// <summary>
        /// TRACE    回显服务器收到的请求，主要用于测试或诊断。测试服务器性能时使用
        /// </summary>
        public static HttpMethod Trace { get { return new HttpMethod("Trace"); } }
        /// <summary>
        /// CONNECT     方法用来建立到给定URI标识的服务器的隧道；它通过简单的TCP / IP隧道更改请求连接，通常实使用解码的HTTP代理来进行SSL编码的通信（HTTPS）。
        /// </summary>
        public static HttpMethod Connect { get { return new HttpMethod("Connect"); } }
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
        /// 显示转换
        /// </summary>
        /// <param name="method">请求类型</param>
        public static explicit operator System.Net.Http.HttpMethod(HttpMethod method) =>new System.Net.Http.HttpMethod(method.Method);
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="method">请求类型</param>
        public static implicit operator HttpMethod(System.Net.Http.HttpMethod method) => new HttpMethod(method.Method);
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
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Method);
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns>返回请求类型</returns>
        public override string ToString() => this.Method;
        #endregion
    }
}
