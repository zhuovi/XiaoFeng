using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-28 08:52:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// Socket服务端 接口
    /// </summary>
    public interface ISocketServer : INetSocket
    {
        #region 属性
        /// <summary>
        /// 客户端列表 <see cref="ISocketClient"/> 集合
        /// </summary>
        ICollection<ISocketClient> Clients { get; }
        /// <summary>
        /// SSL证书
        /// </summary>
        X509Certificate Certificate { get; set; }
        /// <summary>
        /// 挂起连接队列的最大长度
        /// </summary>
        int ListenCount { get; set; }
        /// <summary>
        /// 是否允许将套接字绑定到已在使用中的地址。
        /// </summary>
        Boolean ReuseAddress { get; set; }
        /// <summary>
        /// 验证Socket请求的合法性
        /// </summary>
        Func<ISocketClient, Boolean> Authentication { get; set; }
        /// <summary>
        /// 是否自动Pong
        /// </summary>
        Boolean IsPong { get; set; }
        /// <summary>
        /// pong间隔 单位为秒
        /// </summary>
        int PongTime { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 新客户端连接事件
        /// </summary>
        event OnNewConnectionEventHandler OnNewConnection;
        /// <summary>
        /// 客户端断开连接事件
        /// </summary>
        event OnDisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 出错事件
        /// </summary>
        event OnErrorEventHandler OnError;
        #endregion

        #region 事件回调
        /// <summary>
        /// 新客户端连接事件回调
        /// </summary>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        void NewConnectionEventHandler(ISocketClient client);
        /// <summary>
        /// 客户端断开事件回调
        /// </summary>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        void DisconnectedEventHandler(ISocketClient client);
        /// <summary>
        /// 客户端出错事件回调
        /// </summary>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        /// <param name="endPoint">IP节点 <see cref="IPEndPoint"/></param>
        /// <param name="e">错误</param>
        void ClientErrorEventHandler(ISocketClient client, IPEndPoint endPoint, Exception e);
        /// <summary>
        /// 接收到客户端消息回调
        /// </summary>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        /// <param name="message">消息</param>
        void MessageEventHandler(ISocketClient client, string message);
        /// <summary>
        /// 接收到客户端消息回调
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="message">消息</param>
        void MessageByteEventHandler(ISocketClient client, byte[] message);
        /// <summary>
        /// 认证事件回调
        /// </summary>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        /// <param name="message">消息</param>
        void AuthenticationEventHandler(ISocketClient client, string message);
        #endregion

        #region 启动
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="backlog">挂起连接队列的最大长度</param>
        void Start(int backlog);
        #endregion

        #region 黑名单
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <param name="ips">ip集合</param>
        void AddBlack(params string[] ips);
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <param name="ips">ip集合</param>
        void AddBlack(IEnumerable<string> ips);
        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        void RemoveBlack(string ip);
        /// <summary>
        /// 清空黑名单
        /// </summary>
        void ClearBlack();
        /// <summary>
        /// 是否存在黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>是否包含当前指定的ip
        /// <para><term>true</term>  包含 </para>
        /// <para><term>false</term> 不包含</para>
        /// </returns>
        Boolean ContainsBlack(string ip);
        /// <summary>
        /// 是否存在黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>是否包含当前指定的ip
        /// <para><term>true</term>  包含 </para>
        /// <para><term>false</term> 不包含</para>
        /// </returns>
        Boolean ContainsBlack(long ip);
        /// <summary>
        /// 获取黑名单列表
        /// </summary>
        /// <returns>黑名单列表</returns>
        ICollection<string> GetBlackList();
        /// <summary>
        /// 群发消息
        /// </summary>
        /// <param name="message">消息</param>
        void Send(string message);
        /// <summary>
        /// 群发数据
        /// </summary>
        /// <param name="buffers">数据</param>
        void Send(byte[] buffers);
        /// <summary>
        /// 针对客户端发送消息
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        void Send(byte[] buffers, ISocketClient client);
        /// <summary>
        /// 针对客户端发送消息
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">长度</param>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        void Send(byte[] buffers, int offset, int count, ISocketClient client);
        /// <summary>
        /// 针对频道客户端发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="channel">频道</param>
        void Send(byte[] buffers, params string[] channel);
        /// <summary>
        /// 针对特定的key value的客户端发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        void Send(byte[] buffers, string key, object value);
        /// <summary>
        /// 异步群发消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>Task</returns>
        Task SendAsync(string message);
        /// <summary>
        /// 异步群发数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <returns>Task</returns>
        Task SendAsync(byte[] buffers);
        /// <summary>
        /// 异步针对客户端发送消息
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        /// <returns>Task</returns>
        Task SendAsync(byte[] buffers, ISocketClient client);
        /// <summary>
        /// 异步针对客户端发送消息
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">长度</param>
        /// <param name="client">客户端 <see cref="ISocketClient"/></param>
        /// <returns>Task</returns>
        Task SendAsync(byte[] buffers, int offset, int count, ISocketClient client);
        /// <summary>
        /// 异步针对频道客户端发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="channel">频道</param>
        /// <returns>Task</returns>
        Task SendAsync(byte[] buffers, params string[] channel);
        /// <summary>
        /// 异步针对特定的key value的客户端发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>Task</returns>
        Task SendAsync(byte[] buffers, string key, object value);
        #endregion
    }
}