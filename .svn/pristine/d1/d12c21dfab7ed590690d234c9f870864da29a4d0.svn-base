using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-26 15:34:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 响应对象接口
    /// </summary>
    interface IHttpResponse : IHttpBase
    {
        /// <summary>
        /// 响应对象
        /// </summary>
        HttpResponseMessage Response { get; set; }
        /// <summary>
        /// 获取响应请求的 Internet 资源的 URI。
        /// </summary>
        Uri ResponseUri { get; set; }
        /// <summary>
        /// 获取响应中使用的 HTTP 协议的版本。
        /// </summary>
        Version ProtocolVersion { get; set; }
        /// <summary>
        /// 获取与响应一起返回的状态说明。
        /// </summary>
        string StatusDescription { get; set; }
        /// <summary>
        /// 获取响应的状态。
        /// </summary>
        HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 获取最后一次修改响应内容的日期和时间。
        /// </summary>
        DateTimeOffset? LastModified { get; set; }
        /// <summary>
        /// 获取发送响应的服务器的名称。
        /// </summary>
        string Server { get; set; }
        /// <summary>
        /// 获取响应的字符集。
        /// </summary>
        string CharacterSet { get; set; }
        /// <summary>
        /// 获取用于对响应体进行编码的方法。
        /// </summary>
        string ContentEncoding { get; set; }
        /// <summary>
        /// 获取请求返回的内容的长度。
        /// </summary>
        long ContentLength { get; set; }
        /// <summary>
        /// 结果字节集
        /// </summary>
        byte[] Data { get; set; }
        /// <summary>
        /// 响应内容
        /// </summary>
        string Html { get; }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件保存路径</param>
        /// <returns></returns>
        Task DownFileAsync(string path);
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        Cookie GetCookie(string key);
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        string GetCookieValue(string key);
    }
}