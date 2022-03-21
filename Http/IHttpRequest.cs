using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-26 15:34:01                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 请求对象接口
    /// </summary>
    public interface IHttpRequest : IHttpBase
    {
        /// <summary>
        /// 操作是在响应可利用时立即视为已完成，还是在读取包含上下文的整个答案信息之后才视为已完成。
        /// </summary>
        HttpCompletionOption CompletionOption { get; set; }
        /// <summary>
        /// 设置Host的标头信息
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// 获取或设置与此响应关联的 Cookie
        /// </summary>
        string Cookies { set; }
        /// <summary>
        /// 请求网址
        /// </summary>
        string Address { get; set; }
        /// <summary>
        /// 请求内容
        /// </summary>
        HttpContent HttpContent { get; set; }
        /// <summary>
        /// 请求网址编码
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// 获取或设置 User-agent HTTP 标头的值。
        /// </summary>
        string UserAgent { get; set; }
        /// <summary>
        /// 请求超时时间 单位为毫秒
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// 默认写入Post数据超时时间 单位为毫秒
        /// </summary>
        int ReadWriteTimeout { get; set; }
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        string Accept { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示请求是否应跟随重定向响应。
        /// </summary>
        Boolean AllowAutoRedirect { get; set; }
        /// <summary>
        /// 获取或设置请求将跟随的重定向的最大数目。
        /// </summary>
        int MaximumAutomaticRedirections { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接。
        /// </summary>
        Boolean KeepAlive { get; set; }
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        X509Certificate2Collection ClentCertificates { get; set; }
        /// <summary>
        /// 获取和设置IfModifiedSince，默认为当前日期和时间
        /// </summary>
        DateTime? IfModifiedSince { get; set; }
        /// <summary>
        /// 设置本地的出口ip和端口
        /// </summary>
        IPEndPoint IPEndPoint { get; set; }
        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        ICredentials ICredentials { get; set; }
        /// <summary>
        /// 指定 Schannel 安全包支持的安全协议
        /// </summary>
        SecurityProtocolType ProtocolType { get; set; }
        /// <summary>
        /// 获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        Version ProtocolVersion { get; set; }
        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        Boolean Expect100Continue { get; set; }
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        string CertPath { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        string CertPassWord { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        int Connectionlimit { get; set; }
        /// <summary>
        /// Http 代理
        /// </summary>
        WebProxy WebProxy { get; set; }
        /// <summary>
        /// 获取或设置 Referer HTTP 标头的值。
        /// </summary>
        string Referer { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        Dictionary<string, string> Data { get; set; }
        /// <summary>
        /// Body请求数据
        /// </summary>
        string BodyData { get; set; }
        /// <summary>
        /// FormData数据
        /// </summary>
        List<FormData> FormData { get; set; }
        /// <summary>
        /// 请求完后是否重置请求对象以及响应对象
        /// </summary>
        Boolean IsReset { get; set; }
        /// <summary>
        /// 请求消息
        /// </summary>
        HttpRequestMessage Request { get; set; }
        /// <summary>
        /// 压缩方式
        /// </summary>
        DecompressionMethods DecompressionMethod { get; set; }
        /// <summary>
        /// 获取响应数据
        /// </summary>
        /// <returns></returns>
        Task<HttpResponse> GetResponseAsync();
        /// <summary>
        /// 取消请求
        /// </summary>
        void Abort();
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        void AddCookie(string name, string value);
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookie">cookie</param>
        void AddCookie(Cookie cookie);
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="collection">cookie集</param>
        void AddCookie(CookieCollection collection);
    }
}