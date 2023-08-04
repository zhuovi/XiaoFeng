﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-27 17:43:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// ISocketClient 类说明
    /// </summary>
    public interface ISocketClient : INetSocket
    {
        #region 构造器

        #endregion

        #region 属性
        /// <summary>
        /// 是否是服务端
        /// </summary>
        Boolean IsServer { get; }
        /// <summary>
        /// 获取已经从网络接收且可供读取的数据量
        /// </summary>
        int Available { get; }
        /// <summary>
        /// 连接状态
        /// </summary>
        Boolean Connected { get; }
        /// <summary>
        /// 证书名称
        /// </summary>
        string HostName { get; set; }
        /// <summary>
        /// 认证集合
        /// </summary>
        X509CertificateCollection ClientCertificates { get; set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        ConnectionType ConnectionType { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        Boolean? IsAuthenticated { get; }
        /// <summary>
        /// 请求头
        /// </summary>
        string RequestHeader { get; }
        #endregion

        #region 方法

        #region 连接
        /// <summary>
        /// 将客户端连接到指定主机上的指定端口。
        /// </summary>
        /// <param name="hostname">主机</param>
        /// <param name="port">端口</param>
        void Connect(string hostname, int port);
        /// <summary>
        /// 将客户端连接到指定主机上的指定端口
        /// </summary>
        /// <param name="address">主机</param>
        /// <param name="port">端口</param>
        void Connect(IPAddress address, int port);
        /// <summary>
        /// 将客户端连接到指定端点
        /// </summary>
        /// <param name="remoteEP">端点</param>
        void Connect(IPEndPoint remoteEP);
        /// <summary>
        /// 将客户端连接到指定主机上的指定端口
        /// </summary>
        /// <param name="ipAddresses">主机</param>
        /// <param name="port">端口</param>
        void Connect(IPAddress[] ipAddresses, int port);
        /// <summary>
        /// 将客户端连接到指定主机上的指定端口
        /// </summary>
        /// <param name="address">主机</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Task ConnectAsync(IPAddress address, int port);
        /// <summary>
        /// 将客户端连接到指定主机上的指定端口
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Task ConnectAsync(string host, int port);
        /// <summary>
        /// 将客户端连接到指定主机上的指定端口
        /// </summary>
        /// <param name="addresses">IP地址组</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Task ConnectAsync(IPAddress[] addresses, int port);
        /// <summary>
        /// 将客户端连接到指定端点
        /// </summary>
        /// <param name="remoteEP">端点</param>
        /// <returns></returns>
        Task ConnectAsync(IPEndPoint remoteEP);
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送ping
        /// </summary>
        /// <param name="buffers">数据</param>
        void SendPing(byte[] buffers = null);
        /// <summary>
        /// 异步发送ping
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <returns></returns>
        Task SendPingAsync(byte[] buffers = null);
        /// <summary>
        /// 发送pong
        /// </summary>
        /// <param name="buffers">数据</param>
        void SendPong(byte[] buffers = null);
        /// <summary>
        /// 异步发送pong
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <returns></returns>
        Task SendPongAsync(byte[] buffers = null);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffers">消息</param>
        /// <returns>发送字节长度 -1表示socket无连接 0表示无发送数据</returns>
        int Send(byte[] buffers);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffers">消息</param>
        /// <returns>发送字节长度 -1表示socket无连接 0表示无发送数据</returns>
        Task<int> SendAsync(byte[] buffers);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>发送字节长度 -1表示socket无连接 0表示无发送数据</returns>
        int Send(string message);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>发送字节长度 -1表示socket无连接 0表示无发送数据</returns>
        Task<int> SendAsync(string message);
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件</param>
        void SendFile(string fileName);
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <returns>Task</returns>
        Task SendFileAsync(string fileName);
        #endregion

        #region 接收数据
        /// <summary>
        /// 接收一条数据
        /// </summary>
        /// <returns>接收数据</returns>
        Task<byte[]> ReceviceMessageAsync();
        /// <summary>
        /// 循环接收数据
        /// </summary>
        /// <returns>Task</returns>
        Task ReceviceDataAsync();
        #endregion

        #region 获取网络流
        /// <summary>
        /// 获取网络流
        /// </summary>
        /// <returns>网络流</returns>
        NetworkStream GetStream();
        /// <summary>
        /// 获取SslStream
        /// </summary>
        /// <returns>SSL网络流</returns>
        Stream GetSslStream();
        #endregion

        #region 频道
        /// <summary>
        /// 添加订阅频道
        /// </summary>
        /// <param name="channel">频道</param>
        void AddChannel(params string[] channel);
        /// <summary>
        /// 移除订阅频道
        /// </summary>
        /// <param name="channel">频道</param>
        void RemoveChannel(params string[] channel);
        /// <summary>
        /// 清除所有频道
        /// </summary>
        void ClearChannel();
        /// <summary>
        /// 获取订阅频道
        /// </summary>
        /// <returns>订阅频道列表</returns>
        IList<string> GetChannel();
        /// <summary>
        /// 频道是否存在
        /// </summary>
        /// <param name="channel">频道</param>
        /// <returns>是否订阅过频道 true订阅过 false 未订阅</returns>
        Boolean ContainsChannel(params string[] channel);
        #endregion

        #region 设置Socket
        /// <summary>
        /// 连接socket
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="authentication">认证</param>
        /// <param name="certificate">ssl证书</param>
        void SetSocket(Socket socket, Func<ISocketClient, bool> authentication = null, X509Certificate certificate = null);
        #endregion

        #region 自定义数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>自定议数据</returns>
        object GetData(string key);
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        void AddData(string key, object value);
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">key</param>
        void RemoveData(string key);
        /// <summary>
        /// 清空数据
        /// </summary>
        void ClearData();
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否存在 true 存在 false 不存在</returns>
        Boolean ContainsData(string key);
        #endregion

        #region 事件回调
        /// <summary>
        /// 客户端事件回调
        /// </summary>
        /// <param name="e">错误信息</param>
        void ClientErrorEventHandler(Exception e);
        /// <summary>
        /// 接收消息事件回调
        /// </summary>
        /// <param name="message">消息</param>
        void MessageEventHandler(string message);
        /// <summary>
        /// 接收消息事件回调
        /// </summary>
        /// <param name="message">消息</param>
        void MessageByteEventHandler(byte[] message);
        /// <summary>
        /// 认证事件回调
        /// </summary>
        /// <param name="message">消息</param>
        void AuthenticationEventHandler(string message);
        #endregion

        #endregion
    }
}