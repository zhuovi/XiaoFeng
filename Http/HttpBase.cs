using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-25 18:35:08                            *
*  Version : v 1.0.0                                            *
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
        /// 获取或设置与此响应关联的 Cookie
        /// </summary>
        public CookieContainer CookieContainer { get; set; }
        /// <summary>
        /// 指定构成 HTTP 标头的名称/值对的集合。
        /// </summary>
        public Dictionary<string,string> Headers { get; set; }
        /// <summary>
        /// 获取或设置请求的方法
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        /// <summary>
        /// 请求或响应内容类型
        /// </summary>
        public string ContentType { get; set; }
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
        #endregion

        #region 方法

        #endregion
    }
}