using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    #region 参数操作类 第一种内核
    /// <summary>
    /// 参数操作类 第一种内核 第二种 ParamHelper
    /// Version : 1.0.1
    /// </summary>
    public class QueryHelper:IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public QueryHelper() { this.data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase); }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="KeyValue">网址,键值对 如 a=b&amp;c=d 或a:b,c:d</param>
        public QueryHelper(string KeyValue)
        {
            this.data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.setData(KeyValue);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据
        /// </summary>
        public SortedDictionary<string, string> data { get; set; }
        /// <summary>
        /// 网址
        /// </summary>
        private string _path = "";
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get { return this._path; } set { this._path = value; } }
        /// <summary>
        /// 参数
        /// </summary>
        public string query { get { return this.getQuery(); } }
        /// <summary>
        /// 网址
        /// </summary>
        public string url { get { return this.path + (this.path.IsNullOrEmpty() ? "" : "?") + this.getQuery(); } }
        /// <summary>
        /// 键值对数目
        /// </summary>
        public int Count { get { return this.data.Count; } }
        /// <summary>
        /// 网址长度
        /// </summary>
        public int Length { get { return this.url.Length; } }
        #endregion

        #region 方法

        #region 添加数据
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">键值</param>
        public QueryHelper add(string key, string value)
        {
            if (key.IsNullOrEmpty()) return this;
            if (this.data.ContainsKey(key))
                this.data[key] = value;
            else
                this.data.Add(key, value);
            return this;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="KeyValue">网址,键值对 如 a=b&amp;c=d 或a:b,c:d</param>
        /// <returns></returns>
        public QueryHelper add(string KeyValue)
        {
            this.setData(KeyValue);
            return this;
        }
        #endregion

        #region 设置数据
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">键值</param>
        /// <returns></returns>
        public QueryHelper set(string key, string value) { return this.add(key, value); }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="keyValue">网址,键值对 如 a=b&amp;c=d 或a:b,c:d</param>
        /// <returns></returns>
        public QueryHelper set(string keyValue) { return this.add(keyValue); }
        #endregion

        #region 按Key值移除数据
        /// <summary>
        /// 按Key值移除数据
        /// </summary>
        /// <param name="key">key值</param>
        public QueryHelper RemoveKey(string key)
        {
            if (this.data.ContainsKey(key))
                this.data.Remove(key);
            return this;
        }
        #endregion

        #region 按value值移除数据
        /// <summary>
        /// 按value值移除数据
        /// </summary>
        /// <param name="value">键值</param>
        public QueryHelper RemoveValue(string value)
        {
            if (this.data.ContainsValue(value))
                this.data.Remove(this.getKey(value));
            return this;
        }
        #endregion

        #region 按Key值移除数据
        /// <summary>
        /// 按Key值移除数据
        /// </summary>
        /// <param name="key">key值</param>
        public QueryHelper remove(string key) { return this.RemoveKey(key); }
        #endregion

        #region 获取参数字符串
        /// <summary>
        /// 获取参数字符串
        /// </summary>
        /// <returns></returns>
        private string getQuery()
        {
            string s = "";
            foreach (KeyValuePair<string, string> k in this.data)
                s += k.Key + "=" + k.Value + "&";
            return s.Trim('&');
        }
        #endregion

        #region 清空数据
        /// <summary>
        /// 清空数据
        /// </summary>
        public QueryHelper Clear()
        {
            this.data.Clear();
            this.path = "";
            return this;
        }
        #endregion

        #region 根据键值查找key
        /// <summary>
        /// 根据键值查找key
        /// </summary>
        /// <param name="value">键值</param>
        /// <returns></returns>
        public string getKey(string value)
        {
            if (this.data.ContainsValue(value))
                foreach (KeyValuePair<string, string> k in this.data)
                    if (k.Value == value) return k.Key;
            return "";
        }
        #endregion

        #region 根据key查找键值
        /// <summary>
        /// 根据key查找键值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string getValue(string key)
        {
            if (key.IsNullOrEmpty()) return "";
            if (this.data.ContainsKey(key))
                return this.data[key];
            return "";
        }
        #endregion

        #region 根据key查找键值
        /// <summary>
        /// 根据key查找键值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string get(string key) { return this.getValue(key); }
        #endregion

        #region 转换成字符串
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return this.getQuery(); }
        #endregion

        #region 设置数据
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="keyValue">网址,键值对 如 a=b&amp;c=d 或a:b,c:d</param>
        public void setData(string keyValue)
        {
            if (keyValue.IsNullOrEmpty()) return;
            if (keyValue.IsSite() || keyValue.IsMatch(@"\?([^=&?#]+=[^=&#]*)*"))
            {
                string[] url = keyValue.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
                if (url.Length == 0) return;
                if (url.Length == 1) { this.path = url[0]; return; }
                if (url.Length > 1)
                {
                    this.path = url[0];
                    this.setData(url[1].IsNullOrEmpty() ? "" : ("&" + url[1]));
                }
            }
            else
            {
                if (keyValue.IsMatch(@"&?([^=&?#]+=[^=&?#]*)+")) keyValue = "&" + keyValue.TrimStart('&');
                keyValue = Regex.Replace(keyValue, @"^[\r\n\s]*{([^}]*?)}[\r\n\s]*$", "$1", RegexOptions.IgnoreCase);
                MatchCollection mc = Regex.Matches(keyValue, @"((([""']?)(?<a>[^,:]+)\3\s*:\s*([""']?)(?<b>[^,]*)\4)|(&(?<a>[^=&?#]+)=(?<b>[^=&?#]*)))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (Match m in mc)
                    this.add(this.ReplaceStartEnd(m.Groups["a"].Value.Trim()), m.Groups["b"].Value.Trim().UrlDecode());
            }
        }
        #endregion

        #region 替换首尾"'
        /// <summary>
        /// 替换首尾"'
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        private string ReplaceStartEnd(string str)
        {
            return Regex.Replace(str, @"^(['""]?)(.*?)\1", "$2", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 回收资源
        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        ~QueryHelper()
        {
            this.Dispose();
        }
        #endregion

        #endregion
    }
    #endregion

    #region 参数操作类 第二种内核
    /// <summary>
    /// 参数操作类 第二种内核  第一种 QueryHelper
    /// Version : 1.0.1
    /// </summary>
    public class ParamHelper:IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ParamHelper() { }
        /// <summary>
        /// 设置参数数据
        /// </summary>
        /// <param name="queryString">参数</param>
        public ParamHelper(string queryString)
        {
            this.setData(queryString);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据
        /// </summary>
        private string data = "&";
        /// <summary>
        /// 路径
        /// </summary>
        private string _path = "";
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get { return this._path; } set { this._path = value; } }
        /// <summary>
        /// 参数
        /// </summary>
        public string query { get { return this.data.TrimStart('&'); } }
        /// <summary>
        /// 参数列表
        /// </summary>
        public SortedDictionary<string, string> querys { get { return this.getQuery(); } }
        /// <summary>
        /// 网址
        /// </summary>
        public string url { get { return this.path + (this.path.IndexOf("?") > -1 ? "" : "?") + this.query; } }
        /// <summary>
        /// 参数数目
        /// </summary>
        public int Count { get { return this.getQuery().Count; } }
        /// <summary>
        /// 网址长度
        /// </summary>
        public int Length { get { return this.path.Length; } }
        #endregion

        #region 方法

        #region 添加参数
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="query">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public ParamHelper add(string query, string value)
        {
            if (this.data.IsMatch(@"[&]" + query + "=[^&]*"))
                this.data = this.data.ReplacePattern(@"([&])" + query + "=[^&]*", "$1" + query + "=" + value);
            else
                this.data += (this.data.IsNullOrEmpty() ? "" : "&") + query + "=" + value;
            return this;
        }
        #endregion

        #region 批量添加参数
        /// <summary>
        /// 批量添加参数
        /// </summary>
        /// <param name="queryValue">参数键值对 {a:b,c:d}或a=b&amp;c=d</param>
        /// <returns></returns>
        public ParamHelper add(string queryValue)
        {
            this.setData(queryValue);
            return this;
        }
        #endregion

        #region 设置参数
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="query">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public ParamHelper set(string query, string value) { return this.add(query, value); }
        #endregion

        #region 设置参数
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="queryValue">参数键值对</param>
        /// <returns></returns>
        public ParamHelper set(string queryValue) { return this.add(queryValue); }
        #endregion

        #region 移除参数
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="query">参数名</param>
        /// <returns></returns>
        public ParamHelper remove(string query)
        {
            if (this.data.IsMatch(@"[&]" + query + "=[^&]*"))
                this.data = this.data.ReplacePattern(@"([&])" + query + "=[^&]*", "$1").TrimEnd('&');
            return this;
        }
        #endregion

        #region 获取参数值
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="query">参数名</param>
        /// <returns></returns>
        public string get(string query)
        {
            if (this.data.IsMatch(@"&" + query + "=[^&]*"))
            {
                Match m = Regex.Match(this.data, @"&" + query + "=(?<v>[^&]*)", RegexOptions.IgnoreCase);
                if (m.Success)
                    return m.Groups["v"].Value.UrlDecode();
                else
                    return "";
            }
            return "";
        }
        #endregion

        #region 获取参数列表
        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <returns></returns>
        private SortedDictionary<string, string> getQuery()
        {
            SortedDictionary<string, string> d = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.data.GetMatches(@"&?(?<q>[^=&]+)=(?<v>[^&]*)").Each(m =>
            {
                d.Add(m["q"], m["v"].UrlDecode());
            });
            return d;
        }
        #endregion

        #region 设置数据
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="queryValue">网址,键值对 如 a=b&amp;c=d 或a:b,c:d</param>
        public void setData(string queryValue)
        {
            if (queryValue.IsNullOrEmpty()) return;
            if (queryValue.IsSite() || queryValue.IsMatch(@"\?([^=&?#]+=[^=&#]*)*"))
            {
                string[] url = queryValue.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
                if (url.Length == 0) return;
                if (url.Length == 1) { this.path = url[0]; return; }
                if (url.Length > 1)
                {
                    this.path = url[0];
                    this.setData(url[1].IsNullOrEmpty() ? "" : ("&" + url[1]));               
                }
            }
            else
            {
                if (queryValue.IsMatch(@"&?([^=&?#]+=[^=&?#]*)+")) queryValue = "&" + queryValue.TrimStart('&');
                queryValue = queryValue.ReplacePattern(@"^[\r\n\s]*{([^}]*?)}[\r\n\s]*$", "$1");
                queryValue.GetMatches(@"((([""']?)(?<a>[^,:]+)\3\s*:\s*([""']?)(?<b>[^,]*)\4)|(&(?<a>[^=&?#]+)=(?<b>[^=&?#]*)))").Each(m =>
                {
                    this.add(this.ReplaceStartEnd(m["a"].Trim()), m["b"].Trim().UrlDecode());
                });
            }
        }
        #endregion

        #region 清空数据
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            this.data = "";
            this.path = "";
        }
        #endregion

        #region 替换首尾"'
        /// <summary>
        /// 替换首尾"'
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        private string ReplaceStartEnd(string str)
        {
            return str.ReplacePattern(@"^(['""]?)(.*?)\1", "$2");
        }
        #endregion

        #region 回收资源
        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        ~ParamHelper()
        {
            this.Dispose();
        }
        #endregion

        #endregion
    }
    #endregion
}