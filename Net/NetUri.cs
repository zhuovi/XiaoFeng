using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Collections;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 22:53:39                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络地址
    /// </summary>
    public class NetUri
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public NetUri() { }
        /// <summary>
        /// 设置网络地址
        /// </summary>
        /// <param name="uri">网络地址</param>
        public NetUri(Uri uri)
        {
            if (uri == null) return;
            this.ParseUri(uri);
        }
        /// <summary>
        /// 设置网络地址
        /// </summary>
        /// <param name="uri">网络地址</param>
        public NetUri(string uri)
        {
            if (uri.IsNullOrEmpty()) return;
            this.ParseUri(new Uri(uri));
        }
        /// <summary>
        /// 设置网络地址
        /// </summary>
        /// <param name="netType">网络类型</param>
        /// <param name="host">主机</param>
        /// <param name="port">端口号</param>
        public NetUri(NetType netType, string host, int port) : this(netType, host, port, string.Empty) { }
        /// <summary>
        /// 设置网络地址
        /// </summary>
        /// <param name="netType">网络类型</param>
        /// <param name="host">主机</param>
        /// <param name="port">端口号</param>
        /// <param name="pathAndQuery">路径和参数</param>
        public NetUri(NetType netType, string host, int port, string pathAndQuery) : this(netType, string.Empty, string.Empty, host, port, pathAndQuery) { }
        /// <summary>
        /// 设置网络地址
        /// </summary>
        /// <param name="netType">网络类型</param>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="host">主机</param>
        /// <param name="port">端口号</param>
        public NetUri(NetType netType, string userName, string password, string host, int port) : this(netType, userName, password, host, port, string.Empty) { }
        /// <summary>
        /// 设置网络地址
        /// </summary>
        /// <param name="netType">网络类型</param>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="host">主机</param>
        /// <param name="port">端口号</param>
        /// <param name="pathAndQuery">路径和参数</param>
        public NetUri(NetType netType, string userName, string password, string host, int port, string pathAndQuery)
        {
            this._NetType = netType;
            this._Host = host;
            this._Port = port;
            this._Scheme = netType.ToString().ToLower();
            if (pathAndQuery.IsNotNullOrEmpty())
            {
                this._PathAndQuery = "/" + pathAndQuery.TrimStart('/');
                var querys = this._PathAndQuery.Split('?');
                this._AbsolutePath = querys[0];
                this._Segments = this._AbsolutePath.Split('/');
                if (querys.Length == 2)
                {
                    this._Query = "?" + querys[1];
                    this._Parameters = new ParameterCollection(this._Query);
                }
            }
            this._UserName = userName;
            this._Password = password;
            this._EndPoint = new IPEndPoint(Dns.GetHostAddresses(this.Host).LastOrDefault(), this.Port);
            var userInfo = "";
            if (this.UserName.IsNotNullOrEmpty())
                userInfo = $"{this.UserName}:{this.Password}@";
            else if (this.Password.IsNotNullOrEmpty())
                userInfo = $"{this.Password}@";
            this._OriginalString = $"{this.NetType.ToString().ToLower()}://{userInfo}{this.Host}:{this.Port}{this.PathAndQuery}";
        }
        #endregion

        #region 属性
        /// <summary>
        /// 方案名称
        /// </summary>
        private string _Scheme;
        /// <summary>
        /// 方案名称
        /// </summary>
        public string Scheme => this._Scheme;
        /// <summary>
        /// 方案类型
        /// </summary>
        private NetType _NetType;
        /// <summary>
        /// 方案类型
        /// </summary>
        public NetType NetType => this._NetType;
        /// <summary>
        /// 帐号
        /// </summary>
        private string _UserName;
        /// <summary>
        /// 帐号
        /// </summary>
        public string UserName => this._UserName;
        /// <summary>
        /// 密码
        /// </summary>
        private string _Password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password => this._Password;
        /// <summary>
        /// 主机名
        /// </summary>
        private string _Host;
        /// <summary>
        /// 主机名
        /// </summary>
        public string Host => this._Host;
        /// <summary>
        /// 端口号
        /// </summary>
        private int _Port;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port => this._Port;
        /// <summary>
        /// 获取用问号 (?) 分隔的 System.Uri.AbsolutePath 和 System.Uri.Query 属性
        /// </summary>
        private string _PathAndQuery;
        /// <summary>
        /// 获取用问号 (?) 分隔的 System.Uri.AbsolutePath 和 System.Uri.Query 属性
        /// </summary>
        public string PathAndQuery => this._PathAndQuery;
        /// <summary>
        /// 获取包含构成指定 URI 的路径段的数组
        /// </summary>
        private string[] _Segments;
        /// <summary>
        /// 获取包含构成指定 URI 的路径段的数组
        /// </summary>
        public string[] Segments => this._Segments;
        /// <summary>
        /// 资源的绝对路径
        /// </summary>
        private string _AbsolutePath;
        /// <summary>
        /// 资源的绝对路径。
        /// </summary>
        public string AbsolutePath => this._AbsolutePath;
        /// <summary>
        /// 原始数据
        /// </summary>
        private string _OriginalString;
        /// <summary>
        /// 原始数据
        /// </summary>
        public string OriginalString => this._OriginalString;
        /// <summary>
        /// 参数集合
        /// </summary>
        private ParameterCollection _Parameters;
        /// <summary>
        /// 参数集合
        /// </summary>
        public ParameterCollection Parameters => this._Parameters;
        /// <summary>
        /// 参数信息
        /// </summary>
        private string _Query;
        /// <summary>
        /// 参数信息
        /// </summary>
        public string Query => this._Query;
        /// <summary>
        /// 网络终结点
        /// </summary>
        private IPEndPoint _EndPoint;
        /// <summary>
        /// 网络终结点
        /// </summary>
        public IPEndPoint EndPoint => this._EndPoint;
        #endregion

        #region 方法
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="uri">URI</param>
        private void ParseUri(Uri uri)
        {
            this._Scheme = uri.Scheme;
            this._NetType = this.Scheme.ToEnum<NetType>();
            this._AbsolutePath = uri.AbsolutePath;
            var userInfo = uri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (userInfo.Length == 2)
            {
                this._UserName = userInfo[0];
                this._Password = userInfo[1];
            }
            else if (userInfo.Length == 1)
                this._Password = userInfo[0];
            this._OriginalString = uri.OriginalString;
            this._Host = uri.Host;
            this._Port = uri.Port == 0 ? this.NetType.GetPort() : uri.Port;
            this._PathAndQuery = uri.PathAndQuery;
            this._Segments = uri.Segments;
            this._Query = uri.Query;
            if (this._Query.IsNotNullOrEmpty())
                this._Parameters = new ParameterCollection(this._Query.TrimStart('?'));
            if (this._Host.IsNullOrEmpty())
            {
                this._EndPoint = new IPEndPoint(IPAddress.Any, this.Port);
            }
            else
            {
                this._EndPoint = new IPEndPoint(Dns.GetHostAddresses(this.Host).LastOrDefault(), this.Port);
            }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="uri">网址</param>
        /// <returns></returns>
        public static NetUri Parse(string uri)
        {
            return new NetUri(uri);
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="uri">网址</param>
        /// <returns></returns>
        public static NetUri Parse(Uri uri)
        {
            return new NetUri(uri);
        }
        /// <summary>
        /// 重写字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.OriginalString;
        }
        /// <summary>
        /// HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.OriginalString.GetHashCode();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="uri">网络地址</param>
        public static implicit operator NetUri(Uri uri)
        {
            return new NetUri(uri);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="uri">网络地址</param>
        public static explicit operator Uri(NetUri uri)
        {
            return new Uri(uri.OriginalString);
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="uri">网络地址</param>
        public static implicit operator string(NetUri uri)
        {
            return uri.OriginalString;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="uri">网络地址</param>
        public static explicit operator NetUri(string uri)
        {
            return new NetUri(uri);
        }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is NetUri uri)
            {
                return uri.ToString().EqualsIgnoreCase(this.ToString());
            }
            return false;
        }
        /// <summary>
        /// 两类型相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator ==(NetUri a, NetUri b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            return a.Equals(b);
        }
        /// <summary>
        /// 两类型不相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator !=(NetUri a, NetUri b)
        {
            return !(a == b);
        }
        #endregion
    }
}