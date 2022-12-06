using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// <summary>
    /// ISession
    /// </summary>
    public interface INetSession
    {
        /// <summary>
        /// WS类型
        /// </summary>
        WebSocketType WsType { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// Header头信息
        /// </summary>
        String Headers { get; set; }
        /// <summary>
        /// 是否是WebSocket
        /// </summary>
        SocketTypes SocketType { get; set; }
        /// <summary>
        /// Socket数据类型
        /// </summary>
        SocketDataType DataType { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// 取消通知
        /// </summary>
        CancellationTokenSource CancelToken { get; set; }
        /// <summary>
        /// Socket
        /// </summary>
        Socket ConnectionSocket { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        DateTime ConnectionTime { get; set; }
        /// <summary>
        /// 4位操作码，定义有效负载数据，如果收到了一个未知的操作码，连接必须断开.
        /// </summary>
        OpCode OpCode { get; set; }
        /// <summary>
        /// 是否打包
        /// </summary>
        Boolean IsDataMasked { get; set; }
        /// <summary>
        /// ping时间戳
        /// </summary>
        Int64 PingTimeStamp { get; set; }
        /// <summary>
        /// 发送消息是否换行
        /// </summary>
        Boolean IsNewLine { get; set; }
        /// <summary>
        /// 获取字节组
        /// </summary>
        /// <param name="content">数据</param>
        /// <returns></returns>
        byte[] GetBytes(string content);
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        string GetString(byte[] bytes);
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        Boolean IsConnected(Socket socket = null);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        Boolean Send(byte[] bytes);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        Boolean Send(string message);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">对象</param>
        /// <returns></returns>
        bool Send<T>(T data);
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件地址</param>
        Boolean SendFile(string fileName);
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
        
    }
}