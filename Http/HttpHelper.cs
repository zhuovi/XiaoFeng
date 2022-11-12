using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-26 18:11:49                            *
*  Version : v 1.0.3                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /*
    * 模拟http请求
    * 
    * 模拟请求就是传说中的 爬虫 工具所用的都是模拟请求
    * 
    * 请求类型包括 GET POST PUT HEAD OPTIONS DELETE TRACE PATCH
    * 
    * GET  从指定的资源请求数据。
    * 获取数据
    * 
    * POST 向指定的资源提交要被处理的数据。
    * 提交数据
    * 
    * HEAD 向服务器索要与GET请求相一致的响应，只不过响应体将不会被返回。
    * 获取内容头
    * 
    * PUT  向指定资源位置上传其最新内容。
    * 更新数据
    * 
    * OPTIONS  返回服务器针对特定资源所支持的HTTP请求方法。
    * 获取服务器所支持的请求类型
    * 
    * DELETE   请求服务器删除Request-URI所标识的资源。
    * 删除数据
    * 
    * TRACE    回显服务器收到的请求，主要用于测试或诊断。
    * 测试服务器性能
    * 
    * PATCH    实体中包含一个表，表中说明与该URI所表示的原内容的区别。
    * 验证数据差异
    * 
    */
    /// <summary>
    /// 网络请求库
    /// </summary>
    public class HttpHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public HttpHelper()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public async Task<HttpResponse> GetResponseAsync(HttpRequest request)
        {
            if (request.Address.IsNullOrEmpty() || !request.Address.IsSite()) return null;
            return await request.GetResponseAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public async Task<HttpResponse> GetResponseAsync(string url, HttpCore httpCore = HttpCore.HttpClient) => await this.GetResponseAsync(new HttpRequest
        {
            Method = "GET",
            Address = url,
            HttpCore = httpCore
        }).ConfigureAwait(false);
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="localPath">保存地址</param>
        /// <returns></returns>
        public async Task<long> DownFileAsync(HttpRequest request, string localPath)
        {
            var response = await this.GetResponseAsync(request).ConfigureAwait(false);
            return await response.DownFileAsync(localPath).ConfigureAwait(false);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="localPath">保存地址</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public async Task<long> DownFileAsync(string url, string localPath, HttpCore httpCore = HttpCore.HttpClient) => await this.DownFileAsync(new HttpRequest
        {
            Address = url,
            HttpCore = httpCore
        }, localPath);
        /// <summary>
        /// CURL请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="formData">formdata</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public async Task<HttpResponse> CURLAsync(string url, List<FormData> formData, HttpCore httpCore = HttpCore.HttpClient) => await new HttpRequest
        {
            Address = url,
            Method = HttpMethod.Post,
            FormData = formData,
            HttpCore = httpCore
        }.GetResponseAsync().ConfigureAwait(false);
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public HttpResponse GetResponse(HttpRequest request) => this.GetResponseAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="httpCore">核心</param>
        /// <returns></returns>
        public HttpResponse GetResponse(string url, HttpCore httpCore = HttpCore.HttpClient) => this.GetResponseAsync(url, httpCore).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="localPath">保存地址</param>
        /// <returns></returns>
        public void DownFile(HttpRequest request, string localPath)
        {
            this.DownFileAsync(request, localPath).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="localPath">保存地址</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public void DownFile(string url, string localPath, HttpCore httpCore = HttpCore.HttpClient) => this.DownFile(new HttpRequest
        {
            Address = url,
            HttpCore = httpCore
        }, localPath);
        /// <summary>
        /// CURL请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="formData">formdata</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public HttpResponse CURL(string url, List<FormData> formData, HttpCore httpCore = HttpCore.HttpClient) => CURLAsync(url, formData, httpCore).Result;
        #endregion

        #region 静态方法
        /// <summary>
        /// 单例
        /// </summary>
        private static WeakReference<HttpHelper> http;
        /// <summary>
        /// 获取单例
        /// </summary>
        public static HttpHelper Instance
        {
            get
            {
                http = http ?? new WeakReference<HttpHelper>(new HttpHelper());
                return http.IsAlive ? http.Target : http.Target = new HttpHelper();
            }
        }
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static async Task<HttpResponse> GetHtmlAsync(HttpRequest request) => await Instance.GetResponseAsync(request).ConfigureAwait(false);
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public static async Task<HttpResponse> GetHtmlAsync(string url, HttpCore httpCore = HttpCore.HttpClient) => await Instance.GetResponseAsync(url, httpCore).ConfigureAwait(false);
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static HttpResponse GetHtml(HttpRequest request) => Instance.GetResponse(request);
        /// <summary>
        /// 获取Http内容
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="httpCore">请求内核</param>
        /// <returns></returns>
        public static HttpResponse GetHtml(string url, HttpCore httpCore = HttpCore.HttpClient) => Instance.GetResponse(url, httpCore);
        #endregion
    }
}