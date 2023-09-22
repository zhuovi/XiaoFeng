using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-22 14:34:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// IMemcachedSocketClient 接口
    /// </summary>
    public interface IMemcachedSocketClient
    {
        /// <summary>
        /// 是否认证
        /// </summary>
        Boolean IsAuthenticated { get; set; }
        /// <summary>
        /// 网络端
        /// </summary>
        ISocketClient Socket { get; }
        /// <summary>
        /// 配置
        /// </summary>
        MemcachedConfig Config { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        Boolean Connected { get; }
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="username">帐号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<Boolean> AuthenticateAsync(string username, string password);
        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        Task<Boolean> AuthenticateAsync();
        /// <summary>
        /// 连接网络
        /// </summary>
        /// <returns></returns>
        SocketError Connect();
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
    }
}