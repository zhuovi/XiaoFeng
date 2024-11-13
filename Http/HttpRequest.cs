using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Collections;
using XiaoFeng.IO;
using XiaoFeng.Net;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-25 18:36:04                            *
*  Update Time : 2022-12-26 11:41:25                            *
*  Version : v 3.0.1                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public class HttpRequest : HttpBase, IHttpRequest
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public HttpRequest() { base.HttpCore = HttpCore.HttpClient; }
        /// <summary>
        /// 设置请求对象
        /// </summary>
        /// <param name="httpClient">请求对象</param>
        public HttpRequest(HttpClient httpClient) : this(httpClient, null) { }
        /// <summary>
        /// 设置请求对象
        /// </summary>
        /// <param name="source">信号源</param>
        public HttpRequest(CancellationTokenSource source) : this(null, source) { }
        /// <summary>
        /// 设置请求对象
        /// </summary>
        /// <param name="httpClient">请求对象</param>
        /// <param name="source">信号源</param>
        public HttpRequest(HttpClient httpClient, CancellationTokenSource source)
        {
            if (httpClient != null)
                this.Client = httpClient;
            if (source != null)
                this.CancelToken = source;
            base.HttpCore = HttpCore.HttpClient;
        }
        /// <summary>
        /// 设置网址
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="method">请求类型</param>
        /// <param name="httpCore">请求内核</param>
        public HttpRequest(string url, HttpMethod method, HttpCore httpCore = HttpCore.HttpClient)
        {
            base.HttpCore = httpCore;
            this.Address = url;
            this.Method = method;
        }
        /// <summary>
        /// 设置网址
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="httpCore">请求内核</param>
        public HttpRequest(string url, HttpCore httpCore = HttpCore.HttpClient)
        {
            base.HttpCore = httpCore;
            this.Address = url;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 取消状态
        /// </summary>
        private CancellationTokenSource CancelToken { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// 请求对象
        /// </summary>
        private HttpClient Client { get; set; }
        /// <summary>
        /// 消息处理程序
        /// </summary>
        private HttpClientHandler ClientHandler { get; set; }
        /// <summary>
        /// 操作是在响应可利用时立即视为已完成，还是在读取包含上下文的整个答案信息之后才视为已完成。
        /// </summary>
        public HttpCompletionOption CompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;
        /// <summary>
        /// Host的标头信息
        /// </summary>
        private string _Host = string.Empty;
        /// <summary>
        /// 设置Host的标头信息
        /// </summary>
        public string Host
        {
            get
            {
                if (_Host.IsNullOrEmpty())
                {
                    if (this.Address.IsNullOrEmpty()|| !this.Address.IsSite()) return string.Empty;
                    var uri = new Uri(this.Address);
                    return uri.Authority;
                }
                return this._Host;
            }
            set => _Host = value;
        }
        /// <summary>
        /// 获取或设置与此响应关联的 Cookie
        /// </summary>
        public string Cookies
        {
            set
            {
                if (value.IsQuery())
                {
                    var param = new ParameterCollection(value);
                    param.AllKeys.Each(k =>
                    {
                        this.AddCookie(k, param[k]);

                    });
                }
            }
        }
        /// <summary>
        /// 请求网址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 请求内容
        /// </summary>
        public HttpContent HttpContent { get; set; }
        /// <summary>
        /// 请求网址编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 获取或设置 User-agent HTTP 标头的值。
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.91 Safari/537.36";
        /// <summary>
        /// 请求超时时间 单位为毫秒
        /// </summary>
        public int Timeout { get; set; } = 10_000;
        /// <summary>
        /// 默认写入Post数据超时时间 单位为毫秒
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 10_000;
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept { get; set; } = "text/html, application/xhtml+xml, */*";
        /// <summary>
        /// Accept-Charset 标头，指定响应可接受的内容编码。
        /// </summary>
        public string AcceptEncoding { get; set; }
        /// <summary>
        /// Accept-Langauge 标头，指定用于响应的首选自然语言。
        /// </summary>
        public string AcceptLanguage { get; set; }
        /// <summary>
        /// Cache-Control 标头，指定请求/响应链上所有缓存控制机制必须服从的指令。
        /// </summary>
        public CacheControlHeaderValue CacheControl { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示请求是否应跟随重定向响应。
        /// </summary>
        public Boolean AllowAutoRedirect { get; set; } = true;
        /// <summary>
        /// 获取或设置请求将跟随的重定向的最大数目。
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; } = 5;
        /// <summary>
        /// 获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接。
        /// </summary>
        public Boolean KeepAlive { get; set; } = true;
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClientCertificates { get; set; }
        /// <summary>
        /// 获取和设置IfModifiedSince，默认为当前日期和时间
        /// </summary>
        public DateTime? IfModifiedSince { get; set; }
        /// <summary>
        /// 设置本地的出口ip和端口
        /// </summary>
        public IPEndPoint IPEndPoint { get; set; }
        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        public ICredentials Credentials { get; set; }
        /// <summary>
        /// 指定 Schannel 安全包支持的安全协议
        /// </summary>
        public SecurityProtocolType ProtocolType { get; set; } = SecurityProtocolType.Tls12;
        /// <summary>
        /// 获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion { get; set; } = System.Net.HttpVersion.Version11;
        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue { get; set; }
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CertPath { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPassWord { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit { get; set; } = 1024;
        /// <summary>
        /// Http 代理
        /// </summary>
        public WebProxy WebProxy { get; set; }
        /// <summary>
        /// 获取或设置 Referer HTTP 标头的值。
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<string, string> Data { get; set; }
        /// <summary>
        /// Body请求数据
        /// </summary>
        public string BodyData { get; set; }
        /// <summary>
        /// Body请求字节数据
        /// </summary>
        public byte[] BodyBytes { get; set; }
        /// <summary>
        /// FormData数据
        /// </summary>
        public List<FormData> FormData { get; set; }
        /// <summary>
        /// 请求完后是否重置请求对象以及响应对象
        /// </summary>
        public Boolean IsReset { get; set; }
        /// <summary>
        /// 请求消息
        /// </summary>
        public HttpRequestMessage Request { get; private set; }
        /// <summary>
        /// 压缩方式
        /// </summary>
        public DecompressionMethods DecompressionMethod { get; set; } = DecompressionMethods.None;
        /// <summary>
        /// 请求
        /// </summary>
        public HttpWebRequest RequestHttp { get; private set; }
        #endregion

        #region 方法

        #region 获取响应数据 HttpClient
        /// <summary>
        /// 获取响应数据
        /// </summary>
        /// <returns>响应数据</returns>
        public async Task<HttpResponse> GetResponseAsync()
        {
            if (this.Address.IsNullOrEmpty() || !this.Address.IsSite()) return null;
            if (this.HttpCore == HttpCore.HttpWebRequest)
                return await this.GetHttpResponseAsync().ConfigureAwait(false);
            else if (this.HttpCore == HttpCore.HttpSocket)
                return await this.GetHttpSocketResponseAsync().ConfigureAwait(false);
            var Response = new HttpResponse();
            /*回收*/
            GC.Collect();
            /*注册编码支持GB2312*/
#if NETCORE
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            if (this.FormData != null) this.Method = HttpMethod.Post;
            if (this.Method.Method.ToUpper() == "GET" && this.Data.IsNotNullOrEmpty())
                this.Address += (this.Address.IndexOf("?") == -1 ? "?" : "&") + this.Data.ToQuery();
            /*初始化对像，并设置请求的URL地址*/
            this.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(this.Address)
            };

            if (",POST,DELETE,PUT,".IndexOf("," + this.Method.Method.ToUpper() + ",", StringComparison.OrdinalIgnoreCase) > -1 && this.HttpContent == null)
            {
                if (this.FormData == null)
                {
                    if (this.Data != null && this.Data.Any())
                    {
                        HttpContent = new FormUrlEncodedContent(this.Data);
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/x-www-form-urlencoded";
                        else if (this.ContentType.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            HttpContent = new StringContent(this.Data.ToJson(), this.Encoding);
                        }
                    }
                    else if (this.BodyData.IsNotNullOrEmpty())
                    {
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/json";
                        HttpContent = new StringContent(this.BodyData, this.Encoding);
                    }
                    else if(this.BodyBytes!=null && this.BodyBytes.Length > 0)
                    {
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/octet-stream";
                        HttpContent = new ByteArrayContent(this.BodyBytes);
                    }
                    if (this.ContentType.IsNotNullOrEmpty())
                        HttpContent.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
                }
                else
                {
                    var formData = new MultipartFormDataContent();
                    if (this.Data != null && this.Data.Any())
                    {
                        this.Data.Each(kv =>
                        {
                            formData.Add(new StringContent(kv.Value), kv.Key);
                        });
                    }
                    this.FormData.Each(f =>
                    {
                        if (f.FormType == FormType.Text)
                            formData.Add(new StringContent(f.Value), f.Name);
                        else
                        {
                            byte[] fileContent = Array.Empty<byte>();
                            var fileName = string.Empty;
                            if (f.Data != null)
                            {
                                fileContent = f.Data;
                            }
                            else if (f.Value.IsBasePath())
                            {
                                fileContent = FileHelper.OpenBytes(f.Value);
                                fileName = f.Value.GetFileName();
                            }
                            else
                                fileContent = f.Value.GetBytes();
                            var file = new ByteArrayContent(fileContent);
                            file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            if(fileName.IsNullOrEmpty())
                                formData.Add(file, f.Name);
                            else formData.Add(file,f.Name, fileName);
                        }
                    });
                    HttpContent = formData;
                }
            }
            this.Request.Method = new System.Net.Http.HttpMethod(this.Method.Method);
            if (HttpContent != null)
            {
                this.Request.Content = HttpContent;
            }
            if (this.ClientHandler == null) this.ClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            else
            {
                this.ClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                this.ClientHandler.UseCookies = true;
            }
            /*设置证书*/
            this.SetCerList();
            /*设置HTTP代理*/
            if (this.WebProxy != null)
            {
                this.ClientHandler.UseProxy = true;
                this.ClientHandler.Proxy = this.WebProxy;
            }
            /*设置Http版本*/
            if (this.ProtocolVersion != null) this.Request.Version = this.ProtocolVersion;

            this.Request.Headers.ExpectContinue = this.Expect100Continue;
            this.ClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;

            if (this.Host.IsNotNullOrWhiteSpace()) this.Request.Headers.Host = this.Host;
            if (this.IfModifiedSince != null) this.Request.Headers.IfModifiedSince = this.IfModifiedSince.Value;

            /*Accept*/
            if (this.Accept.IsNotNullOrWhiteSpace())
                this.Request.Headers.Accept.ParseAdd(this.Accept);

            /*UserAgent客户端的访问类型，包括浏览器版本和操作系统信息*/
            this.Request.Headers.UserAgent.Clear();
            this.Request.Headers.UserAgent.ParseAdd(this.UserAgent);
            /*编码*/
            if (this.AcceptEncoding.IsNotNullOrWhiteSpace())
            {
                this.Request.Headers.AcceptEncoding.Clear();
                this.Request.Headers.AcceptEncoding.ParseAdd(this.AcceptEncoding);
            }
            if (this.AcceptLanguage.IsNotNullOrWhiteSpace())
            {
                this.Request.Headers.AcceptLanguage.Clear();
                this.Request.Headers.AcceptLanguage.ParseAdd(this.AcceptLanguage);
            }
            if (this.CacheControl != null)
            {
                this.Request.Headers.CacheControl = this.CacheControl;
            }

            this.Request.Headers.AcceptCharset.ParseAdd(this.Encoding.WebName);
            /*设置安全凭证*/
            this.ClientHandler.Credentials = this.Credentials;
            /*设置Cookie*/
            if (this.CookieContainer != null)
            {
                this.ClientHandler.CookieContainer = this.CookieContainer;
            }
            /*来源地址*/
            if (this.Referer.IsNotNullOrEmpty() && this.Referer.IsSite())
                this.Request.Headers.Referrer = new Uri(this.Referer);
            /*是否执行跳转功能*/
            this.ClientHandler.AllowAutoRedirect = this.AllowAutoRedirect;
            if (this.MaximumAutomaticRedirections > 0) this.ClientHandler.MaxAutomaticRedirections = this.MaximumAutomaticRedirections;
            /*设置最大连接*/
#if !NETSTANDARD20
            if (this.Connectionlimit > 0) this.ClientHandler.MaxConnectionsPerServer = this.Connectionlimit;
#endif 
            /*压缩方式*/
            if (this.DecompressionMethod != DecompressionMethods.None && this.ClientHandler.SupportsAutomaticDecompression)
            {
                this.ClientHandler.AutomaticDecompression = this.DecompressionMethod;
            }

            if (this.Client == null)
            {
                this.Client = new HttpClient(this.ClientHandler, true);
            }
            this.Client.Timeout = TimeSpan.FromMilliseconds(this.Timeout);
            this.Request.Headers.Connection.Clear();
            this.Request.Headers.Connection.ParseAdd(this.KeepAlive ? "Keep-Alive" : "close");
            /*设置Header参数*/
            if (this.Authorization.IsNotNullOrEmpty())
            {
                if (this.Headers == null) this.Headers = new Dictionary<string, string>();
                this.Headers.Add("Authorization", this.Authorization);
            }
            if (this.Headers != null && this.Headers.Any())
                this.Headers.Each(kv =>
                {
                    this.Request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                });
            if (this.IPEndPoint != null)
            {
                var ServicePoint = ServicePointManager.FindServicePoint(this.Request.RequestUri);
                ServicePoint.BindIPEndPointDelegate = (sp, remote, retryCount) => this.IPEndPoint;
            }
            try
            {
                Response.Request = this;
                this.BeginTime = DateTime.Now;
                /*请求数据*/
                Response.Response = await this.Client.SendAsync(this.Request, CompletionOption, this.CancelToken.Token).ConfigureAwait(false);
                this.EndTime = DateTime.Now;
                Response.HttpCore = this.HttpCore;
                Response.SetBeginAndEndTime(this.BeginTime, this.EndTime);
                await Response.InitAsync().ConfigureAwait(false);
                return Response;
            }
            catch (HttpRequestException ex)
            {
                using (Response.Response = new HttpResponseMessage() { Content = new StringContent(ex.Message), RequestMessage = this.Request, StatusCode = HttpStatusCode.BadRequest, Version = this.ProtocolVersion, ReasonPhrase = ex.Message })
                {
                    this.Headers.Each(kv =>
                    {
                        Response.Response.Headers.Add(kv.Key, kv.Value);
                    });
                    await Response.InitAsync().ConfigureAwait(false);
                }
                return Response;
            }
            catch (TaskCanceledException ex)
            {
                using (Response.Response = new HttpResponseMessage() { Content = new StringContent(ex.Message), RequestMessage = this.Request, StatusCode = HttpStatusCode.RequestTimeout, Version = this.ProtocolVersion, ReasonPhrase = ex.Message })
                {
                    this.Headers.Each(kv =>
                    {
                        Response.Response.Headers.Add(kv.Key, kv.Value);
                    });
                    await Response.InitAsync().ConfigureAwait(false);
                }
                return Response;
            }
            catch (Exception ex)
            {
                using (Response.Response = new HttpResponseMessage() { Content = new StringContent(ex.Message), RequestMessage = this.Request, StatusCode = HttpStatusCode.InternalServerError, Version = this.ProtocolVersion, ReasonPhrase = ex.Message })
                {
                    this.Headers.Each(kv =>
                    {
                        Response.Response.Headers.Add(kv.Key, kv.Value);
                    });
                    await Response.InitAsync().ConfigureAwait(false);
                }
                return Response;
            }
            finally
            {
                if (this.IsReset)
                {
                    /*释放http连接*/
                    this.Request.Dispose();
                    this.Request = null;
                    this.Client.Dispose();
                    this.Client = null;
                    this.ClientHandler.Dispose();
                    this.ClientHandler = null;
                    if (Response.Response != null)
                    {
                        try
                        {
                            Response.Response.Dispose();
                            Response.Response = null;
                        }
                        catch { }
                    }
                }
            }
        }
        /// <summary>
        /// 获取响应数据
        /// </summary>
        /// <returns>响应数据</returns>
        public HttpResponse GetResponse() => this.GetResponseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        #endregion

        #region 获取响应数据 HttpWebRequest
        /// <summary>
        /// 获取响应数据
        /// </summary>
        /// <returns>响应数据</returns>
        private async Task<HttpResponse> GetHttpResponseAsync()
        {
            var Response = new HttpResponse();
            /*回收*/
            GC.Collect();
            /*注册编码支持GB2312*/
#if NETCORE
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            if (this.Method.Method.ToUpper() == "GET" && this.Data.IsNotNullOrEmpty())
                this.Address += (this.Address.IndexOf("?") == -1 ? "?" : "&") + this.Data.ToQuery();
            //初始化对像，并设置请求的URL地址
            this.RequestHttp = WebRequest.CreateHttp(this.Address);
            /*设置证书*/
            this.SetCerList();
            /*设置本地的出口ip和端口*/
            if (this.IPEndPoint != null) this.RequestHttp.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(BindIPEndPointCallback);
            /*设置Header参数*/
            if (this.Headers == null) this.Headers = new Dictionary<string, string>();
            if (this.Authorization.IsNotNullOrEmpty()) this.Headers.Add("Authorization", this.Authorization);
            this.Headers.Each(k => this.RequestHttp.Headers.Add(k.Key, k.Value));
            /*设置HTTP代理*/
            if (this.WebProxy != null)
                this.RequestHttp.Proxy = this.WebProxy;
            /*设置Http版本*/
            if (this.ProtocolVersion != null) this.RequestHttp.ProtocolVersion = this.ProtocolVersion;

            byte[] RequestData = this.GetReuqestBody();

            this.RequestHttp.ServicePoint.Expect100Continue = this.Expect100Continue;
            this.RequestHttp.Method = this.Method.Method;
            this.RequestHttp.Timeout = this.Timeout;
            this.RequestHttp.KeepAlive = this.KeepAlive;
            this.RequestHttp.ReadWriteTimeout = this.ReadWriteTimeout;
            if (this.Host.IsNotNullOrWhiteSpace()) this.RequestHttp.Host = this.Host;
            if (this.IfModifiedSince != null) this.RequestHttp.IfModifiedSince = this.IfModifiedSince.Value;
            /*Accept*/
            if (this.Accept.IsNotNullOrWhiteSpace())
                this.RequestHttp.Accept = this.Accept;

            if (this.AcceptEncoding.IsNotNullOrWhiteSpace())
                this.RequestHttp.Headers.Add(HttpRequestHeader.AcceptEncoding, this.AcceptEncoding);

            if (this.AcceptLanguage.IsNotNullOrWhiteSpace())
                this.RequestHttp.Headers.Add(HttpRequestHeader.AcceptLanguage, this.AcceptLanguage);

            if (this.CacheControl != null)
                this.RequestHttp.Headers.Add(HttpRequestHeader.CacheControl, this.CacheControl.ToString());
            if (this.ContentType.IsNullOrEmpty())
                this.ContentType = "text/html";
            /*请求内容类型*/
            this.RequestHttp.ContentType = this.ContentType;
            /*UserAgent客户端的访问类型，包括浏览器版本和操作系统信息*/
            if (this.UserAgent.IsNotNullOrEmpty())
                this.RequestHttp.UserAgent = this.UserAgent;
            /*编码*/
            //encoding = Encoding.GetEncoding(this.Encoding);
            /*设置安全凭证*/
            if (this.Credentials != null)
                this.RequestHttp.Credentials = this.Credentials;
            /*设置Cookie*/
            if (this.CookieContainer != null) this.RequestHttp.CookieContainer = this.CookieContainer;
            /*来源地址*/
            if (this.Referer.IsNotNullOrEmpty())
                this.RequestHttp.Referer = this.Referer;
            /*是否执行跳转功能*/
            this.RequestHttp.AllowAutoRedirect = this.AllowAutoRedirect;
            if (this.MaximumAutomaticRedirections > 0) this.RequestHttp.MaximumAutomaticRedirections = this.MaximumAutomaticRedirections;
            /*设置最大连接*/
            if (this.Connectionlimit > 0) this.RequestHttp.ServicePoint.ConnectionLimit = this.Connectionlimit;

            /*设置请求数据*/
            if (RequestData != null && RequestData.Length > 0)
            {
                this.RequestHttp.ContentLength = RequestData.Length;
                this.RequestHttp.GetRequestStream().Write(RequestData, 0, RequestData.Length);
            }
            try
            {
                Response.Request = this;
                this.BeginTime = DateTime.Now;
                /*请求数据*/
                Response.ResponseHttp = await this.RequestHttp.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;
                this.EndTime = DateTime.Now;
                Response.HttpCore = this.HttpCore;
                Response.SetBeginAndEndTime(this.BeginTime, this.EndTime);
                await Response.InitHttpAsync().ConfigureAwait(false);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    using (Response.ResponseHttp = (HttpWebResponse)ex.Response) await Response.InitHttpAsync().ConfigureAwait(false);
                else
                {
                    Response.StatusCode = HttpStatusCode.BadRequest;
                    Response.StatusDescription = ex.Message;
                }
            }
            finally
            {
                if (this.IsReset)
                {
                    /*释放http连接*/
                    if (this.RequestHttp != null && RequestHttp.ServicePoint != null) this.ReleaseServicePoint(RequestHttp.ServicePoint);
                    this.Request = null;
                    if (Response.Response != null)
                    {
                        try
                        {
                            Response.ResponseHttp.Close();
                            Response.ResponseHttp.Dispose();
                            Response.Response = null;
                        }
                        catch { }
                    }
                }
            }
            return Response;
        }
        #endregion

        #region 获取响应数据 Socket
        /// <summary>
        /// 获取响应数据
        /// </summary>
        /// <returns>响应数据</returns>
        private async Task<HttpResponse> GetHttpSocketResponseAsync()
        {
            var httpSocket = new HttpSocket(this);
            return await httpSocket.SendRequestAsync().ConfigureAwait(false);
        }
        #endregion

        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件保存路径</param>
        /// <returns>运行时长</returns>
        public async Task<long> DownFileAsync(string filePath)
        {
            var response = await this.GetResponseAsync().ConfigureAwait(false);
            return await response.DownFileAsync(filePath);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件保存路径</param>
        /// <returns>运行时长</returns>
        public long DownFile(string filePath) => this.GetResponse().DownFile(filePath);
        #endregion

        #region 通过设置这个属性，可以在发出连接的时候绑定客户端发出连接所使用的IP地址。
        /// <summary>
        /// 通过设置这个属性，可以在发出连接的时候绑定客户端发出连接所使用的IP地址。 
        /// </summary>
        /// <param name="servicePoint"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        private IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            return this.IPEndPoint;
        }
        #endregion

        #region 释放证书
        /// <summary>
        /// 释放证书
        /// </summary>
        /// <param name="servicePoint">Http连接管理</param>
        private void ReleaseServicePoint(ServicePoint servicePoint)
        {
            return;
            /*var m = typeof(ServicePointManager);
            var f = m.GetMethod("IdleServicePointTimeoutCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            f.Invoke(null, new object[] { null, 0, servicePoint });*/
        }
        #endregion

        #region 获取请求Body
        /// <summary>
        /// 获取请求Body
        /// </summary>
        /// <returns>请求Body</returns>
        public byte[] GetReuqestBody()
        {
            byte[] RequestData = Array.Empty<byte>();
            if (",POST,DELETE,PUT,".IndexOf("," + this.Method.Method.ToUpper() + ",", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (this.FormData == null)
                {
                    if (this.Data != null && this.Data.Any())
                    {
                        if (this.Method == "POST")
                        {
                            if (this.ContentType.IsNullOrEmpty())
                            {
                                this.ContentType = "application/x-www-form-urlencoded";
                                RequestData = this.Data.ToQuery().GetBytes(this.Encoding);
                            }
                            else if (this.ContentType.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                RequestData = this.Data.ToJson().GetBytes(this.Encoding);
                            }
                        }
                    }
                    else if (this.BodyData.IsNotNullOrEmpty())
                    {
                        this.Method = HttpMethod.Post;
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/json";
                        RequestData = this.BodyData.GetBytes(this.Encoding);
                    }
                    else if (this.BodyBytes != null && this.BodyBytes.Length > 0)
                    {
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/octet-stream";
                        RequestData = this.BodyBytes;
                    }
                    if (this.ContentType.IsNotNullOrEmpty())
                    {
                        if (this.RequestHttp != null)
                            this.RequestHttp.ContentType = this.ContentType;
                    }
                }
                else
                {
                    this.Method = HttpMethod.Post;
                    if (this.Data.IsNotNullOrEmpty())
                    {
                        this.Data.Each(kv =>
                        {
                            this.FormData.Add(new FormData(kv.Key, kv.Value, FormType.Text));
                        });
                    }
                    var boundary = this.GetBoundary();
                    this.ContentType = "multipart/form-data; boundary=" + boundary;
                    RequestData = this.GetFormDataBytes(boundary);
                }
                this.ContentLength = RequestData.Length;
            }
            return RequestData;
        }
        #endregion

        #region 获取FormData流
        /// <summary>
        /// 获取FormData流
        /// </summary>
        /// <param name="boundary">分界线</param>
        /// <returns>FormData字节数组</returns>
        private byte[] GetFormDataBytes(string boundary)
        {
            if (this.FormData == null || this.FormData.Count == 0) return Array.Empty<byte>();
            using (var ms = new MemoryStream())
            {
                this.FormData.Each(f =>
                {
                    if (f.FormType == FormType.Text)
                    {
                        var bytes = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{f.Name}\"\r\n\r\n{f.Value}\r\n".GetBytes();
                        ms.Write(bytes, 0, bytes.Length);
                    }
                    else if (f.FormType == FormType.File)
                    {
                        byte[] fileContent = Array.Empty<byte>();
                        var fileName = string.Empty;
                        if (f.Data != null)
                        {
                            fileContent = f.Data;
                        }
                        else if (f.Value.IsBasePath())
                        {
                            fileContent = FileHelper.OpenBytes(f.Value);
                            fileName = f.Value.GetFileName();
                        }
                        else
                            fileContent = f.Value.GetBytes();
                        var bytes = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{f.Name}\"; filename=\"{fileName}\"\r\nContent-Type: application/octet-stream\r\n\r\n".GetBytes();
                        ms.Write(bytes, 0, bytes.Length);
                        ms.Write(fileContent, 0, fileContent.Length);
                        bytes = "\r\n".GetBytes();
                        ms.Write(bytes, 0, bytes.Length);
                    }
                });
                var footData = $"\r\n--{boundary}--\r\n".GetBytes();
                ms.Write(footData, 0, footData.Length);
                return ms.ToArray();
            }
        }
        #endregion

        #region 分界线
        /// <summary>
        /// 分界线
        /// </summary>
        public string GetBoundary()
        {
            var Boundary =
#if NETCORE
            new string('-', 4) + "FayElfWebFormBoundary"
#else
            new string('-', 15)
#endif
            + DateTime.Now.Ticks.ToString("x");
            return Boundary;
        }
        #endregion

        #region 取消请求
        /// <summary>
        /// 取消请求
        /// </summary>
        public void Abort()
        {
            if (this.HttpCore == HttpCore.HttpClient)
                this.CancelToken.Cancel();
            else
                this.RequestHttp?.Abort();
        }
        #endregion

        #region 设置多个证书
        /// <summary>
        /// 设置多个证书
        /// </summary>
        private void SetCerList()
        {
            var ClientCerts = this.HttpCore == HttpCore.HttpClient ? this.ClientHandler.ClientCertificates : this.RequestHttp.ClientCertificates;
            if (this.ClientCertificates != null && this.ClientCertificates.Count > 0)
            {
                foreach (X509Certificate2 c in this.ClientCertificates)
                    ClientCerts.Add(c);
            }
            if (this.Address.IsMatch(@"^https://"))
            {
                /*设置最大连接数*/
                ServicePointManager.DefaultConnectionLimit = this.Connectionlimit;
                if (this.CertPath.IsNotNullOrWhiteSpace())
                {
                    /*这一句一定要写在创建连接的前面,使用回调的方法进行证书验证。*/
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    /*将证书添加到请求里*/
                    var cert = this.CertPath.GetBasePath();
                    if (File.Exists(cert))
                    {
                        ClientCerts.Add(this.CertPassWord.IsNullOrEmpty() ? new X509Certificate2(cert) : new X509Certificate2(cert, this.CertPassWord));
                    }
                }
                else
                {
                    ServicePointManager.SecurityProtocol = this.ProtocolType;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }
            }
        }
        #endregion

        #region 添加cookie
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddCookie(string name, string value) => this.AddCookie(new Cookie(name, value));
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期间隔</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddCookie(string name, string value, TimeSpan? timeSpan)
        {
            var cookie = new Cookie(name, value);
            if (timeSpan != null) cookie.Expires = DateTime.Now.AddMilliseconds(timeSpan.Value.TotalMilliseconds);
            return this.AddCookie(cookie);
        }
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="path">路径</param>
        /// <param name="domain">域名</param>
        /// <returns></returns>
        public IHttpRequest AddCookie(string name, string value, string path, string domain) => this.AddCookie(new Cookie(name, value, path, domain));
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public IHttpRequest AddCookie(string name, string value, string path) => this.AddCookie(new Cookie(name, value, path));
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddCookie(Cookie cookie)
        {
            if (cookie.Domain.IsNullOrEmpty() && this.Address.IsNotNullOrEmpty())
                cookie.Domain = new Uri(this.Address).Host;
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            this.CookieContainer.Add(cookie);
            return this;
        }
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="collection">cookie集</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddCookie(CookieCollection collection)
        {
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            this.CookieContainer.Add(collection);
            return this;
        }
        #endregion

        #region 添加参数
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddParameter(string key, string value)
        {
            if (this.Data == null) this.Data = new Dictionary<string, string>();
            if (this.Data.ContainsKey(key))
                this.Data[key] = value;
            else
                this.Data.Add(key, value);
            return this;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddParameter(Dictionary<string, string> data)
        {
            if (this.Data == null) { this.Data = data; return this; }
            data.Each(d =>
            {
                this.AddParameter(d.Key, d.Value);
            });
            return this;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="model">值</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddParameter<T>(T model) where T : new()
        {
            if (model == null) return this;
            if (this.Data == null) this.Data = new Dictionary<string, string>();
            model.GetType().GetPropertiesAndFields().Each(m =>
            {
                if (m is PropertyInfo p)
                {
                    this.Data.Add(m.Name, p.GetValue(model).getValue());
                }
                else
                {
                    var f = m as FieldInfo;
                    this.Data.Add(m.Name, f.GetValue(model).getValue());
                }
            });
            return this;
        }
        #endregion

        #region 添加formdata
        /// <summary>
        /// 添加formdata
        /// </summary>
        /// <param name="formData">formdata</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddFormData(FormData formData)
        {
            if (this.FormData == null) this.FormData = new List<FormData>();
            var index = this.FormData.FindIndex(a => a.Name == formData.Name);
            if (index == -1)
                this.FormData.Add(formData);
            else
                this.FormData[index] = formData;
            return this;
        }
        /// <summary>
        /// 添加formdata
        /// </summary>
        /// <param name="key">名称</param>
        /// <param name="value">值</param>
        /// <param name="formType">类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddFormData(string key, string value, FormType formType = FormType.Text)
        {
            return this.AddFormData(new Http.FormData(key, value, formType));
        }
        /// <summary>
        /// 添加formdata
        /// </summary>
        /// <param name="forms">formdata集合</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddFormData(List<FormData> forms)
        {
            forms.Each(f =>
            {
                var index = this.FormData.FindIndex(a => a.Name == f.Name);
                if (index == -1)
                    this.FormData.Add(f);
                else
                    this.FormData[index] = f;
            });
            return this;
        }
        #endregion

        #region 设置请求类型
        /// <summary>
        /// 设置请求类型
        /// </summary>
        /// <param name="method">请求类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetMethod(HttpMethod method)
        {
            this.Method = method;
            return this;
        }
        /// <summary>
        /// 设置请求类型
        /// </summary>
        /// <param name="method">请求类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetMethod(System.Net.Http.HttpMethod method)
        {
            this.Method = method;
            return this;
        }
        #endregion

        #region 设置编码
        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetEncoding(Encoding encoding)
        {
            this.Encoding = encoding;
            return this;
        }
        #endregion

        #region 设置请求内容
        /// <summary>
        /// 设置请求内容
        /// </summary>
        /// <param name="httpContent">请求内容</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetHttpContent(HttpContent httpContent)
        {
            this.HttpContent = httpContent;
            return this;
        }
        #endregion

        #region 设置内容类型
        /// <summary>
        /// 设置内容类型
        /// </summary>
        /// <param name="contentType">内容类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetContentType(string contentType)
        {
            this.ContentType = contentType;
            return this;
        }
        #endregion

        #region 回调验证证书问题
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns></returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        #endregion

        #region 设置超时
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="timeout">超时时间 单位为毫秒</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetTimeout(int timeout)
        {
            this.Timeout = timeout;
            return this;
        }
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetTimeout(TimeSpan timeSpan)
        {
            this.Timeout = (int)timeSpan.TotalMilliseconds;
            return this;
        }
        #endregion

        #region 设置代理
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxy">代理</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetWebProxy(WebProxy proxy)
        {
            this.WebProxy = proxy;
            return this;
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetWebProxy(string host, int port) => this.SetWebProxy(new WebProxy(host, port));
        #endregion

        #region 设置请求将跟随的重定向的最大数目
        /// <summary>
        /// 设置请求将跟随的重定向的最大数目
        /// </summary>
        /// <param name="count">数目</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetMaximumAutomaticRedirections(int count)
        {
            if (count <= 0)
                this.AllowAutoRedirect = false;
            else
            {
                this.AllowAutoRedirect = true;
                this.MaximumAutomaticRedirections = count;
            }
            return this;
        }
        #endregion

        #region 设置请求标头
        /// <summary>
        /// 设置请求标头
        /// </summary>
        /// <param name="accept">请求标头</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetAccept(string accept)
        {
            this.Accept = accept;
            return this;
        }
        #endregion

        #region 设置地址
        /// <summary>
        /// 设置地址
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetAddress(string url)
        {
            this.Address = url;
            return this;
        }
        /// <summary>
        /// 设置地址
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetAddress(string url, HttpMethod method)
        {
            this.Address = url;
            this.Method = method;
            return this;
        }
        #endregion

        #region 设置Body数据
        /// <summary>
        /// 设置Body数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="contentType">内容类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetBodyData(string data, string contentType = "application/json")
        {
            this.Method = HttpMethod.Post;
            this.ContentType = contentType;
            this.BodyData = data;
            return this;
        }
        /// <summary>
        /// 设置Body数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="model">数据</param>
        /// <param name="contentType">内容类型</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetBodyData<T>(T model, string contentType = "application/json") where T : new()
        {
            if (model == null) return this;
            return this.SetBodyData(model.ToJson(), contentType);
        }
        #endregion

        #region 设置 User-agent HTTP 标头的值
        /// <summary>
        /// 设置 User-agent HTTP 标头的值
        /// </summary>
        /// <param name="userAgent">User-agent HTTP 标头的值</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetUserAgent(string userAgent)
        {
            this.UserAgent = userAgent;
            return this;
        }
        #endregion

        #region 设置请求的身份验证信息
        /// <summary>
        /// 设置请求的身份验证信息
        /// </summary>
        /// <param name="credentials">请求的身份验证信息</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetCredentials(ICredentials credentials)
        {
            this.Credentials = credentials;
            return this;
        }
        #endregion

        #region 设置 Referer HTTP 标头的值
        /// <summary>
        /// 设置 Referer HTTP 标头的值
        /// </summary>
        /// <param name="referer">Referer HTTP 标头的值</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetReferer(string referer)
        {
            this.Referer = referer;
            return this;
        }
        #endregion

        #region 设置请求证书
        /// <summary>
        /// 设置请求证书
        /// </summary>
        /// <param name="path">证书路径</param>
        /// <param name="password">证书密码</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetCert(string path, string password)
        {
            this.CertPath = path;
            this.CertPassWord = password;
            return this;
        }
        /// <summary>
        /// 设置证书集
        /// </summary>
        /// <param name="certificates">证书集</param>
        /// <returns></returns>
        public IHttpRequest SetCertificate(X509CertificateCollection certificates)
        {
            this.ClientCertificates = certificates;
            return this;
        }
        #endregion

        #region 设置头信息
        /// <summary>
        /// 设置头信息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns>请求对象</returns>
        public IHttpRequest AddHeader(string key, string value)
        {
            if (key.IsNullOrEmpty()) return this;
            switch (key.Replace("-", "").ToLower())
            {
                case "acceptencoding":
                    this.AcceptEncoding = value;
                    return this;
                case "acceptlanguage":
                    this.AcceptLanguage = value;
                    return this;
            }
            if (this.Headers == null) this.Headers = new Dictionary<string, string>();
            if (this.Headers.ContainsKey(key))
                this.Headers[key] = value;
            else
                this.Headers.Add(key, value);
            return this;
        }
        /// <summary>
        /// 设置头信息
        /// </summary>
        /// <param name="vals">集合</param>
        /// <returns></returns>
        public IHttpRequest AddHeader(Dictionary<string, string> vals)
        {
            if (vals == null || vals.Count == 0) return this;
            if (this.Headers == null) this.Headers = new Dictionary<string, string>();
            vals.Each(k =>
            {
                this.AddHeader(k.Key, k.Value);
            });
            return this;
        }
        #endregion

        #region 设置请求内核
        /// <summary>
        /// 设置请求内核
        /// </summary>
        /// <param name="httpCore">请求内核</param>
        /// <returns>请求对象</returns>
        public IHttpRequest SetHttpCore(HttpCore httpCore)
        {
            this.HttpCore = httpCore;
            return this;
        }
        #endregion

        #region 设置 Accept-Charset 标头
        /// <summary>
        /// 设置 Accept-Charset 标头，指定响应可接受的内容编码。
        /// </summary>
        /// <param name="acceptEncoding">Accept-Charset 标头</param>
        /// <returns></returns>
        public IHttpRequest SetAcceptEncoding(string acceptEncoding)
        {
            if (acceptEncoding.IsNullOrEmpty()) return this;
            this.AcceptEncoding = acceptEncoding;
            return this;
        }
        #endregion

        #region 设置 Accept-Langauge 标头
        /// <summary>
        /// 设置 Accept-Langauge 标头，指定用于响应的首选自然语言。
        /// </summary>
        /// <param name="acceptLanguage">Accept-Charset 标头</param>
        /// <returns></returns>
        public IHttpRequest SetAcceptLanguage(string acceptLanguage)
        {
            if (acceptLanguage.IsNullOrEmpty()) return this;
            this.AcceptLanguage = acceptLanguage;
            return this;
        }
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
                if (this.Client != null)
                {
                    this.Client.Dispose();
                    this.Client = null;
                }
                if (this.ClientHandler != null)
                {
                    this.ClientHandler.Dispose();
                    this.ClientHandler = null;
                }
            });
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~HttpRequest()
        {
            this.Dispose(false);
        }
        #endregion

        #endregion
    }
}