using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-25 18:36:17                            *
*  Update Time : 2022-12-26 11:41:25                            *
*  Version : v 1.0.1                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 响应对象
    /// </summary>
    public class HttpResponse : HttpBase, IHttpResponse
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
        /// 请求对象
        /// </summary>
        public IHttpRequest Request { get; set; }
        /// <summary>
        /// 响应对象
        /// </summary>
        public HttpResponseMessage Response { get; set; }
        /// <summary>
        /// 响应对象
        /// </summary>
        public HttpWebResponse ResponseHttp { get; set; }
        /// <summary>
        /// 获取响应请求的 Internet 资源的 URI。
        /// </summary>
        public Uri ResponseUri { get; set; }
        /// <summary>
        /// 转换 URI 列表
        /// </summary>
        public List<Uri> ResponseUris { get; set; }
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
        /// 是否分块响应
        /// </summary>
        public Boolean IsChunked { get; set; } = false;
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
                    this._Html = this.Data.GetString(this.CharacterSet);
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
                  if (h.Key.EqualsIgnoreCase("Authorization")) this.Authorization = h.Value.ToString();
              });
            //获取最后访问的URl
            this.ResponseUri = this.Response.Headers.Location;
            if (this.ResponseUri.IsNullOrEmpty())
                this.ResponseUri = this.Request.Request.RequestUri;
            this.ProtocolVersion = this.Response.Version;
            this.ContentEncoding = this.Response.Content.Headers.ContentEncoding.FirstOrDefault();
            this.ContentLength = this.Response.Content.Headers.ContentLength.Value;
            this.ContentType = this.Response.Content.Headers.ContentType?.MediaType;
            this.CharacterSet = this.Response.Content.Headers.ContentType?.CharSet;
            this.Server = this.Response.Headers.Server.ToString();
            this.Method = (HttpMethod)this.Response.RequestMessage.Method;
            this.LastModified = this.Response.Content.Headers.LastModified;
            //获取CookieCollection
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            if (this.Response.Headers.TryGetValues("Set-Cookie", out var Cookies))
            {
                Cookies.Each(c =>
                {
                    var uri = new Uri(this.Request.Address);
                    Cookie cookie = null;
                    //lang=zh-CN; path=/; secure; samesite=lax; httponly
                    var _c = c.RemovePattern(@"\s+(httponly|samesite=lax|secure)(;|$)");
                    var cs = _c.GetMatches(@"(^|\s+)(?<name>[^=]+)=(?<value>[^;]*)(;|$)");
                    cs.Each(a =>
                    {
                        var name = a["name"];
                        var value = a["value"];
                        if (name.IsNullOrEmpty()) return;
                        switch (name.ToLower())
                        {
                            case "domain":
                                cookie.Domain = value;
                                break;
                            case "path":
                                cookie.Path = value;
                                break;
                            case "expires":
                                if (DateTime.TryParse(value, out var d))
                                    cookie.Expires = d;
                                break;
                            case "max-age":
                                break;
                            case "secure":
                                cookie.Secure = true;
                                break;
                            case "httponlay":
                                cookie.HttpOnly = true;
                                break;
                            default:
                                if (cookie != null)
                                    this.CookieContainer.Add(cookie);
                                cookie = new Cookie
                                {
                                    HttpOnly = c.ToLower().EndsWith("httponly"),
                                    Domain = uri.Host
                                };
                                cookie.Name = name;
                                cookie.Value = value;
                                break;
                        }
                    });
                    if (cookie != null)
                        this.CookieContainer.Add(cookie);
                });
            }
            /*读取数据*/
            this.Data = await this.GetBytesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public async Task InitHttpAsync()
        {
            if (this.ResponseHttp == null) return;
            //获取StatusCode
            this.StatusCode = this.ResponseHttp.StatusCode;
            //获取StatusDescription
            this.StatusDescription = this.ResponseHttp.StatusDescription;
            //获取Headers
            this.Headers = new Dictionary<string, string>();
            this.ResponseHttp.Headers.AllKeys.Each(k =>
            {
                this.Headers.Add(k, this.ResponseHttp.Headers[k]);
                if (k.EqualsIgnoreCase("Authorization"))
                    this.Authorization = this.ResponseHttp.Headers[k];
            });
            //获取最后访问的URl
            var location = this.ResponseHttp.Headers.Get(HttpRequestHeader.ContentLocation.ToString());
            if (location.IsNotNullOrEmpty())
                this.ResponseUri = new Uri(location);
            else
                this.ResponseUri = this.ResponseHttp.ResponseUri;
            this.ProtocolVersion = this.ResponseHttp.ProtocolVersion;
            this.ContentEncoding = this.ResponseHttp.ContentEncoding;
            this.ContentLength = this.ResponseHttp.ContentLength;
            this.ContentType = this.ResponseHttp.ContentType;
            this.CharacterSet = this.ResponseHttp.CharacterSet;
            this.Server = this.ResponseHttp.Server;
            this.Method = (HttpMethod)this.ResponseHttp.Method;
            this.LastModified = this.ResponseHttp.LastModified;
            //获取CookieCollection
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            if (this.ResponseHttp.Cookies != null && this.ResponseHttp.Cookies.Count > 0)
            {
                if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
                this.CookieContainer.Add(this.ResponseHttp.Cookies);
            }
            /*读取数据*/
            this.Data = await this.GetBytesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public async Task InitSocketAsync()
        {
            if (this.Headers == null || this.Headers.Count == 0) return;

            this.ProtocolVersion = this.Request.ProtocolVersion;
            if (this.Headers.TryGetValue("Content-Encoding", out var ContentEncoding))
            {
                this.ContentEncoding = ContentEncoding;
            }
            if (this.Headers.TryGetValue("Content-Length", out var ContentLength))
            {
                this.ContentLength = ContentLength.ToCast<long>();
            }
            if (this.Headers.TryGetValue("Content-Type", out var ContentType))
            {
                if (ContentType.Contains(";"))
                {
                    var _ContentType = ContentType.Split(';');

                    this.ContentType = _ContentType[0];
                    var charset = _ContentType[1].Split(':');
                    if (charset.Length == 2) this.CharacterSet = charset[1];
                }
                else
                    this.ContentType = ContentType;
            }
            if (this.Headers.TryGetValue("Server", out var Server))
            {
                this.Server = Server;
            }
            this.Method = (HttpMethod)this.Request.Method;
            if (this.Headers.TryGetValue("Last-Modified", out var LastModified))
            {
                this.LastModified = new DateTimeOffset(DateTime.Parse(LastModified, CultureInfo.InvariantCulture));
            }
            //获取CookieCollection
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            if (this.Headers.TryGetValue("Set-Cookie", out var Cookies))
            {
                var uri = new Uri(this.Request.Address);
                Cookie cookie = null;
                //lang=zh-CN; path=/; secure; samesite=lax; httponly
                var _c = Cookies.RemovePattern(@"\s+(httponly|samesite=lax|secure)(;|$)");
                var cs = _c.GetMatches(@"(^|\s+)(?<name>[^=]+)=(?<value>[^;]*)(;|$)");
                cs.Each(a =>
                {
                    var name = a["name"];
                    var value = a["value"];
                    if (name.IsNullOrEmpty()) return;
                    switch (name.ToLower())
                    {
                        case "domain":
                            cookie.Domain = value;
                            break;
                        case "path":
                            cookie.Path = value;
                            break;
                        case "expires":
                            if (DateTime.TryParse(value, out var d))
                                cookie.Expires = d;
                            break;
                        case "max-age":
                            break;
                        case "secure":
                            cookie.Secure = true;
                            break;
                        case "httponlay":
                            cookie.HttpOnly = true;
                            break;
                        default:
                            if (cookie != null)
                                this.CookieContainer.Add(cookie);
                            cookie = new Cookie
                            {
                                HttpOnly = Cookies.ToLower().EndsWith("httponly"),
                                Domain = uri.Host
                            };
                            cookie.Name = name;
                            cookie.Value = value;
                            break;
                    }
                });
                if (cookie != null)
                    this.CookieContainer.Add(cookie);
            }
            /*读取数据*/
            this.Data = await this.GetBytesAsync().ConfigureAwait(false);
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
                Stream stream;
                if (this.HttpCore == HttpCore.HttpClient)
                {
                    stream = await this.Response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                }
                else if (this.HttpCore == HttpCore.HttpWebRequest)
                {
                    stream = this.ResponseHttp.GetResponseStream();
                }
                else
                    stream = new MemoryStream(this.Data);

                /*GZIP处理*/
                if (ContentEncoding.IsNotNullOrEmpty())
                {
                    try
                    {
                        if (ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            /*开始读取流并设置编码方式*/
                            using (var zip = new GZipStream(stream, CompressionMode.Decompress))
                            {
                                //var bytes = new byte[stream.Length];
                                //var length = await zip.ReadAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                                //await _stream.WriteAsync(bytes, 0, length).ConfigureAwait(false);
                                await zip.CopyToAsync(_stream).ConfigureAwait(false);
                            }
                        }
                        else if (ContentEncoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
                        {
                            using (var deflate = new DeflateStream(stream, CompressionMode.Decompress)) await deflate.CopyToAsync(_stream).ConfigureAwait(false);
                        }
#if !NETFRAMEWORK && !NETSTANDARD2_0
                        else if (ContentEncoding.Equals("br", StringComparison.InvariantCultureIgnoreCase))
                        {
                            using (var br = new BrotliStream(stream, CompressionMode.Decompress)) await br.CopyToAsync(_stream).ConfigureAwait(false);
                        }
#endif
                        else
                            /*开始读取流并设置编码方式*/
                            stream.CopyTo(_stream);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "解压出错.");
                    }
                }
                else
                    /*开始读取流并设置编码方式*/
                    stream.CopyTo(_stream);
                /*获取Byte*/
                ResponseByte = _stream.ToArray();
            }
            return await Task.FromResult(ResponseByte);
        }
        #endregion

        #region 设置结束时间
        /// <summary>
        /// 设置结束时间
        /// </summary>
        public void SetEndTime() => this.EndTime = DateTime.Now;
        /// <summary>
        /// 设置开始结束时间
        /// </summary>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        public void SetBeginAndEndTime(DateTime begin, DateTime end)
        {
            this.BeginTime = begin;
            this.EndTime = end;
        }
        /// <summary>
        /// 设置开始时间
        /// </summary>
        public void SetBeginTime() => this.BeginTime = DateTime.Now;
        #endregion

        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件保存路径</param>
        /// <returns>运行时长</returns>
        public async Task<long> DownFileAsync(string path)
        {
            path = path.GetBasePath();
            FileHelper.CreateDirectory(path.GetDirectoryName());
            FileHelper.DeleteFile(path);
            using (var file = File.Create(path))
                await file.WriteAsync(this.Data, 0, this.Data.Length).ConfigureAwait(false);
            this.EndTime = DateTime.Now;
            return this.RunTime;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件保存路径</param>
        /// <returns>运行时长</returns>
        public long DownFile(string path)
        {
            path = path.GetBasePath();
            FileHelper.CreateDirectory(path.GetDirectoryName());
            FileHelper.DeleteFile(path);
            using (var file = File.Create(path))
                file.Write(this.Data, 0, this.Data.Length);
            this.EndTime = DateTime.Now;
            return this.RunTime;
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
            if (this.ResponseUri.IsNullOrEmpty()) return null;
            var cookies = this.CookieContainer.GetCookies(this.ResponseUri.Multivariate(new Uri(this.Request.Address)));
            return cookies[key];
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string GetCookieValue(string key) => this.GetCookie(key).Value;
        #endregion

        #region 获取Header
        /// <summary>
        /// 获取Header值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string GetHeader(string key)
        {
            if (this.Headers != null && this.Headers.Count > 0 && this.Headers.TryGetValue(key, out var val))
                return val;
            return String.Empty;
        }
        /// <summary>
        /// 获取Header值
        /// </summary>
        /// <param name="header">key</param>
        /// <returns></returns>
        public string GetHeader(HttpRequestHeader header) => this.GetHeader(header.ToString());
        #endregion

        #region 释放资源
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放状态</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing, () =>
            {
                if (this.Request != null)
                {
                    this.Request.Dispose();
                    this.Request = null;
                }
                if(this.Response != null)
                {
                    this.Response.Dispose();
                    this.Response = null;
                }
                if (this.ResponseHttp != null)
                {
                    this.ResponseHttp.Dispose();
                    this.ResponseHttp = null;
                }
            });
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~HttpResponse()
        {
            this.Dispose(false);
        }
        #endregion

        #endregion
    }
}