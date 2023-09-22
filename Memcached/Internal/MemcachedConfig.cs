using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XiaoFeng.Collections;
using XiaoFeng.Memcached.Transform;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 16:33:31                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcached配置
    /// </summary>
    public class MemcachedConfig
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedConfig()
        {
            this.Servers = new List<IPEndPoint>();
            this.Protocol = MemcachedProtocol.Text;
            this.Transform= new ModifiedFNV1_32();
            this.Encoding = Encoding.UTF8;
        }
        /// <summary>
        /// 设置服务器
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public MemcachedConfig(string host, int port, string username, string password) : this()
        {
            this.UserName = username;
            this.Password = password;
            this.AddServer(host, port);
        }
        /// <summary>
        /// 设置服务器
        /// </summary>
        /// <param name="iPEndPoints">网络终节点</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public MemcachedConfig(IEnumerable<IPEndPoint> iPEndPoints, string username, string password):this()
        {
            this.AddServer(iPEndPoints);
            this.UserName = username;
            this.Password = password;
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="connString">连接串</param>
        /// <remarks>
        /// <para><see langword="格式"/> : [&lt;protocol&gt;]://[[&lt;username&gt;:&lt;password&gt;@]&lt;host&gt;:&lt;port&gt;][?&lt;ConnectionTimeout&gt;=10[&amp;&lt;ReadTimeout&gt;=10][&amp;&lt;WiteTimeout&gt;=10][&amp;&lt;PoolSize&gt;=10]]</para>
        /// <para><term>protocol</term> 协议，固定值为memcached</para>
        /// <para><term>username</term> 帐号</para>
        /// <para><term>password</term> 密码</para>
        /// <para><term>host</term> 服务器地址或DNS</para>
        /// <para><term>port</term> 服务器端口 默认为11211</para>
        /// <para><term>ConnectionTimeout</term> 连接超时时间 单位为秒</para>
        /// <para><term>ReadTimeout</term> 读取超时时间 单位为秒</para>
        /// <para><term>WiteTimeout</term> 写入超时时间 单位为秒</para>
        /// <para><term>PoolSize</term> 连接池数量</para>
        /// <para>例子 : memcached://memcached:7092734@127.0.0.01:11211?ConnectionTimeout=10&amp;ReadTimeout=10&amp;WiteTimeout=10&amp;PoolSize=10
        /// </para>
        /// </remarks>
        public MemcachedConfig(string connString) : this()
        {
            var uri = new Uri(connString);
            if (!uri.Scheme.EqualsIgnoreCase("memcached")) return;
            this.AddServer(uri.Host, uri.Port == -1 ? 11211 : uri.Port);
            if (uri.UserInfo.IsNotNullOrEmpty())
            {
                var vals = uri.UserInfo.Split(new char[] { ':' });
                if (vals.Length == 1) return;
                this.UserName = vals[0];
                this.Password = vals[1];
            }
            if (uri.Query.IsNotNullOrEmpty())
            {
                var paramers = new ParameterCollection(uri.Query);
                if (paramers.Contains("ConnectionTimeout"))
                {
                    this.ConnectTimeout = paramers["ConnectionTimeout"].ToCast<int>();
                }
                if (paramers.Contains("readtimeout"))
                {
                    this.ReadTimeout = paramers["readtimeout"].ToCast<int>();
                }
                if (paramers.Contains("writetimeout"))
                {
                    this.WriteTimeout = paramers["writetimeout"].ToCast<int>();
                }
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 服务器集群
        /// </summary>
        public IList<IPEndPoint> Servers { get; set; }
        /// <summary>
        /// 传输协议
        /// </summary>
        public MemcachedProtocol Protocol { get; set; }
        /// <summary>
        /// 客户端证书
        /// </summary>
        public X509CertificateCollection Certificates { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Hash算法
        /// </summary>
        public IMemcachedTransform Transform { get; set; }
        /// <summary>
        /// 读超时 单位秒
        /// </summary>
        public int ReadTimeout { get; set; }
        /// <summary>
        /// 写超时 单位秒
        /// </summary>
        public int WriteTimeout { get; set; }
        /// <summary>
        /// 连接超时 单位秒
        /// </summary>
        public int ConnectTimeout { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// 线程数量 设置当前线程数量则启用线程池来处理
        /// </summary>
        public int PoolSize { get; set; }
        /// <summary>
        /// 压缩长度 默认是1M
        /// </summary>
        public int CompressLength { get; set; } = 1024 * 1024;
        #endregion

        #region 方法
        /// <summary>
        /// 添加服务器
        /// </summary>
        /// <param name="host">地址</param>
        /// <param name="port">端口</param>
        public void AddServer(string host, int port = 11211)
        {
            IPAddress addr;
            if (host.IsIP())
            {
                addr = IPAddress.Parse(host);
            }
            else
            {
                addr = Dns.GetHostAddresses(host).First(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }
            var endpoint = new IPEndPoint(addr, port);
            this.AddServer(endpoint);
        }
        /// <summary>
        /// 添加服务器
        /// </summary>
        /// <param name="endPoint">终点</param>
        public void AddServer(IPEndPoint endPoint)
        {
            if (this.Servers.Contains(endPoint)) return;
            this.Servers.Add(endPoint);
        }
        /// <summary>
        /// 添加服务器
        /// </summary>
        /// <param name="iPEndPoints">终点</param>
        public void AddServer(IEnumerable<IPEndPoint> iPEndPoints)
        {
            foreach (var iPEndPoint in iPEndPoints)
            {
                this.AddServer(iPEndPoint);
            }
        }
        #endregion
    }
}