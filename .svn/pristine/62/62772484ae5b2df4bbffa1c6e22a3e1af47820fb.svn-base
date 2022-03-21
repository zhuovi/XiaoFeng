using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络服务端接口
    /// </summary>
    public interface INetServer : ISocket
    {
        #region 事件
        /// <summary>
        /// 新的连接事件
        /// </summary>
        event NewConnectionEventHandler OnNewConnection;
        /// <summary>
        /// 接收消息事件
        /// </summary>
        event MessageEventHandler OnMessage;
        /// <summary>
        /// 接收消息事件
        /// </summary>
        event MessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 断开连接事件
        /// </summary>
        event DisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 停止服务事件
        /// </summary>
        event StopEventHandler OnStop;
        /// <summary>
        /// 出错事件
        /// </summary>
        event ErrorEventHandler OnError;
        /// <summary>
        /// 客户端错误事件
        /// </summary>
        event SessionErrorEventHandler OnClientError;
        /// <summary>
        /// 服务器启动事件
        /// </summary>
        event StartEventHandler OnStart;
        #endregion

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        void SendFile(string fileName);
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="session">连接</param>
        void SendFile(string fileName, IServerSession session);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        void Send(string message);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="socket">连接</param>
        void Send(string message, Socket socket);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="session">连接</param>
        void Send(string message, IServerSession session);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        void Send(byte[] bytes);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        /// <param name="socket">连接</param>
        void Send(byte[] bytes, Socket socket);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        /// <param name="session">连接</param>
        void Send(byte[] bytes, IServerSession session);
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="session"></param>
        void AddQueue(IServerSession session);
        /// <summary>
        /// 移除队列
        /// </summary>
        /// <param name="session">会话</param>
        void RemoveQueue(INetSession session);
        /// <summary>
        /// 移除队列
        /// </summary>
        /// <param name="endPoint">网络地址</param>
        void RemoveQueue(IPEndPoint endPoint);
        /// <summary>
        /// 清空队列
        /// </summary>
        void ClearQueue();
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <param name="socket">连接</param>
        /// <returns></returns>
        IServerSession GetQueue(Socket socket);
        /// <summary>
        /// 获取在线列表中的客户端
        /// </summary>
        /// <param name="func">满足条件的函数</param>
        /// <returns></returns>
        IServerSession GetQueue(Func<IServerSession, bool> func);
        /// <summary>
        /// 获取队列数
        /// </summary>
        /// <returns></returns>
        int CountQueue();
        /// <summary>
        /// 复制出一个在线列表
        /// </summary>
        /// <returns></returns>
        IServerSession[] GetData();
        /// <summary>
        /// 批量加入黑名单
        /// </summary>
        /// <param name="list">列表</param>
        void BulkAddIpBlack(List<string> list);
        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        void AddIpBlack(string ip);
        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        void RemoveIpBlack(string ip);
        /// <summary>
        /// 清空黑名单
        /// </summary>
        void ClearIpBlack();
        /// <summary>
        /// 是否在黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        Boolean IsBlackIP(string ip);
        /// <summary>
        /// 关闭在线列表中的客户端
        /// </summary>
        /// <param name="func">满足条件的函数</param>
        void Close(Func<IServerSession, bool> func);
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="session">连接</param>
        void Close(IServerSession session = null);
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="endPoint">IP节点</param>
        void Close(IPEndPoint endPoint);
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="endPoint">IP节点</param>
        void Close(EndPoint endPoint);
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="ip">客户端IP</param>
        /// <param name="port">端口</param>
        void Close(string ip, string port);
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="ip">客户端IP</param>
        /// <param name="port">端口</param>
        void Close(string ip, int port);
        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="endPoint">IP和port</param>
        /// <returns></returns>
        Boolean IsConnected(IPEndPoint endPoint);

    }
}