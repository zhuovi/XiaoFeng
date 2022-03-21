using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel;

namespace XiaoFeng.Net
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
    ===================================================================
    */
    /// <summary>
    /// 请求数据 WebClient
    /// Verstion : 1.0.0
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2018/04/10 09:34:53
    /// Update Time : 2018/04/10 09:34:53
    /// </summary>
    public class WebClientHelper : WebClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public WebClientHelper()
        {
            this.CookieContainer = new CookieContainer();
            this.Method = "GET";
            this.AllowAutoRedirect = true;
            this.MaximumAutomaticRedirections = 5;
            this.Encoding = Encoding.UTF8;
        }
        #endregion

        #region 属性
        /// <summary>
        /// Cookie 容器
        /// </summary>
        public CookieContainer CookieContainer { get; set; }
        /// <summary>
        /// 获取或设置 User-agent HTTP 标头的值。
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 获取或设置请求的方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 请求内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 超时时间 毫秒为单位
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept { get; set; }
        /// <summary>
        /// 是否应跟随重定向响应
        /// </summary>
        public Boolean AllowAutoRedirect { get; set; }
        /// <summary>
        /// 获取或设置请求将跟随的重定向的最大数目。
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }
        /// <summary>
        /// 获取或设置 Referer HTTP 标头的值。
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// Post请求数据
        /// </summary>
        public string PostData { get; set; }
        /// <summary>
        /// Post请求字节
        /// </summary>
        public byte[] PostDataByte { get; set; }
        /// <summary>
        /// 请求路径
        /// </summary>
        public Uri Address { get; set; }
        /// <summary>
        /// 本地地址
        /// </summary>
        public string LocalAddress { get; set; }
        #endregion

        #region 方法

        #region 返回带有 Cookie 的 HttpWebRequest
        /// <summary>
        /// 返回带有 Cookie 的 HttpWebRequest
        /// </summary>
        /// <param name="address">一个 System.Uri ，它标识要请求的资源</param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            this.Address = address;
            if (request is HttpWebRequest)
            {
                HttpWebRequest httpRequest = request as HttpWebRequest;
                if (this.CookieContainer != null) httpRequest.CookieContainer = CookieContainer;
                if (!this.UserAgent.IsNullOrEmpty()) httpRequest.UserAgent = UserAgent;
                if (this.Method.ToUpper() == "POST")
                {
                    httpRequest.Method = this.Method;
                    httpRequest.ContentType = "application/x-www-form-urlencoded";
                    byte[] pData = PostData.GetBytes();
                    httpRequest.ContentLength = pData.Length;
                    httpRequest.GetRequestStream().Write(pData, 0, pData.Length);
                }
                else
                {
                    if (this.ContentType.IsNullOrEmpty())
                        httpRequest.ContentType = "text/html";
                    else
                        httpRequest.ContentType = ContentType;
                }
                if (this.TimeOut > 0) httpRequest.Timeout = TimeOut;

                if (!this.Accept.IsNullOrEmpty()) httpRequest.Accept = Accept;
                else httpRequest.Accept = "text/html, application/xhtml+xml, */*";

                if (this.AllowAutoRedirect)
                {
                    httpRequest.AllowAutoRedirect = AllowAutoRedirect;
                    httpRequest.MaximumAutomaticRedirections = MaximumAutomaticRedirections;
                }
                if (!this.Referer.IsNullOrEmpty()) httpRequest.Referer = Referer;
            }
            return request;
        }
        #endregion

        #region 下载形式指定的资源 System.Uri
        /// <summary>
        /// 下载形式指定的资源 System.Uri。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">一个 System.Uri 包含要下载的 URI。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void DownloadStringAsync(Uri address, DownloadStringCompletedEventHandler callback, object userToken = null)
        {
            base.DownloadStringCompleted += callback;
            base.DownloadStringAsync(address, userToken);
        }
        #endregion

        #region 对本地文件，下载将具有指定的 URI 的资源
        /// <summary>
        /// 对本地文件，下载将具有指定的 URI 的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要下载的资源的 URI。</param>
        /// <param name="fileName">要放置在本地计算机上的文件的名称。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void DownloadFileAsync(Uri address, string fileName, AsyncCompletedEventHandler callback, object userToken = null)
        {
            this.LocalAddress = fileName;
            base.DownloadFileCompleted += callback;
            base.DownloadFileAsync(address, fileName, userToken);
        }
        #endregion

        #region 作为资源下载 System.Byte 数组，从异步操作的形式指定的 URI 数组
        /// <summary>
        /// 作为资源下载 System.Byte 数组，从异步操作的形式指定的 URI 数组。
        /// </summary>
        /// <param name="address">一个 System.Uri 包含要下载的 URI。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void DownloadDataAsync(Uri address, DownloadDataCompletedEventHandler callback,object userToken=null)
        {
            base.DownloadDataCompleted += callback;
            base.DownloadDataAsync(address,userToken);
        }
        #endregion

        #region 将数据缓冲区上载到通过使用 POST 方法的 URI 标识的资源
        /// <summary>
        /// 将数据缓冲区上载到通过使用 POST 方法的 URI 标识的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收的数据资源的 URI。</param>
        /// <param name="data">要向资源发送的数据缓冲区。</param>
        /// <param name="callback">回调方法</param>
        public void UploadDataAsync(Uri address,byte[] data, UploadDataCompletedEventHandler callback)
        {
            base.UploadDataCompleted += callback;
            base.UploadDataAsync(address, data);
        }
        /// <summary>
        /// 将数据缓冲区上载到通过使用 POST 方法的 URI 标识的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收的数据资源的 URI。</param>
        /// <param name="method">用来将数据发送到该资源的方法。 如果 null, ，默认情况下，开机自检 （http） 和 STOR ftp。</param>
        /// <param name="data">要向资源发送的数据缓冲区。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void UploadDataAsync(Uri address, string method, byte[] data, UploadDataCompletedEventHandler callback, object userToken = null)
        {
            base.UploadDataCompleted += callback;
            base.UploadDataAsync(address, method, data, userToken);
        }
        #endregion

        #region 将指定的本地文件上载到指定的资源使用 POST 方法
        /// <summary>
        /// 将指定的本地文件上载到指定的资源使用 POST 方法。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收的文件资源的 URI。 有关 HTTP 资源，此 URI 必须标识可以接受使用 POST 方法，如脚本或 ASP 页发送的请求的资源。</param>
        /// <param name="method">用来将数据发送到该资源的方法。 如果 null, ，默认情况下，开机自检 （http） 和 STOR ftp。</param>
        /// <param name="fileName">要发送到资源的文件。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void UploadFileAsyncs(Uri address, string method, string fileName, UploadFileCompletedEventHandler callback, object userToken = null)
        {
            base.UploadFileCompleted += callback;
            base.UploadFileAsync(address, method, fileName, userToken);
        }
        /// <summary>
        /// 将指定的本地文件上载到指定的资源使用 POST 方法。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收的文件资源的 URI。 有关 HTTP 资源，此 URI 必须标识可以接受使用 POST 方法，如脚本或 ASP 页发送的请求的资源。</param>
        /// <param name="fileName">要发送到资源的文件。</param>
        /// <param name="callback">回调方法</param>
        public void UploadFileAsyncs(Uri address, string fileName, UploadFileCompletedEventHandler callback)
        {
            base.UploadFileCompleted += callback;
            base.UploadFileAsync(address, fileName);
        }
        #endregion

        #region 将指定的字符串上载到指定的资源
        /// <summary>
        /// 将指定的字符串上载到指定的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收字符串资源的 URI。 有关 HTTP 资源，此 URI 必须标识可以接受使用 POST 方法，如脚本或 ASP 页发送的请求的资源。</param>
        /// <param name="data">要上载的字符串。</param>
        /// <param name="callback">回调方法</param>
        public void UploadStringAsync(Uri address,string data, UploadStringCompletedEventHandler callback)
        {
            base.UploadStringCompleted += callback;
            base.UploadStringAsync(address, data);
        }
        /// <summary>
        /// 将指定的字符串上载到指定的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收字符串资源的 URI。 有关 HTTP 资源，此 URI 必须标识可以接受使用 POST 方法，如脚本或 ASP 页发送的请求的资源。</param>
        /// <param name="method">用于将文件发送到该资源的 HTTP 方法。 如果为 null，默认值为 POST （http） 和 STOR ftp。</param>
        /// <param name="data">要上载的字符串。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void UploadStringAsync(Uri address,string method, string data, UploadStringCompletedEventHandler callback,object userToken)
        {
            base.UploadStringCompleted += callback;
            base.UploadStringAsync(address, method, data,userToken);
        }
        #endregion

        #region 将指定的名称/值集合中的数据上载到由指定的 URI 标识的资源
        /// <summary>
        /// 将指定的名称/值集合中的数据上载到由指定的 URI 标识的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收该集合资源的 URI。 此 URI 必须标识可以接受使用默认方法发送的请求的资源。 请参阅备注。</param>
        /// <param name="method">用来将字符串发送到资源的 HTTP 方法。 如果为 null，默认值为 POST （http） 和 STOR ftp。</param>
        /// <param name="data">System.Collections.Specialized.NameValueCollection 将发送到资源。</param>
        /// <param name="callback">回调方法</param>
        /// <param name="userToken">用户定义的对象传递给异步操作完成时调用的方法。</param>
        public void UploadValuesAsync(Uri address, string method, System.Collections.Specialized.NameValueCollection data, UploadValuesCompletedEventHandler callback, object userToken = null)
        {
            base.UploadValuesCompleted += callback;
            base.UploadValuesAsync(address, method, data, userToken);
        }
        /// <summary>
        /// 将指定的名称/值集合中的数据上载到由指定的 URI 标识的资源。 此方法不会阻止调用线程。
        /// </summary>
        /// <param name="address">要接收该集合资源的 URI。 此 URI 必须标识可以接受使用默认方法发送的请求的资源。 请参阅备注。</param>
        /// <param name="data">System.Collections.Specialized.NameValueCollection 将发送到资源。</param>
        /// <param name="callback">回调方法</param>
        public void UploadValuesAsync(Uri address, System.Collections.Specialized.NameValueCollection data, UploadValuesCompletedEventHandler callback)
        {
            base.UploadValuesCompleted += callback;
            base.UploadValuesAsync(address, data);
        }
        #endregion

        #endregion
    }
}