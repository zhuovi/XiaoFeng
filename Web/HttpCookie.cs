#if NETCORE
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace XiaoFeng.Web
{
    /// <summary>
    /// HttpCookie
    /// </summary>
    public static class HttpCookie
    {
        #region 设置Cookie
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="options">配置</param>
        public static void Set(string name, string value, CookieOption options = null)
        {
            if (name.IsNullOrEmpty()) return;
#if NETFRAMEWORK
            System.Web.HttpCookie cookie = new System.Web.HttpCookie(name)
            {
                Value = options == null || !options.IsEncrypt ? value : "F:" + value.ELFEncrypt().TrimEnd('='),
                Path = options == null || options.Path.IsNullOrEmpty() ? "/" : options.Path,
                HttpOnly = options == null || options.HttpOnly,
                SameSite = options == null ? SameSiteMode.Lax : options.SameSite,
                Secure = options == null ? HttpContext.Current.Request.IsSecureConnection : options.Secure
            };
            if (options != null)
            {
                if (options.Expires.HasValue) cookie.Expires = options.Expires.Value;
                if (options.Domain.IsNotNullOrEmpty()) cookie.Domain = options.Domain;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
#else
            var option = new CookieOptions()
            {
                Path = options == null || options.Path.IsNullOrEmpty() ? "/" : options.Path,
                HttpOnly = options == null || options.HttpOnly,
                IsEssential = options == null || options.IsEssential,
                SameSite = options == null ? SameSiteMode.Lax : options.SameSite,
                Secure = options == null ? HttpContext.Current.Request.IsHttps : options.Secure
            };
            if (options != null)
            {
                if (options.Expires.HasValue) option.Expires = new DateTimeOffset(options.Expires.Value);
                if (options.Domain.IsNotNullOrEmpty()) option.Domain = options.Domain;
            }
            HttpContext.Current.Response.Cookies.Append(name, options == null || !options.IsEncrypt ? value : "F:" + value.ELFEncrypt(), option);
#endif
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="domain">域名</param>
        /// <param name="path">路径</param>
        /// <param name="encrypt">是否加密</param>
        public static void Set(string name, string value, string domain, string path = "", Boolean encrypt = true) => Set(name, value, new CookieOption { Domain = domain, Path = path, IsEncrypt = encrypt });
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <param name="encrypt">是否加密</param>
        public static void Set(string name, string value, DateTime expire, Boolean encrypt = true) => Set(name, value, new CookieOption
        { Expires = expire, IsEncrypt = encrypt });
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="encrypt">是否加密 true加密false不加密</param>
        public static void Set(string name, string value, Boolean encrypt) => Set(name, value, new CookieOption { IsEncrypt = encrypt });
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">值</param>
        /// <param name="options">配置</param>
        public static void Set(string name, Dictionary<string, string> data, CookieOption options = null) => Set(name, data.ToQuery(), options);
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">值</param>
        /// <param name="domain">域名</param>
        /// <param name="path">路径</param>
        /// <param name="encrypt">是否加密</param>
        public static void Set(string name, Dictionary<string, string> data, string domain, string path = "", Boolean encrypt = true) => Set(name, data.ToQuery(), domain, path, encrypt);
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">值</param>
        /// <param name="expire">过期时间</param>
        /// <param name="encrypt">是否加密</param>
        public static void Set(string name, Dictionary<string, string> data, DateTime expire, Boolean encrypt = true) => Set(name, data.ToQuery(), expire, encrypt);
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">值</param>
        /// <param name="encrypt">是否加密 true加密false不加密</param>
        public static void Set(string name, Dictionary<string, string> data, Boolean encrypt) => Set(name, data.ToQuery(), encrypt);
        #endregion

        #region 获取Cookie
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="name">cookie名称</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            var value = "";
#if NETFRAMEWORK
            value = HttpContext.Current.Request.Cookies[name]?.Value;
#else
            if (HttpContext.Current == null || !HttpContext.Current.Request.Cookies.TryGetValue(name, out value)) return string.Empty;
#endif
            if (value.IsNullOrEmpty()) return string.Empty;
            else if (value.IsMatch(@"^F:")) return value.RemovePattern(@"^F:").ELFDecrypt();
            return value;
        }
        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static Dictionary<string, string> Gets(string name)
        {
            var val = Get(name);
            if (val.IsQuery()) return val.ObjectToDictionary() as Dictionary<string, string>;
            else return new Dictionary<string, string> { { name, val } };
        }
        /// <summary>
        /// 获取cookie集中的值
        /// </summary>
        /// <param name="names">集名</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string Gets(string names, string key)
        {
            var dic = Gets(names);
            return dic.TryGetValue(key, out var val) ? val : string.Empty;
        }
        #endregion

        #region 移除Cookie
        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="name">名称</param>
        public static void Remove(string name)
        {
            #if NETFRAMEWORK
            System.Web.HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                cookie.Value = "";
                cookie.Values.Clear();
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            #else
            HttpContext.Current.Response.Cookies.Delete(name);
            #endif
        }
        #endregion

        #region 移除所有Cookie
        /// <summary>
        /// 移除所有Cookie
        /// </summary>
        public static void Clear()
        {
            var Request = HttpContext.Current.Request;
            var Response = HttpContext.Current.Response;
#if NETFRAMEWORK
            Response.Cookies.Clear();
            Request.Cookies.AllKeys.Each(k => Response.Cookies.Remove(k));
#else
            Request.Cookies.Keys.Each(k => Response.Cookies.Delete(k));
#endif
        }
#endregion
    }

    #region Cookie配置
    /// <summary>
    /// Cookie配置
    /// </summary>
    public class CookieOption
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public CookieOption()
        {
            if (HttpContext.Current.Request.
#if NETFRAMEWORK
            IsSecureConnection
#else
            IsHttps
#endif
            ) { this.Secure = true;  }
        }
        /// <summary>
        /// 生效目录
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? Expires { get; set; }
        /// <summary>
        /// 是否允许客户端脚本访问
        /// </summary>
        public Boolean HttpOnly { get; set; } = true;
        /// <summary>
        /// 是否加密
        /// </summary>
        public Boolean IsEncrypt { get; set; } = true;
        /// <summary>
        /// Cookie是否是必须的
        /// </summary>
        public Boolean IsEssential { get; set; } = true;
        /// <summary>
        /// SameSite
        /// </summary>
        public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;
        /// <summary>
        /// 安全
        /// </summary>
        public Boolean Secure { get; set; } = false;
    }
#endregion
}