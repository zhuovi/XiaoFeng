using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.IO;
using XiaoFeng.Memcached.Protocol.Binary;
using XiaoFeng.Memcached.Protocol.Text;
using XiaoFeng.Memcached.Transform;
using XiaoFeng.Net;

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
namespace XiaoFeng.Memcached.Internal
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
        /// 设置配置
        /// </summary>
        /// <param name="connString">连接串</param>
        /// <remarks>
        /// 格式如 memcached://[[username][:password]@]host[:port]?[readtimeout=10]&amp;[writetimeout=10]&amp;[pool=3]
        /// <para>
        /// <see langword="username"/> 帐号
        /// </para>
        /// <para>
        /// <see langword="username"/> 密码
        /// </para>
        /// <para>
        /// <see langword="host"/> 主机
        /// </para>
        /// <para>
        /// <see langword="port"/> 端口号
        /// </para>
        /// <para>
        /// <see langword="readtimeout"/> 读超时时间长 单位为秒
        /// </para>
        /// <para>
        /// <see langword="writetimeout"/> 写超时时间长 单位为秒
        /// </para>
        /// <para>
        /// <see langword="pool"/> 线程池数量
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
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// 线程数量
        /// </summary>
        public int Pool { get; set; }
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
        #endregion
    }
}