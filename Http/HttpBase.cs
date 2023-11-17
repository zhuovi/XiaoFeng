using System;
using System.Collections.Generic;
using System.Net;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-25 18:35:08                            *
*  Update Time : 2022-12-26 11:41:25                            *
*  Version : v 1.0.1                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// HTTP 基础类
    /// </summary>
    public class HttpBase : IHttpBase
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public HttpBase()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 请求内核
        /// </summary>
        public HttpCore HttpCore { get; set; } = HttpCore.HttpClient;
        /// <summary>
        /// 获取或设置与此响应关联的 Cookie
        /// </summary>
        public CookieContainer CookieContainer { get; set; }
        /// <summary>
        /// 指定构成 HTTP 标头的名称/值对的集合。
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
        /// <summary>
        /// 获取或设置请求的方法
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        /// <summary>
        /// 请求或响应内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 请求内容长度
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// 开始请求时间
        /// </summary>
        public DateTime BeginTime { get; protected set; }
        /// <summary>
        /// 请求结束时间
        /// </summary>
        public DateTime EndTime { get; protected set; }
        /// <summary>
        /// 运行总毫秒数
        /// </summary>
        public long RunTime => (long)(EndTime - BeginTime).TotalMilliseconds;
        /// <summary>
        /// 认证信息
        /// </summary>
        public string Authorization { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}