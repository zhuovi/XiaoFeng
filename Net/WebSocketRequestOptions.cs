using System;
using System.Net;
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
    /// WebSocketClient请求配置
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
            this[HttpRequestHeader.AcceptEncoding] = (AcceptEncodingType.GZIP | AcceptEncodingType.DEFLATE | AcceptEncodingType.BR).ToLower();
            this[HttpRequestHeader.CacheControl] = "no-cache";
            this[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6";
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取请求头值
        /// </summary>
        /// <param name="header">头key <see cref="HttpRequestHeader"/></param>
        /// <returns>请求头值</returns>
        public String this[HttpRequestHeader header]
        {
            get => this.Get(header);
            set => this.Set(header, value);
        }
        /// <summary>
        /// 获取请求头值
        /// </summary>
        /// <param name="header">头key <see cref="HttpResponseHeader"/></param>
        /// <returns>请求头值</returns>
        public String this[HttpResponseHeader header]
        {
            get => this.Get(header);
            set => this.Set(header, value);
        }
        /// <summary>
        /// 获取请求头值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>请求头值</returns>
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
        /// 请求地址 <see cref="System.Uri"/>
        /// </summary>
        public Uri Uri
        {
            get => this._Uri; set
            {
                this._Uri = value;
                this[HttpRequestHeader.Host] = value.Host + (value.Port == 80 || value.Port == 443 ? "" : (":" + value.Port));
            }
        }
        /// <summary>
        /// 请求类型 <see cref="XiaoFeng.Http.HttpMethod"/> 或 <see cref="System.Net.Http.HttpMethod"/>
        /// </summary>
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        /// <summary>
        /// Http协议版本 1.0,1.1,2.0,3.0
        /// </summary>
        public HttpVersionX HttpVersion { get; set; } = HttpVersionX.Version11;
        /// <summary>
        /// Host 标头，指定要请求的资源的主机名和端口号。
        /// </summary>
        public String Host => this[HttpRequestHeader.Host];
        /// <summary>
        /// Connection 标头，指定特定连接所需的选项。指定客户端和服务器之间的连接类型，如 keep-alive、close 等。
        /// </summary>
        public String Connection => this[HttpRequestHeader.Connection];
        /// <summary>
        /// Pragma 标头，指定特定于实现的指令，这些指令可应用到请求/响应链上的任意代理。用来包含实现特定的指令。
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
        public String SecWebSocketKey
        {
            get => this["Sec-WebSocket-Key"];
            set => this["Sec-WebSocket-Key"] = value;
        }
        /// <summary>
        /// WebSocket版本
        /// </summary>
        public String SecWebSocketVersion => this["Sec-WebSocket-Version"];
        /// <summary>
        /// Upgrade 标头，指定客户端支持的其他通信协议。向服务器指定某种传输协议以便服务器进行转换。
        /// </summary>
        public String Upgrade => this[HttpRequestHeader.Upgrade];
        /// <summary>
        /// User-Agent 标头，指定有关客户端代理的信息。标识浏览器的详细信息，包括名称、版本、操作系统等。
        /// </summary>
        public String UserAgent
        {
            get => this[HttpRequestHeader.UserAgent];
            set => this[HttpRequestHeader.UserAgent] = value;
        }
        /// <summary>
        /// Accept-Charset 标头，指定响应可接受的内容编码。指定客户端可以接受的压缩编码类型
        /// </summary>
        public AcceptEncodingType AcceptEncoding
        {
            get => this[HttpRequestHeader.AcceptEncoding].ToUpper().ToCast<AcceptEncodingType>();
            set => this[HttpRequestHeader.AcceptEncoding] = value.ToLower();
        }
        /// <summary>
        /// Cache-Control 标头，指定请求/响应链上所有缓存控制机制必须服从的指令。如 no-cache、max-age 等。
        /// </summary>
        public String CacheControl => this[HttpRequestHeader.CacheControl];
        /// <summary>
        /// Accept-Langauge 标头，指定用于响应的首选自然语言。。
        /// </summary>
        public String AcceptLanguage
        {
            get => this[HttpRequestHeader.AcceptLanguage];
            set => this[HttpRequestHeader.AcceptLanguage] = value;
        }
        /// <summary>
        /// Referer 标头，指定可从中获取请求 URI 的资源 URI。
        /// </summary>
        public String Referer
        {
            get => this[HttpRequestHeader.Referer];
            set => this[HttpRequestHeader.Referer] = value;
        }
        /// <summary>
        /// From 标头，指定控制请求的用户代理的用户的 Internet 电子邮件地址。
        /// </summary>
        public String From
        {
            get => this[HttpRequestHeader.From];
            set => this[HttpRequestHeader.From] = value;
        }
        /// <summary>
        /// Authorization 标头，指定客户端提供的以向服务器验证自身身份的凭据。 凭证是 (用户名:密码)的base64
        /// </summary>
        /// <remarks>
        /// <para>假设用户名为jacky密码为eelf.cn则代码就是
        /// <code>Convert.ToBase64String(Encoding.UTF8.GetBytes("jacky:eelf.cn"))</code>
        /// </para>
        /// <code>
        /// 则上面的帐号密码输出为以下
        /// 
        /// Authorization: Basic amFja3k6ZWVsZi5jbg==
        /// </code>
        /// </remarks>
        public String Authorization
        {
            get => this[HttpRequestHeader.Authorization];
            set => this[HttpRequestHeader.Authorization] = value;
        }
        /// <summary>
        /// Proxy-Authorization 标头，指定客户端提供的以向代理验证自身身份的凭据。
        /// </summary>
        /// <remarks>
        /// 设置参照 <see cref="Authorization"/> 属性设置
        /// </remarks>
        public String ProxyAuthorization
        {
            get => this[HttpRequestHeader.ProxyAuthorization];
            set => this[HttpRequestHeader.ProxyAuthorization] = value;
        }
        /// <summary>
        ///  Cookie 标头，指定向服务器提供的 cookie 数据。
        /// </summary>
        public String Cookie
        {
            get => this[HttpRequestHeader.Cookie];
            set => this[HttpRequestHeader.Cookie] = value;
        }
        /// <summary>
        ///  Content-Encoding 标头，指定应用到随附的正文数据的编码。
        /// </summary>
        public String ContentEncoding
        {
            get => this[HttpRequestHeader.ContentEncoding];
            set => this[HttpRequestHeader.ContentEncoding] = value;
        }
        /// <summary>
        ///  Content-Language 标头，指定随附的正文数据的自然语言。
        /// </summary>
        public String ContentLangauge
        {
            get => this[HttpRequestHeader.ContentLanguage];
            set => this[HttpRequestHeader.ContentLanguage] = value;
        }
        /// <summary>
        ///  Content-MD5 标头，指定随附的正文数据的 MD5 摘要，以便提供端到端消息完整性检查。 由于 MD5 出现冲突问题，Microsoft 建议使用基于SHA256 或更高版本的安全模型。
        /// </summary>
        public String ContentMd5
        {
            get => this[HttpRequestHeader.ContentMd5];
            set => this[HttpRequestHeader.ContentMd5] = value;
        }
        /// <summary>
        /// 自定义协议标头。
        /// </summary>
        private WebHeaderCollection WebHeader { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 设置协议标头信息
        /// </summary>
        /// <param name="key">协议标头key</param>
        /// <param name="value">协议标头value</param>
        public void Set(string key, string value)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Set(key, value);
        }
        /// <summary>
        /// 设置协议标头信息
        /// </summary>
        /// <param name="header">协议标头key <see cref="HttpRequestHeader"/></param>
        /// <param name="value">协议标头value</param>
        public void Set(HttpRequestHeader header, string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Set(header, value);
        }
        /// <summary>
        /// 设置协议标头信息
        /// </summary>
        /// <param name="header">协议标头 key <see cref="HttpResponseHeader"/></param>
        /// <param name="value">协议标头 value</param>
        public void Set(HttpResponseHeader header, string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Set(header, value);
        }
        /// <summary>
        /// 添加协议标头信息
        /// </summary>
        /// <param name="key">协议标头 key</param>
        /// <param name="value">协议标头 value</param>
        public void Add(string key, string value)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Add(key, value);
        }
        /// <summary>
        /// 添加协议标头信息
        /// </summary>
        /// <param name="header">协议标头 key <see cref="HttpRequestHeader"/></param>
        /// <param name="value">协议标头 value</param>
        public void Add(HttpRequestHeader header, string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Add(header, value);
        }
        /// <summary>
        /// 添加协议标头信息
        /// </summary>
        /// <param name="header">协议标头 key <see cref="HttpResponseHeader"/></param>
        /// <param name="value">协议标头 value</param>
        public void Add(HttpResponseHeader header, string value)
        {
            if (this.WebHeader == null) this.WebHeader = new WebHeaderCollection();
            this.WebHeader.Add(header, value);
        }
        /// <summary>
        /// 获取协议标头数据
        /// </summary>
        /// <param name="header">协议标头 key <see cref="HttpRequestHeader"/></param>
        /// <returns>协议标头值</returns>
        public string Get(HttpRequestHeader header)
        {
            if (this.WebHeader == null) return String.Empty;
            return this.WebHeader[header];
        }
        /// <summary>
        /// 获取协议标头数据
        /// </summary>
        /// <param name="key">协议标头 key</param>
        /// <returns>协议标头值</returns>
        public string Get(string key)
        {
            if (this.WebHeader == null) return String.Empty;
            return this.WebHeader[key];
        }
        /// <summary>
        /// 获取响应协议标头数据
        /// </summary>
        /// <param name="header">响应协议标头key <see cref="HttpResponseHeader"/></param>
        /// <returns>响应协议标头数据</returns>
        public string Get(HttpResponseHeader header)
        {
            if (this.WebHeader == null) return String.Empty;
            return this.WebHeader[header];
        }
        /// <summary>
        /// 转成协议标头字符串
        /// </summary>
        /// <returns>合并协议标头字符串</returns>
        public override string ToString()
        {
            var sbr = new StringBuilder();
            sbr.AppendLine($"{this.Method.ToString().ToUpper()} {this.Uri?.PathAndQuery} HTTP/{((float)this.HttpVersion) / 10F}");
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