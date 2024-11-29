using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    #region NetSession
    /// <summary>
    /// NetSession
    /// </summary>
    public class NetSession : Disposable, INetSession
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public NetSession()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 频道
        /// </summary>
        public IList<String> Channel { get; set; }
        /// <summary>
        /// WS类型
        /// </summary>
        public WebSocketType WsType { get; set; } = WebSocketType.Null;
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// Header头信息
        /// </summary>
        public Dictionary<string, string> Header { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Header头信息
        /// </summary>
        public String Headers { get; set; }
        /// <summary>
        /// 是否是WebSocket
        /// </summary>
        public SocketTypes SocketType { get; set; } = SocketTypes.Socket;
        /// <summary>
        /// Socket数据类型
        /// </summary>
        public SocketDataType DataType { get; set; } = SocketDataType.String;
        /// <summary>
        /// 请求地址
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// 取消通知
        /// </summary>
        public CancellationTokenSource CancelToken { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// Socket
        /// </summary>
        public Socket ConnectionSocket { get; set; }
        /// <summary>
        /// 发送消息是否换行
        /// </summary>
        public Boolean IsNewLine { get; set; }
        /// <summary>
        /// 是否打包
        /// </summary>
        public Boolean IsDataMasked { get; set; }
        /// <summary>
        /// 4位操作码，定义有效负载数据，如果收到了一个未知的操作码，连接必须断开.
        /// </summary>
        public OpCode OpCode { get; set; } = OpCode.Text;
        /// <summary>
        /// ping时间戳
        /// </summary>
        public Int64 PingTimeStamp { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectionTime { get; set; }
        #endregion

        #region 方法

        #region 获取字节组
        /// <summary>
        /// 获取字节组
        /// </summary>
        /// <param name="content">数据</param>
        /// <returns></returns>
        public byte[] GetBytes(string content) => this.DataType == SocketDataType.HexString ? content.HexStringToBytes() : content.GetBytes(this.Encoding);
        #endregion

        #region 获取字符串
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public string GetString(byte[] bytes) => this.DataType == SocketDataType.HexString ? bytes.ToHexString() : bytes.GetString(this.Encoding);
        #endregion

        #region 连接状态
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public Boolean IsConnected(Socket socket = null)
        {
            if (socket == null) socket = this.ConnectionSocket;
            if (socket == null) return false;
            return !(!socket.Connected || (socket.Poll(1000, SelectMode.SelectRead) && (socket.Available == 0)));
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        public virtual Boolean Send(byte[] bytes)
        {
            if (bytes.IsNullOrEmpty() || this.ConnectionSocket == null || !this.ConnectionSocket.Connected) return false;
            try
            {
                this.ConnectionSocket.Send(bytes);
                return true;
            }
            catch (SocketException ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        public virtual Boolean Send(string message)
        {
            if (message.IsNullOrEmpty() || this.ConnectionSocket == null || !this.ConnectionSocket.Connected) return false;
            if (this.IsNewLine)
                message += Environment.NewLine;
            return this.Send(this.WsType != WebSocketType.Null &&
                 message.IndexOf("Sec-WebSocket-Key") == -1 ?
                 new DataFrame(message, this.Encoding, this.OpCode, this.DataType).GetBytes() :
                 this.GetBytes(message));
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">对象</param>
        /// <returns></returns>
        public virtual Boolean Send<T>(T data) => this.Send(data.ToJson());
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件地址</param>
        public virtual Boolean SendFile(string fileName)
        {
            if (this.ConnectionSocket.Connected)
                try
                {
                    this.ConnectionSocket?.SendFile(fileName);
                    return true;
                }
                catch (SocketException ex)
                {
                    LogHelper.Error(ex);
                    return false;
                }
            else return false;
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        public virtual void Close()
        {
            this.CancelToken.Cancel();
            if (this.IsConnected())
            {
                this.ConnectionSocket.Shutdown(SocketShutdown.Both);
                this.ConnectionSocket.Disconnect(false);
                this.ConnectionSocket.Close();
                this.ConnectionSocket.Dispose();
            }
        }
        #endregion

        #region 添加频道
        /// <summary>
        /// 添加频道
        /// </summary>
        /// <param name="channel">频道</param>
        public void AddChannel(string channel)
        {
            if (channel.IsNullOrEmpty()) return;
            if (this.Channel.IsNullOrEmpty()) this.Channel = new List<string>();
            if (this.Channel.Contains(channel, StringComparer.OrdinalIgnoreCase)) return;
            this.Channel.Add(channel);
        }
        /// <summary>
        /// 添加频道
        /// </summary>
        /// <param name="channels">频道</param>
        public void AddChannel(IEnumerable<String> channels)
        {
            if (channels.IsNullOrEmpty()) return;
            if (this.Channel.IsNullOrEmpty()) this.Channel = new List<string>();
            channels.Each(channel =>
            {
                if (this.Channel.Contains(channel, StringComparer.OrdinalIgnoreCase)) return;
                this.Channel.Add(channel);
            });
        }
        #endregion

        #region 移除频道
        /// <summary>
        /// 移除频道
        /// </summary>
        /// <param name="channel">频道</param>
        public void RemoveChannel(string channel)
        {
            if (channel.IsNullOrEmpty() || this.Channel.IsNullOrEmpty()) return;
            if (this.Channel.Contains(channel, StringComparer.OrdinalIgnoreCase)) this.Channel.Remove(channel);
        }
        /// <summary>
        /// 移除频道
        /// </summary>
        /// <param name="channels">频道</param>
        public void RemoveChannel(IEnumerable<String> channels)
        {
            if (channels.IsNullOrEmpty() || this.Channel.IsNullOrEmpty()) return;
            this.Channel.Each(channel =>
            {
                if (this.Channel.Contains(channel, StringComparer.OrdinalIgnoreCase)) this.Channel.Remove(channel);
            });
        }
        #endregion

        #endregion
    }
    #endregion
}