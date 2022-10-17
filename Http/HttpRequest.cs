﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-25 18:36:04                            *
*  Version : v 2.0.0                                            *
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
        public HttpRequest() { this.HttpCore = HttpCore.HttpClient; }
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
            this.HttpCore = HttpCore.HttpClient;
        }
        /// <summary>
        /// 设置网址
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="method">请求类型</param>
        /// <param name="httpCore">请求内核</param>
        public HttpRequest(string url, HttpMethod method, HttpCore httpCore = HttpCore.HttpClient)
        {
            this.HttpCore = httpCore;
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
            this.HttpCore = httpCore;
            this.Address = url;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 请求内核
        /// </summary>
        public HttpCore HttpCore { get; set; }
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
        /// 设置Host的标头信息
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 获取或设置与此响应关联的 Cookie
        /// </summary>
        public string Cookies
        {
            set
            {
                if (value.IsQuery())
                {
                    value.GetQuerys().Each(q =>
                    {
                        this.AddCookie(q.Key, q.Value);
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
        public X509Certificate2Collection ClentCertificates { get; set; }
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
        public Dictionary<string, string> Data { get; set; }
        /// <summary>
        /// Body请求数据
        /// </summary>
        public string BodyData { get; set; }
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
        #endregion

        #region 方法

        #region 获取响应数据
        /// <summary>
        /// 获取响应数据
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse> GetResponseAsync()
        {
            if (this.Address.IsNullOrEmpty() || !this.Address.IsSite()) return null;
            var Response = new HttpResponse();
            /*回收*/
            GC.Collect();
            /*注册编码支持GB2312*/
#if NETCORE
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            if (this.FormData != null) this.Method = HttpMethod.Post;
            if (this.Method.Method.ToUpper() == "GET" && this.Data.IsNotNullOrEmpty())
                this.Address += (this.Address.IndexOf("?") == -1 ? "?" : "&") + this.Data;
            /*初始化对像，并设置请求的URL地址*/
            this.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(this.Address)
            };
            /*设置证书*/
            this.SetCerList();
            if (",POST,GET,DELETE,PUT,".IndexOf("," + this.Method.Method.ToUpper() + ",", StringComparison.OrdinalIgnoreCase) > -1 && this.HttpContent == null)
            {
                if (this.FormData == null)
                {
                    if (this.Data != null && this.Data.Any())
                    {
                        HttpContent = new FormUrlEncodedContent(this.Data);
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/x-www-form-urlencoded";
                    }
                    else if (this.BodyData.IsNotNullOrEmpty())
                    {
                        if (this.ContentType.IsNullOrEmpty())
                            this.ContentType = "application/json";
                        HttpContent = new StringContent(this.BodyData, this.Encoding);
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
                            var file = new ByteArrayContent(FileHelper.OpenBytes(f.Value));
                            file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            formData.Add(file, f.Name, f.Value.GetFileName());
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
                ClientCertificateOptions = ClientCertificateOption.Automatic
            };
            else
            {
                this.ClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                this.ClientHandler.UseCookies = true;
            }
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
            else this.Request.Headers.Host = this.Request.RequestUri.Host;
            if (this.IfModifiedSince != null) this.Request.Headers.IfModifiedSince = this.IfModifiedSince.Value;

            /*Accept*/
            if (this.Accept.IsNotNullOrWhiteSpace())
                this.Request.Headers.Accept.ParseAdd(this.Accept);

            /*UserAgent客户端的访问类型，包括浏览器版本和操作系统信息*/
            this.Request.Headers.UserAgent.Clear();
            this.Request.Headers.UserAgent.ParseAdd(this.UserAgent);
            /*编码*/
            this.Request.Headers.AcceptEncoding.Clear();
            this.Request.Headers.AcceptEncoding.ParseAdd(this.Encoding.WebName);
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
            if (this.Headers != null && this.Headers.Any())
                this.Headers.Each(kv =>
                {
                    this.Request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                });
            try
            {
                this.BeginTime = DateTime.Now;
                /*请求数据*/
                Response.Response = await this.Client.SendAsync(this.Request, CompletionOption, this.CancelToken.Token).ConfigureAwait(false);
                this.EndTime = DateTime.Now;
                await Response.InitAsync();
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
                    await Response.InitAsync();
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
                    await Response.InitAsync();
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
                    await Response.InitAsync();
                }
                return Response;
            }
            finally
            {
                if (this.IsReset)
                {
                    /*释放http连接*/
                    // if (this.Request != null && Request.ServicePoint != null) this.ReleaseServicePoint(Request.ServicePoint);
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
        /// <returns></returns>
        public HttpResponse GetResponse() => this.GetResponseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        #endregion

        #region 取消请求
        /// <summary>
        /// 取消请求
        /// </summary>
        public void Abort()
        {
            this.CancelToken.Cancel();
        }
        #endregion

        #region 设置多个证书
        /// <summary>
        /// 设置多个证书
        /// </summary>
        private void SetCerList()
        {
            if (this.ClentCertificates != null && this.ClentCertificates.Count > 0)
            {
                foreach (X509Certificate2 c in this.ClentCertificates)
                    this.ClientHandler.ClientCertificates.Add(c);
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
                    this.ClientHandler.ClientCertificates.Add(this.CertPassWord.IsNullOrEmpty() ? new X509Certificate2(cert) : new X509Certificate2(cert, this.CertPassWord));
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
        /// <returns></returns>
        public IHttpRequest AddCookie(string name, string value) => this.AddCookie(new Cookie(name, value));
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public IHttpRequest AddCookie(Cookie cookie)
        {
            if (cookie.Domain.IsNullOrEmpty() && this.Address.IsNotNullOrEmpty())
                cookie.Domain = new Uri(this.Address).Host.RemovePattern(@":\d+$");
            if (this.CookieContainer == null) this.CookieContainer = new CookieContainer();
            this.CookieContainer.Add(cookie);
            return this;
        }
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="collection">cookie集</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
        public IHttpRequest AddParameter(Dictionary<string, string> data)
        {
            if (this.Data == null) { this.Data = data; return this; }
            data.Each(d =>
            {
                if (this.Data.ContainsKey(d.Key))
                    this.Data[d.Key] = d.Value;
                else
                    this.Data.Add(d.Key, d.Value);
            });
            return this;
        }
        #endregion

        #region 添加formdata
        /// <summary>
        /// 添加formdata
        /// </summary>
        /// <param name="formData">formdata</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IHttpRequest AddFormData(string key, string value, FormType formType)
        {
            return this.AddFormData(new Http.FormData(key, value, formType));
        }
        /// <summary>
        /// 添加formdata
        /// </summary>
        /// <param name="forms">formdata集合</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IHttpRequest SetMethod(HttpMethod method)
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns>bool</returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        #endregion

        #region 设置超时
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="timeout">超时时间 单位为毫秒</param>
        /// <returns></returns>
        public IHttpRequest SetTimeout(int timeout)
        {
            this.Timeout = timeout;
            return this;
        }
        #endregion

        #region 设置代理
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxy">代理</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IHttpRequest SetWebProxy(string host, int port) => this.SetWebProxy(new WebProxy(host, port));
        #endregion

        #region 设置请求将跟随的重定向的最大数目
        /// <summary>
        /// 设置请求将跟随的重定向的最大数目
        /// </summary>
        /// <param name="count">数目</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
        public IHttpRequest SetBodyData(string data, string contentType = "application/json")
        {
            this.Method = HttpMethod.Post;
            this.ContentType = contentType;
            this.BodyData = data;
            return this;
        }
        #endregion

        #region 设置 User-agent HTTP 标头的值
        /// <summary>
        /// 设置 User-agent HTTP 标头的值
        /// </summary>
        /// <param name="userAgent">User-agent HTTP 标头的值</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
        public IHttpRequest SetCert(string path, string password)
        {
            this.CertPath = path;
            this.CertPassWord = password;
            return this;
        }
        #endregion

        #endregion
    }
}