using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Protocol.Binary;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-20 09:05:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// MemcachedSocket客户端
    /// </summary>
    public class MemcachedSocketClient : Disposable, IMemcachedSocketClient
    {
        #region 构造器
        /// <summary>
        /// 设置网络端
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="endPoint">网络终节点</param>
        public MemcachedSocketClient(MemcachedConfig config, IPEndPoint endPoint)
        {
            this.Config = config;
            this.EndPoint = endPoint;
            this.Init();
        }
        /// <summary>
        /// 设置网络端
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="index">服务器索引</param>
        public MemcachedSocketClient(MemcachedConfig config, int index)
        {
            if (index < 0 || index >= config.Servers.Count) throw new ArgumentOutOfRangeException(nameof(index));
            this.Config = config;
            this.EndPoint = config.Servers[index];
            this.Init();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否认证
        /// </summary>
        public Boolean IsAuthenticated { get; set; }
        /// <summary>
        /// 网络终节点
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// 网络端
        /// </summary>
        public ISocketClient Socket { get; private set; }
        /// <summary>
        /// 配置
        /// </summary>
        public MemcachedConfig Config { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        public Boolean Connected => (bool)this.Socket?.Connected;
        #endregion

        #region 方法

        #region 认证
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<Boolean> AuthenticateAsync(string username, string password)
        {
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty()) return await Task.FromResult(this.IsAuthenticated = true);
            return await Task.FromResult(this.IsAuthenticated = true);
            if (this.Config.Protocol == MemcachedProtocol.Binary)
            {
                var request = new RequestPacket(this.Socket, this.Config, "")
                {
                    Opcode = Protocol.CommandType.SASLAuth,
                    Key = "PLAIN".GetBytes(this.Config.Encoding),
                    Value = $"\0{username}\0{password}".GetBytes(this.Config.Encoding)
                };
                var response = await request.GetResponseAsync().ConfigureAwait(false);
                if (response.Status == ResponseStatus.Success)
                {
                    request.Opcode = Protocol.CommandType.SASLStep;
                    request.Value = response.Value;
                    response = await request.GetResponseAsync().ConfigureAwait(false);
                    return this.IsAuthenticated = response.Status == ResponseStatus.Success;
                }
                return false;
            }
            else
            {
                var uname = $"{username} {password}";
                var line = $"set {uname.GetByteCount()}\r\n";
                await this.Socket.SendAsync(line).ConfigureAwait(false);
                await this.Socket.SendAsync($"{uname}\r\n").ConfigureAwait(false);
                var result = await this.Socket.ReceviceMessageAsync().ConfigureAwait(false);
                return this.IsAuthenticated = result.GetString().StartsWith("STORED");
            }
        }
        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> AuthenticateAsync() => await this.AuthenticateAsync(this.Config.UserName, this.Config.Password);
        #endregion

        #region 创建网络端
        /// <summary>
        /// 创建网络端
        /// </summary>
        /// <returns></returns>
        private SocketError CreateSocket()
        {
            this.Socket = new SocketClient(this.EndPoint)
            {
                ReceiveTimeout = this.Config.ReadTimeout,
                SendTimeout = this.Config.WriteTimeout,
                Encoding = this.Config.Encoding
            };
            if (this.Config.ConnectTimeout > 0)
                this.Socket.ConnectTimeout = this.Config.ConnectTimeout * 1000;
            if (this.Config.Certificates != null && this.Config.Certificates.Count > 0)
            {
                this.Socket.ClientCertificates = this.Config.Certificates;
                this.Socket.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            }
            this.Socket.OnClientError += (c, e, ex) =>
            {
                Console.WriteLine($"{c.EndPoint}出错[{ex.Message}].{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}");
            };
            this.Socket.OnStop += (c, e) =>
            {
                Console.WriteLine($"{c.EndPoint}已停止.{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}");
            };
            this.Socket.OnStart += (c, e) =>
            {
                Console.WriteLine($"{c.EndPoint}已启动.{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}");
            };
            return this.Socket.Connect();
        }
        #endregion

        #region 连接网络
        /// <summary>
        /// 连接网络
        /// </summary>
        /// <returns></returns>
        public SocketError Connect()
        {
            if (this.Socket == null)
            {
                var status = this.CreateSocket();
                if (status != SocketError.Success) return status;
            }
            if (this.Socket.Connected) return SocketError.Success;
            return this.Socket.Connect();
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            var status = this.CreateSocket();
            if (status == SocketError.Success)
            {
                this.IsAuthenticated = this.AuthenticateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                this.IsAuthenticated = false;
            }
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (this.Socket == null) return;
            this.Socket.Stop();
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">状态</param>
        protected override void Dispose(bool disposing)
        {
            this.Close();
            base.Dispose(disposing);
            GC.Collect();
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~MemcachedSocketClient() => this.Dispose(false);
        #endregion

        #endregion
    }
}