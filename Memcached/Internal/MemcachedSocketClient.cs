using System;
using System.Collections.Generic;
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
    /// MemcachedSocketClient 类说明
    /// </summary>
    public class MemcachedSocketClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedSocketClient()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否认证
        /// </summary>
        public Boolean IsAuthenticated { get; set; }
        /// <summary>
        /// 网络端
        /// </summary>
        public ISocketClient Socket { get; set; }
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
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<Boolean> AuthenticateAsync(string username, string password)
        {
            return this.IsAuthenticated = true;
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty()) return this.IsAuthenticated = true;
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
    }
}