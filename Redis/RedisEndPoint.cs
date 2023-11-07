using System;
using System.Net;
using System.Net.Sockets;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-12 16:24:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 将网络终结点表示为主机名或 IP 地址和端口号的字符串表示方法。
    /// </summary>
    public class RedisEndPoint : EndPoint
    {
        #region 构造器
        /// <summary>
        /// 设置节点
        /// </summary>
        /// <param name="address">节点 如:127.0.0.1:6379或127.0.0.1或</param>
        public RedisEndPoint(string address)
        {
            if (address.IsNotNullOrWhiteSpace()) return;
            if (address.Contains(":"))
            {
                var addrs = address.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (addrs.Length == 1)
                {
                    if (addrs[0].IsNumberic())
                    {
                        this.Host = "127.0.0.1";
                        this.Port = addrs[0].ToCast<int>();
                    }
                    else
                    {
                        this.Host = addrs[0];
                        this.Port = 6379;
                    }
                }
                else
                {
                    this.Host = addrs[0];
                    this.Port = addrs[1].IsNumberic() ? addrs[1].ToCast<int>() : 6379;
                }
            }
            else
            {
                if (address.IsNumberic())
                {
                    this.Host = "127.0.0.1";
                    this.Port = address.ToCast<int>();
                }
                else
                {
                    this.Host = address;
                    this.Port = 6379;
                }
            }
        }
        /// <summary>
        /// 设置主机名或 IP 地址及端口号
        /// </summary>
        /// <param name="host">主机名或 IP 地址的字符串表示形式</param>
        /// <param name="port">端口</param>
        public RedisEndPoint(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }
        /// <summary>
        /// 设置主机名或 IP 地址及端口号
        /// </summary>
        /// <param name="host">主机名或 IP 地址的字符串表示形式</param>
        /// <param name="port">端口</param>
        /// <param name="addressFamily">System.Net.Sockets.AddressFamily 值之一</param>
        public RedisEndPoint(string host, int port, AddressFamily addressFamily) : this(host, port)
        {
            this.AddressFamily = addressFamily;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取主机的主机名或 Internet 协议 (IP) 地址的字符串表示形式。
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; }
        /// <summary>
        /// System.Net.Sockets.AddressFamily 值
        /// </summary>
        public override AddressFamily AddressFamily { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 主机名或 IP 地址和端口号的字符串表示形式
        /// </summary>
        /// <returns>主机名或 IP 地址和端口号的字符串表示形式</returns>
        public override string ToString() => $"{this.Host}:{this.Port}";
        #endregion
    }
}