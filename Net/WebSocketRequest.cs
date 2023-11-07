using System;
using System.IO;
using System.Net;
using XiaoFeng.Http;
/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-08-14 11:07:39                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocket请求类
    /// </summary>
    public class WebSocketRequest : WebSocketRequestOptions
    {
        #region 构造器
        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="Scheme">URI 的方案名称。</param>
        /// <param name="requestHeader">请求头信息</param>
        public WebSocketRequest(string Scheme, string requestHeader)
        {
            RequestHeader = requestHeader;
            if (RequestHeader.IndexOf("Sec-WebSocket-Key", StringComparison.OrdinalIgnoreCase) > -1)
            {
                StringReader sr = new StringReader(RequestHeader);
                string line;
                string path = "/";
                do
                {
                    line = sr.ReadLine();
                    if (line == null) break;
                    var index = line.IndexOf(":");
                    if (index == -1)
                    {
                        if (line.StartsWith("get", StringComparison.OrdinalIgnoreCase))
                        {
                            var s = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            this.Method = HttpMethod.Get;
                            if (s.Length > 2)
                            {
                                path = s[1];
                                this.HttpVersion = (s[2].RemovePattern("HTTP/").ToCast<double>() * 10).ToCast<HttpVersionX>();
                            }
                        }
                        continue;
                    }
                    var key = line.Substring(0, index).Trim();
                    var val = line.Substring(index + 1).Trim();
                    base[key] = val;
                } while (line != null);
                if (this[HttpRequestHeader.Host] != null)
                    this.Uri = new Uri(Scheme + "://" + this[HttpRequestHeader.Host] + path);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 请求头信息
        /// </summary>
        private string RequestHeader { get; set; }

        #endregion
    }
}