using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using System.IO;

using HttpContext = XiaoFeng.Web.HttpContext;

namespace XiaoFeng
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
       Create Time : 2017/12/22 15:11:05
    ===================================================================
    */
    /// <summary>
    /// 获取请求数据
    /// Verstion : 1.0.7
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2017/12/22 15:11:05
    /// Update Time : 2018/06/16 02:26:13
    /// </summary>
    public static class TokenX
    {
        #region 属性
        /// <summary>
        /// 请求对象
        /// </summary>
#if NETFRAMEWORK
        private static System.Web.HttpRequest Request { get { return HttpContext.Current.Request; } }
#else
        private static Microsoft.AspNetCore.Http.HttpRequest Request { get { return XiaoFeng.Web.HttpContext.Current.Request; } }
#endif
        #endregion

        #region 方法

        #region 获取参数值
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="queryName">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string Get(string queryName = "", string defaultValue = "")
        {
            return Get<string>(queryName, defaultValue);
        }
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="queryName">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Get<T>(string queryName = "", T defaultValue = default(T))
        {
#if NETFRAMEWORK
            if (queryName.IsNullOrEmpty())
            {
                var _ = "";
                Request.QueryString.AllKeys.Each(q =>
                {
                    _ += $"{q}={Request.QueryString[q]}&";
                });
                return _.Trim('&').ToCast<T>(defaultValue);
            }
            return Request.QueryString[queryName] == null ? defaultValue : Request.QueryString[queryName].ToString().ToCast(defaultValue);
#else
            if (queryName.IsNullOrEmpty()) return Request.QueryString.Value.ToCast(defaultValue);
            return Request.Query[queryName].Count == 0 ? defaultValue : Request.Query[queryName].ToString().ToCast(defaultValue);
#endif
        }
        #endregion

        #region 获取表单值
        /// <summary>
        /// 获取表单值
        /// </summary>
        /// <param name="postName">表单名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string Post(string postName = "", string defaultValue = "")
        {
            return Post<string>(postName, defaultValue);
        }
        /// <summary>
        /// 获取表单值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="postName">表单名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
#if NETFRAMEWORK
        public static T Post<T>(string postName = "", T defaultValue = default(T))
        {
            if (Request.HttpMethod != "POST") return defaultValue;
            if (postName.IsNullOrEmpty())
            {
                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (Request.Form.Keys.Count > 0)
                    Request.Form.Keys.ToList<string>().Each(k =>
                    {
                        if (Request.Form[k] != null)
                            data.Add(k, Request.Form[k].ToString());
                    });
                else
                {
                    if (Request.InputStream.Length > 0)
                    {
                        var _stream = new MemoryStream();
                        Request.InputStream.Position = 0;
                        Request.InputStream.CopyTo(_stream);
                        if (_stream.Length > 0)
                        {
                            _stream.Position = 0;
                            using (var sr = new StreamReader(_stream))
                            {
                                var _ = sr.ReadToEnd();
                                if ((_.IsQuery() && !_.IsJson()) || _.IsJson())
                                {
                                    var d = _.JsonToObject<Dictionary<string, string>>();
                                    d.Each(a =>
                                    {
                                        if (data.ContainsKey(a.Key))
                                            data[a.Key] = a.Value;
                                        else
                                            data.Add(a.Key, a.Value);
                                    });
                                }
                            }
                        }
                    }
                }
                return data.DictionaryToObject<T>();
            }
            else
            {
                if (Request.Form.AllKeys.Length > 0)
                {
                    if (Request.Form[postName] != null)
                        return Request.Form[postName].ToString().ToCast(defaultValue);
                    else return defaultValue;
                }
                else
                {
                    if (Request.InputStream.Length > 0)
                    {
                        var _stream = new MemoryStream();
                        Request.InputStream.Position = 0;
                        Request.InputStream.CopyTo(_stream);
                        _stream.Position = 0;
                        if (_stream.Length > 0)
                        {
                            using (var sr = new StreamReader(_stream))
                            {
                                var _ = sr.ReadToEnd();
                                if ((_.IsQuery() && !_.IsJson()) || _.IsJson())
                                {
                                    var d = _.JsonToObject<Dictionary<string, string>>();
                                    if (d.ContainsKey(postName))
                                        return d[postName].ToCast<T>();
                                }
                            }
                        }
                    }
                    return defaultValue;
                }
            }
        }
#else
        public static T Post<T>(string postName = "", T defaultValue = default(T))
        {
            if (Request.Method != "POST") return defaultValue;
            if (postName.IsNullOrEmpty())
            {
                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (Request.HasFormContentType)
                    Request.Form.Keys.Each(k =>
                    {
                        if (Request.Form.TryGetValue(k, out var val))
                            data.Add(k, val.ToString());
                    });
                else
                {
                    if (Request.Body.Length > 0)
                    {
                        var _stream = new MemoryStream();
                        Request.Body.Position = 0;
                        Request.Body.CopyTo(_stream);
                        if (_stream.Length > 0)
                        {
                            _stream.Position = 0;
                            using (var sr = new StreamReader(_stream))
                            {
                                var _ = sr.ReadToEnd();
                                if ((_.IsQuery() && !_.IsJson()) || _.IsJson())
                                {
                                    var d = _.JsonToObject<Dictionary<string, string>>();
                                    d.Each(a =>
                                    {
                                        if (data.ContainsKey(a.Key))
                                            data[a.Key] = a.Value;
                                        else
                                            data.Add(a.Key, a.Value);
                                    });
                                }
                            }
                        }
                    }
                }
                return data.DictionaryToObject<T>();
            }
            else
            {
                if (Request.HasFormContentType)
                {
                    if (Request.Form.TryGetValue(postName, out var val))
                        return val.ToString().ToCast(defaultValue);
                    else return defaultValue;
                }
                else
                {
                    if (Request.Body.Length > 0)
                    {
                        var _stream = new MemoryStream();
                        Request.Body.Position = 0;
                        Request.Body.CopyTo(_stream);
                        _stream.Position = 0;
                        if (_stream.Length > 0)
                        {
                            using (var sr = new StreamReader(_stream))
                            {
                                var _ = sr.ReadToEnd();
                                if ((_.IsQuery() && !_.IsJson()) || _.IsJson())
                                {
                                    var d = _.JsonToObject<Dictionary<string, string>>();
                                    if (d.ContainsKey(postName))
                                        return d[postName].ToCast<T>();
                                }
                            }
                        }
                    }
                    return defaultValue;
                }
            }
        }
#endif
        #endregion

        #region 获取Header值
        /// <summary>
        /// 获取Header值
        /// </summary>
        /// <param name="headerName">Header名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string Header(string headerName = "", string defaultValue = "")
        {
            return Header<string>(headerName, defaultValue);
        }
        /// <summary>
        /// 获取Header值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="headerName">Header名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Header<T>(string headerName = "", T defaultValue = default(T))
        {
#if NETFRAMEWORK
            if (headerName.IsNullOrEmpty())
            {
                var _ = "";
                Request.Headers.AllKeys.Each(h =>
                {
                    _ += $"{h}={Request.Headers[h]}&";
                });
                return _.Trim('&').ToCast<T>(defaultValue);
            }
            return Request.Headers[headerName] == null ? defaultValue : Request.Headers[headerName].ToString().ToCast(defaultValue);
#else
            if (headerName.IsNullOrEmpty()) return Request.Headers.ToQuery().ToCast(defaultValue);
            return Request.Headers.ContainsKey(headerName) ? Request.Headers[headerName].ToString().ToCast(defaultValue) : defaultValue;
#endif
        }
        #endregion

        #region 获取Cookie值
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string Cookie(string cookieName = "", string defaultValue = "")
        {
            return Cookie<string>(cookieName, defaultValue);
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Cookie<T>(string cookieName = "", T defaultValue = default(T))
        {
            if (Request.Cookies.Count == 0) return defaultValue;
            Dictionary<string, string> d = new Dictionary<string, string>();
#if NETFRAMEWORK
            if (cookieName.IsNotNullOrEmpty())
            {
                if (Request.Cookies[cookieName] != null)
                    return Request.Cookies[cookieName].Value.ToString().ReplaceSQL().ToCast(defaultValue);
                else return defaultValue;
            }
            Request.Cookies.Keys.ToList<string>().Each(k =>
            {
                if (Request.Cookies[k] != null)
                    d.Add(k, Request.Cookies[k].Value.ToString());
            });
#else
            if (cookieName.IsNotNullOrEmpty())
            {
                if (Request.Cookies.TryGetValue(cookieName, out var val))
                    return val.ToString().ReplaceSQL().ToCast(defaultValue);
                else return defaultValue;
            }
            Request.Cookies.Keys.Each(k =>
            {
                if (Request.Cookies.TryGetValue(k, out var val))
                    d.Add(k, val.ToString());
            });
#endif
            return d.DictionaryToObject<T>();
        }
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string Value(string name = "", string defaultValue = "")
        {
            return Value<string>(name, defaultValue);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="name">名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Value<T>(string name = "", T defaultValue = default(T))
        {
            return new XToken().Value(name, defaultValue);
        }
        #endregion

        #endregion
    }
}