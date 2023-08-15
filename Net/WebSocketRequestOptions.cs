using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using XiaoFeng.Http;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-08-15 14:44:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocket请求配置
    /// </summary>
    public class WebSocketRequestOptions
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public WebSocketRequestOptions()
        {
            this.WebHeader = new WebHeaderCollection();
            this[HttpRequestHeader.Connection] = "Upgrade";
            this[HttpRequestHeader.Pragma] = "no-cache";
            this["Sec-WebSocket-Extensions"] = "permessage-deflate; client_max_window_bits";
            this["Sec-WebSocket-Version"] = "13";
            this[HttpRequestHeader.Upgrade] = "websocket";
            this[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36";
            this[HttpRequestHeader.AcceptEncoding] = (AcceptEncodingType.GZIP | AcceptEncodingType.DEFLATE | AcceptEncodingType.BR).ToString().ToLower();
            this[HttpRequestHeader.CacheControl] = "no-cache";
            this[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh;q=0.9";
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="header">key</param>
        /// <returns></returns>
        public String this[HttpRequestHeader header]
        {
            get => this.Get(header);
            set => this.Set(header, value);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="header">key</param>
        /// <returns></returns>
        public String this[HttpResponseHeader header]
        {
            get => this.Get(header);
            set => this.Set(header, value);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public String this[String key]
        {
            get => this.Get(key);
            set => this.Set(key, value);
        }
        /// <summary>
        /// 请求地址
        /// </summary>
        private Uri _Uri;
        /// <summary>
        /// 请求地址
        /// </summary>
        public Uri Uri
        {
            get => this._Uri; set
            {
                this._Uri = value;
                this[HttpRequestHeader.Host] = value.Host + ":" + value.Port;
            }
        }
        /// <summary>
        /// 请求类型
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        /// <summary>
        /// Http协议版本
        /// </summary>
        public HttpVersionX HttpVersion { get; set; } = HttpVersionX.Version11;
        /// <summary>
        /// 指定要访问的服务器地址。
        /// </summary>
        public String Host => this[HttpRequestHeader.Host];
        /// <summary>
        /// 指定客户端和服务器之间的连接类型，如 keep-alive、close 等。
        /// </summary>
        public String Connection => this[HttpRequestHeader.Connection];
        /// <summary>
        /// 用来包含实现特定的指令。
        /// </summary>
        public String Pragma => this[HttpRequestHeader.Pragma];
        /// <summary>
        /// 源头
        /// </summary>
        public String Origin
        {
            get => this["Origin"];
            set => this["Origin"] = value;
        }
        /// <summary>
        /// 客户端支持的扩展列表
        /// </summary>
        public String SecWebSocketExtensions
        {
            get => this["Sec-WebSocket-Extensions"];
            set => this["Sec-WebSocket-Extensions"] = value;
        }
        /// <summary>
        /// 请求标识key
        /// </summary>
        public String SecWebSocketKey {
            get => this["Sec-WebSocket-Key"];
            set => this["Sec-WebSocket-Key"] = value;
        }
        /// <summary>
        /// WebSocket版本
        /// </summary>
        public String SecWebSocketVersion => this["Sec-WebSocket-Version"];
        /// <summary>
        /// 向服务器指定某种传输协议以便服务器进行转换。
        /// </summary>
        public String Upgrade => this[HttpRequestHeader.Upgrade];
        /// <summary>
        /// 标识浏览器的详细信息，包括名称、版本、操作系统等。
        /// </summary>
        public String UserAgent {
            get => this[HttpRequestHeader.UserAgent];
            set => this[HttpRequestHeader.UserAgent] = value;
        }
        /// <summary>
        /// 指定客户端可以接受的压缩编码类型
        /// </summary>
        public AcceptEncodingType AcceptEncoding
        {
            get => this[HttpRequestHeader.AcceptEncoding].ToUpper().ToCast<AcceptEncodingType>();
            set => this[HttpRequestHeader.AcceptEncoding] = value.ToString().ToLower();
        }
        /// <summary>
        /// 指定客户端的缓存策略，如 no-cache、max-age 等。
        /// </summary>
        public String CacheControl => this[HttpRequestHeader.CacheControl];
        /// <summary>
        /// 指定客户端接受的语言类型和优先级。
        /// </summary>
        public String AcceptLanguage
        {
            get => this[HttpRequestHeader.AcceptLanguage];
            set => this[HttpRequestHeader.AcceptLanguage] = value;
        }
        /// <summary>
        /// 指定请求来源网页的 URL。
        /// </summary>
        public String Referer { get; set; }
        /// <summary>
        /// 发出请求的用户的Email。
        /// </summary>
        public String From
        {
            get => this[HttpRequestHeader.From];
            set => this[HttpRequestHeader.From] = value;
        }
        /// <summary>
        /// 希望向服务器进行身份验证的用户代理 凭证是 (用户名:密码)的base64
        /// </summary>
        public String Authorization
        {
            get => this[HttpRequestHeader.Authorization];
            set => this[HttpRequestHeader.Authorization] = value;
        }
        /// <summary>
        /// 代理认证
        /// </summary>
        public String ProxyAuthorization
        {
            get => this[HttpRequestHeader.ProxyAuthorization];
            set => this[HttpRequestHeader.ProxyAuthorization] = value;
        }
        /// <summary>
        /// Cookie
        /// </summary>
        public String Cookie
        {
            get => this[HttpRequestHeader.Cookie];
            set => this[HttpRequestHeader.Cookie] = value;
        }
        /// <summary>
        /// 自定义Header
        /// </summary>
        private WebHeaderCollection WebHeader { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 设置头信息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Set(string key, string value)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Set(key, value);
        }
        /// <summary>
        /// 设置头信息
        /// </summary>
        /// <param name="header">key</param>
        /// <param name="value">value</param>
        public void Set(HttpRequestHeader header,string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Set(header, value);
        }
        /// <summary>
        /// 设置头信息
        /// </summary>
        /// <param name="header">key</param>
        /// <param name="value">value</param>
        public void Set(HttpResponseHeader header,string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Set(header, value);
        }
        /// <summary>
        /// 添加头信息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Add(string key, string value)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Add(key, value);
        }
        /// <summary>
        /// 添加头信息
        /// </summary>
        /// <param name="header">key</param>
        /// <param name="value">value</param>
        public void Add(HttpRequestHeader header, string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Add(header, value);
        }
        /// <summary>
        /// 添加头信息
        /// </summary>
        /// <param name="header">key</param>
        /// <param name="value">value</param>
        public void Add(HttpResponseHeader header, string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Add(header, value);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="header">key</param>
        /// <returns></returns>
        public string Get(HttpRequestHeader header)
        {
            if (this.WebHeader == null) return String.Empty;
            return this.WebHeader[header];
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (this.WebHeader == null) return String.Empty;
            return this.WebHeader[key];
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="header">key</param>
        /// <returns></returns>
        public string Get(HttpResponseHeader header)
        {
            if (this.WebHeader == null) return String.Empty;
            return this.WebHeader[header];
        }
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sbr = new StringBuilder();
            sbr.AppendLine($"{this.Method} {this.Uri?.PathAndQuery} HTTP/{((float)this.HttpVersion) / 10F}");
            sbr.Append(this.WebHeader.ToString());
            return sbr.ToString();
        }
        #endregion
    }
    /// <summary>
    /// 压缩编码类型
    /// </summary>
    [Flags]
    public enum AcceptEncodingType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// gzip
        /// </summary>
        GZIP = 1 << 0,
        /// <summary>
        /// deflate
        /// </summary>
        DEFLATE = 1 << 1,
        /// <summary>
        /// br
        /// </summary>
        BR = 1 << 2
    }
}