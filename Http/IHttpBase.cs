using System;
using System.Collections.Generic;
using System.Net;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-26 15:59:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 基础接口
    /// </summary>
    public interface IHttpBase
    {
        /// <summary>
        /// 请求内核
        /// </summary>
        HttpCore HttpCore { get; set; }
        /// <summary>
        /// 获取或设置与此响应关联的 Cookie
        /// </summary>
        CookieContainer CookieContainer { get; set; }
        /// <summary>
        /// 指定构成 HTTP 标头的名称/值对的集合。
        /// </summary>
        IDictionary<string, string> Headers { get; set; }
        /// <summary>
        /// 获取或设置请求的方法
        /// </summary>
        HttpMethod Method { get; set; }
        /// <summary>
        /// 请求或响应内容类型
        /// </summary>
        string ContentType { get; set; }
        /// <summary>
        /// 请求内容长度
        /// </summary>
        long ContentLength { get; set; }
        /// <summary>
        /// 开始请求时间
        /// </summary>
        DateTime BeginTime { get; }
        /// <summary>
        /// 请求结束时间
        /// </summary>
        DateTime EndTime { get; }
        /// <summary>
        /// 运行总毫秒数
        /// </summary>
        long RunTime { get; }
    }
}