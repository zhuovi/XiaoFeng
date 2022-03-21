using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-25 18:36:17                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 响应对象
    /// </summary>
    public class HttpResponse : HttpBase,IHttpResponse
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public HttpResponse()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 响应对象
        /// </summary>
        public HttpResponseMessage Response { get; set; }
        /// <summary>
        /// 获取响应请求的 Internet 资源的 URI。
        /// </summary>
        public Uri ResponseUri { get; set; }
        /// <summary>
        /// 获取响应中使用的 HTTP 协议的版本。
        /// </summary>
        public Version ProtocolVersion { get; set; }
        /// <summary>
        /// 获取与响应一起返回的状态说明。
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// 获取响应的状态。
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 获取最后一次修改响应内容的日期和时间。
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }
        /// <summary>
        /// 获取发送响应的服务器的名称。
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 获取响应的字符集。
        /// </summary>
        public string CharacterSet { get; set; }
        /// <summary>
        /// 获取用于对响应体进行编码的方法。
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// 获取请求返回的内容的长度。
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// 结果字节集
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 响应内容
        /// </summary>
        private string _Html;
        /// <summary>
        /// 响应内容
        /// </summary>
        public string Html
        {
            get
            {
                if (this._Html.IsNullOrEmpty())
                    this._Html = this.Data.GetString();
                return this._Html;
            }
        }
        #endregion

        #region 方法

        #region 初始始数据
        /// <summary>
        /// 初始化数据
        /// </summary>
        public async Task InitAsync()
        {
            if (this.Response == null) return;
            //获取StatusCode
            this.StatusCode = this.Response.StatusCode;
            //获取StatusDescription
            this.StatusDescription = this.Response.ReasonPhrase;
            //获取Headers
            this.Headers = new Dictionary<string, string>();
            this.Response.Headers.Each(h =>
              {
                  this.Headers.Add(h.Key, h.Value.FirstOrDefault());
              });
            //获取最后访问的URl
            this.ResponseUri = this.Response.Headers.Location;
            this.ProtocolVersion = this.Response.Version;
            this.ContentEncoding = this.Response.Content.Headers.ContentEncoding.FirstOrDefault();
            this.ContentLength = this.Response.Content.Headers.ContentLength.Value;
            this.ContentType = this.Response.Content.Headers.ContentType?.MediaType;
            this.CharacterSet = this.Response.Content.Headers.ContentType?.CharSet;
            this.Server = this.Response.Headers.Server.ToString();
            this.Method = this.Response.RequestMessage.Method.Method.ToEnum<HttpMethod>();
            this.LastModified = this.Response.Content.Headers.LastModified;
            //获取CookieCollection
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            if(this.Response.Headers.TryGetValues("Set-Cookie",out var Cookies))
            {
                Cookies.Each(c =>
                {
                    var cookie = new Cookie
                    {
                        HttpOnly = c.ToLower().EndsWith("httponly"),
                        Domain = this.Response.RequestMessage.RequestUri.Host
                    };
                    //lang=zh-CN; path=/; secure; samesite=lax; httponly
                    var _c = c.RemovePattern(@"\s+(httponly|samesite=lax|secure)(;|$)");
                    var cs = _c.GetMatches(@"(^|\s+)(?<name>[^=]+)=(?<value>[^;]*)(;|$)");
                    var dict = new Dictionary<string, string>();
                    cs.Each(a =>
                    {
                        dict.Add(a["name"], a["value"]);
                    });
                    if (dict.ContainsKey("domain"))
                    {
                        cookie.Domain = dict["domain"];
                        dict.Remove("domain");
                    }
                    if (dict.ContainsKey("path"))
                    {
                        cookie.Path = dict["path"];
                        dict.Remove("path");
                    }
                    if (dict.ContainsKey("expires"))
                    {
                        //cookie.Expires = DateTime.Parse(dict["expires"]);
                        dict.Remove("expires");
                    }
                    if (dict.ContainsKey("max-age"))
                    {
                        dict.Remove("max-age");
                    }
                    dict.Each(a =>
                    {
                        cookie.Name = a.Key; cookie.Value = a.Value;
                    });
                   this.CookieContainer.Add(cookie);
                });
            }
            /*读取数据*/
            this.Data = await this.GetBytesAsync();
        }
        #endregion
        
        #region 提取网页Byte
        /// <summary>
        /// 提取网页Byte
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> GetBytesAsync()
        {
            byte[] ResponseByte = null;
            using (MemoryStream _stream = new MemoryStream())
            {
                var ContentEncoding = this.ContentEncoding;
                var stream = await this.Response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                /*GZIP处理*/
                if (ContentEncoding.IsNotNullOrEmpty())
                {
                    if (ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        /*开始读取流并设置编码方式*/
                        using (var zip = new GZipStream(stream, CompressionMode.Decompress)) zip.CopyTo(_stream);
                    }
                    else if (ContentEncoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var deflate = new DeflateStream(stream, CompressionMode.Decompress)) deflate.CopyTo(_stream);
                    }
#if !NETFRAMEWORK && !NETSTANDARD2_0
                    else if (ContentEncoding.Equals("br", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var br = new BrotliStream(stream, CompressionMode.Decompress)) br.CopyTo(_stream);
                    }
#endif
                    else
                        /*开始读取流并设置编码方式*/
                        stream.CopyTo(_stream);
                }
                else
                    /*开始读取流并设置编码方式*/
                    stream.CopyTo(_stream);
                /*获取Byte*/
                ResponseByte = _stream.ToArray();
            }
            return ResponseByte;
        }
        #endregion

        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件保存路径</param>
        /// <returns></returns>
        public async Task DownFileAsync(string path)
        {
            path = path.GetBasePath();
            FileHelper.DeleteFile(path);
            using (var file = File.Create(path))
            {
                await file.WriteAsync(this.Data, 0, this.Data.Length).ConfigureAwait(false);
            }
        }
        #endregion

        #region 获取Cookie
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Cookie GetCookie(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            var cookies = this.CookieContainer.GetCookies(this.ResponseUri);
            return cookies[key];
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string GetCookieValue(string key) => this.GetCookie(key).Value;
        #endregion

        #endregion
    }
}