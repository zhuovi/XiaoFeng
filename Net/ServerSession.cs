﻿using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
    #region 服务端Session
    /// <summary>
    /// 服务端Session
    /// </summary>
    public class ServerSession : NetSession, IServerSession
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ServerSession()
        {
            this.ReceivedDataBuffer = new byte[MaxBufferSize];
            this.FirstByte = new byte[MaxBufferSize];
            this.LastByte = new byte[MaxBufferSize];
            this.FirstByte[0] = 0x00;
            this.LastByte[0] = 0xFF;
        }
        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public String GroupID { get; set; }
        /// <summary>
        /// 最长长度
        /// </summary>
        private readonly int MaxBufferSize = 1024 * 100;
        /// <summary>
        /// 握手
        /// </summary>
        private string _Handshake = "";
        /// <summary>
        /// 握手
        /// </summary>
        private string Handshake
        {
            get
            {
                if (this._Handshake.IsNullOrEmpty())
                {
                    this._Handshake = "HTTP/1.1 101 Web Socket Protocol Handshake" + Environment.NewLine;
                    this._Handshake += "Connection: Upgrade" + Environment.NewLine;
                    this._Handshake += "Upgrade: WebSocket" + Environment.NewLine;
                    this._Handshake += "Date: " + DateTime.Now.ToString(@"ddd, dd-MMM-yyyy HH:mm:ss.fff 'GMT'zzz", System.Globalization.CultureInfo.GetCultureInfo("en-US")) + Environment.NewLine;
                    this._Handshake += "Server: FayElf v1.0(" + this.OSName + ")" + Environment.NewLine;
                    this._Handshake += "Sec-WebSocket-Origin: " + "{0}" + Environment.NewLine;
                    //this._Handshake += string.Format("Sec-WebSocket-Location: " + "ws://{0}:{1}/" + Environment.NewLine, this.EndPoint.ToString(), this.Port);
                    this._Handshake += Environment.NewLine;
                }
                return this._Handshake;
            }
            set { this._Handshake = value; }
        }
        /// <summary>
        /// 新握手
        /// </summary>
        private string _NewHandshake = "";
        /// <summary>
        /// 新握手
        /// </summary>
        private string NewHandshake
        {
            get
            {
                if (this._NewHandshake.IsNullOrEmpty())
                {
                    this._NewHandshake = "HTTP/1.1 101 Switching Protocols" + Environment.NewLine;
                    this._NewHandshake += "Connection: Upgrade" + Environment.NewLine;
                    this._NewHandshake += "Upgrade: WebSocket" + Environment.NewLine;
                    this._NewHandshake += "Date: " + DateTime.Now.ToString(@"ddd, dd-MMM-yyyy HH:mm:ss.fff 'GMT'zzz", System.Globalization.CultureInfo.GetCultureInfo("en-US")) + Environment.NewLine;
                    this._NewHandshake += "Server: FayElf v1.0(" + this.OSName + ")" + Environment.NewLine;
                    this._NewHandshake += "Author: Jacky(QQ:7092734,Email:jacky@fayelf.com,Site:www.fayelf.com)" + Environment.NewLine;
                    this._NewHandshake += "Copyright: 未经授权禁止使用,盗版必究." + Environment.NewLine;
                    this._NewHandshake += "Sec-WebSocket-Accept: {0}" + Environment.NewLine;
                    this._NewHandshake += Environment.NewLine;
                }
                return this._NewHandshake;
            }
            set { this._NewHandshake = value; }
        }
        /// <summary>
        /// 系统名称
        /// </summary>
        private string OSName => OS.Platform.GetOSPlatform().ToString();
        /// <summary>
        /// 接收数据
        /// </summary>
        public byte[] ReceivedDataBuffer { get; set; }
        /// <summary>
        /// 第一个字节
        /// </summary>
        private readonly byte[] FirstByte;
        /// <summary>
        /// 最后字节
        /// </summary>
        private readonly byte[] LastByte;
        /// <summary>
        /// 服务key1
        /// </summary>
        private byte[] ServerKey1;
        /// <summary>
        /// 服务key2
        /// </summary>
        private byte[] ServerKey2;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 协议版本
        /// </summary>
        public SslProtocols SslProtocols { get; set; }
        /// <summary>
        /// SSL证书
        /// </summary>
        public X509Certificate Certificate { get; set; }
        /// <summary>
        /// 读超时
        /// </summary>
        public int ReadTimeout { get; set; }
        /// <summary>
        /// 写超时
        /// </summary>
        public int WriteTimeout { get; set; }
        /// <summary>
        /// SSL流
        /// </summary>
        public SslStream SslStream { get; set; }
        #endregion

        #region 委托
        /// <summary>
        /// 连接委托
        /// </summary>
#pragma warning disable CS0067 // 从不使用事件“ServerSession.OnNewConnection”
        public virtual event NewConnectionEventHandler OnNewConnection;
#pragma warning restore CS0067 // 从不使用事件“ServerSession.OnNewConnection”
        /// <summary>
        /// 消息委托
        /// </summary>
        public virtual event MessageEventHandler OnMessage;
        /// <summary>
        /// 消息委托
        /// </summary>
        public virtual event MessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 断开连接委托
        /// </summary>
        public virtual event DisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 出错委托
        /// </summary>
        public virtual event SessionErrorEventHandler OnSessionError;
        /// <summary>
        /// 连接认证
        /// </summary>
        public virtual Func<ServerSession, bool> SocketAuth { get; set; }
        /// <summary>
        /// 设置是Socket类型
        /// </summary>
        public virtual Action<ServerSession> SetSocketType { get; set; }
        #endregion

        #region 方法

        #region 处理接受Socket的数据
        /// <summary>
        /// 处理接受Socket的数据
        /// </summary>
        /// <param name="state">state</param>
        private void ReceiveSocketData(IAsyncResult state)
        {
            Socket SocketClient = this.ConnectionSocket;
            if (SocketClient == null || !SocketClient.Connected) return;
            if (this.CancelToken.IsCancellationRequested)
            {
                //SocketException ex = new SocketException();
                //LogHelper.Error(ex, "取消连接[Socket]");
                lock (this.ConnectionSocket)
                {
                    if (this.ConnectionSocket == null || this.ConnectionSocket.RemoteEndPoint == null) return;
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                }
                return;
            }
            try
            {
                string IP = SocketClient.RemoteEndPoint.ToString();
                int length = SocketClient.EndReceive(state);
                byte[] buffer = this.ReceivedDataBuffer;
                if (length == 0)
                {
                    if (!this.IsConnected(SocketClient))
                    {
                        lock (this.ConnectionSocket)
                        {
                            if (this.ConnectionSocket == null || this.ConnectionSocket.RemoteEndPoint == null) return;
                            /*断开连接*/
                            this.OnDisconnected?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    return;
                }
                else/*开始接收数据*/
                {
                    SocketClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveSocketData), this);
                }
                string msg;
                if (this.DataType == SocketDataType.HexString)
                {
                    msg = buffer.ToHexString(0, length);
                }
                else
                {
                    msg = buffer.GetString(this.Encoding, 0, length);
                }
                if (msg.StartsWith("SUBSCRIBE:", StringComparison.OrdinalIgnoreCase))
                {
                    this.AddChannel(msg.RemovePattern(@"^SUBSCRIBE:").Split(','));
                }
                else if (msg.StartsWith("UNSUBSCRIBE", StringComparison.OrdinalIgnoreCase))
                {
                    this.RemoveChannel(msg.RemovePattern(@"^UNSUBSCRIBE:").Split(','));
                }
                else
                {
                    /*websocket建立连接的时候,除了TCP连接的三次握手,websocket协议中客户端与服务器想建立连接需要一次额外的握手动作*/
                    OnMessage?.Invoke(this, msg, EventArgs.Empty);
                    OnMessageByte?.Invoke(this, this.GetBytes(msg), EventArgs.Empty);
                    /*if (OnMessageByte != null)
                    {
                        byte[] msgBytes = new byte[length];
                        Buffer.BlockCopy(ReceivedDataBuffer, 0, msgBytes, 0, length);
                        OnMessageByte?.Invoke(this, msgBytes, EventArgs.Empty);
                    }*/
                }
            }
            catch (SocketException ex)
            {
                LogHelper.Error(ex, "接收信息失败[socket]");
                lock (this.ConnectionSocket)
                {
                    /*断开连接*/
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                    /*出错消息*/
                    OnSessionError?.Invoke(this, ex);
                }
            }
        }
        #endregion

        #region 处理接受WebSocket的数据
        /// <summary>
        /// 处理接受WebSocket的数据
        /// </summary>
        /// <param name="status">状态</param>
        private void ReceiveWebSocketData(IAsyncResult status)
        {
            if (!ConnectionSocket.Connected) return;
            if (this.CancelToken.IsCancellationRequested)
            {
                SocketException ex = new SocketException();
                LogHelper.Error(ex, "取消连接[WebSocket]");
                lock (this.ConnectionSocket)
                {
                    if (this.ConnectionSocket == null || this.ConnectionSocket.RemoteEndPoint == null) return;
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                }
                return;
            }
            string messageReceived = string.Empty;
            DataFrame dr = new DataFrame(ReceivedDataBuffer)
            {
                Encoding = this.Encoding,
                DataType = this.DataType
            };
            this.OpCode = dr.Opcode;
            if (this.OpCode == OpCode.Ping)
            {
                this.Send(new DataFrame(dr.Text, this.Encoding, OpCode.Pong, this.DataType).GetBytes());
                return;
            }
            try
            {
                var EndIndex = 0;
                if (!this.IsDataMasked)
                {
                    /*Web Socket protocol: messages are sent with 0x00 and 0xFF as padding bytes*/
                    UTF8Encoding decoder = new UTF8Encoding();
                    int startIndex = 0;
                    int endIndex = 0;
                    /*搜索开始字节*/
                    while (ReceivedDataBuffer[startIndex] == FirstByte[0]) startIndex++;
                    /*搜索结束字节*/
                    endIndex = startIndex + 1;
                    while (ReceivedDataBuffer[endIndex] != LastByte[0] && endIndex != MaxBufferSize - 1) endIndex++;
                    if (endIndex == MaxBufferSize - 1) endIndex = MaxBufferSize;
                    EndIndex = endIndex;
                    /*获取消息*/
                    if (this.DataType == SocketDataType.HexString)
                        messageReceived = ReceivedDataBuffer.ToHexString(startIndex, endIndex - startIndex);
                    else
                        messageReceived = ReceivedDataBuffer.GetString(this.Encoding, startIndex, endIndex - startIndex);
                }
                else
                {
                    messageReceived = dr.Text;
                    EndIndex = (messageReceived.GetBytes() ?? new byte[0]).Length;
                }
                if (messageReceived.Length == 0 || messageReceived.Length == MaxBufferSize && messageReceived[0] == Convert.ToChar(65533))
                {
                    if (this.ConnectionSocket != null)
                    {
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    OnMessage?.Invoke(this, messageReceived, EventArgs.Empty);
                    OnMessageByte?.Invoke(this, this.GetBytes(messageReceived), EventArgs.Empty);
                    /*if (OnMessageByte != null)
                    {
                        byte[] msgBytes = new byte[EndIndex];
                        Buffer.BlockCopy(ReceivedDataBuffer, 0, msgBytes, 0, EndIndex);
                        OnMessageByte?.Invoke(this, msgBytes, EventArgs.Empty);
                    }*/
                    Array.Clear(ReceivedDataBuffer, 0, ReceivedDataBuffer.Length);
                    if (ConnectionSocket == null || !ConnectionSocket.Connected)
                    {
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                    ConnectionSocket.BeginReceive(ReceivedDataBuffer,
                        0,
                        ReceivedDataBuffer.Length,
                        0,
                        new AsyncCallback(ReceiveWebSocketData),
                        null);
                }
            }
            catch (SocketException ex)
            {
                LogHelper.Error(ex, "接收信息失败[WebSocket]");
                lock (this.ConnectionSocket)
                {
                    if (this.ConnectionSocket == null || this.ConnectionSocket.RemoteEndPoint == null) return;
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                    OnSessionError?.Invoke(this, ex);
                }
            }
        }
        #endregion

        #region 处理Key
        /// <summary>
        /// 处理Key
        /// </summary>
        /// <param name="keyNum">key</param>
        /// <param name="clientKey">客户端Key</param>
        private void BuildServerPartialKey(int keyNum, string clientKey)
        {
            string partialServerKey = "";
            byte[] currentKey;
            int spacesNum = 0;
            char[] keyChars = clientKey.ToCharArray();
            foreach (char currentChar in keyChars)
            {
                if (char.IsDigit(currentChar)) partialServerKey += currentChar;
                if (char.IsWhiteSpace(currentChar)) spacesNum++;
            }
            try
            {
                currentKey = BitConverter.GetBytes((int)(Int64.Parse(partialServerKey) / spacesNum));
                if (BitConverter.IsLittleEndian) Array.Reverse(currentKey);
                if (keyNum == 1) ServerKey1 = currentKey;
                else ServerKey2 = currentKey;
            }
            catch
            {
                if (ServerKey1 != null) Array.Clear(ServerKey1, 0, ServerKey1.Length);
                if (ServerKey2 != null) Array.Clear(ServerKey2, 0, ServerKey2.Length);
            }
        }
        #endregion

        #region 处理服务器Key
        /// <summary>
        /// 处理服务器Key
        /// </summary>
        /// <param name="last8Bytes">最后8个字节</param>
        /// <returns></returns>
        private byte[] BuildServerFullKey(byte[] last8Bytes)
        {
            byte[] concatenatedKeys = new byte[16];
            Array.Copy(ServerKey1, 0, concatenatedKeys, 0, 4);
            Array.Copy(ServerKey2, 0, concatenatedKeys, 4, 4);
            Array.Copy(last8Bytes, 0, concatenatedKeys, 8, 8);
            /*MD5 Hash*/
            MD5 MD5Service = MD5.Create();
            return MD5Service.ComputeHash(concatenatedKeys);
        }
        #endregion

        #region 握手信息
        /// <summary>
        /// 握手信息
        /// </summary>
        /// <param name="status">状态</param>
        public void ManageHandshake(IAsyncResult status)
        {
            string header = "Sec-WebSocket-Version:";
            int HandshakeLength = (int)status.AsyncState;
            byte[] last8Bytes = new byte[8];

            UTF8Encoding decoder = new UTF8Encoding();
            string RawClientHandshake = "";
            /*if (HandshakeLength > 0)
                RawClientHandshake = decoder.GetString(ReceivedDataBuffer, 0, HandshakeLength);
            else
                RawClientHandshake = decoder.GetString(ReceivedDataBuffer);*/

            if (HandshakeLength == 0) HandshakeLength = ReceivedDataBuffer.Length;
            RawClientHandshake = decoder.GetString(ReceivedDataBuffer, 0, HandshakeLength);
            this.Headers = RawClientHandshake;
            if (!RawClientHandshake.Contains("Sec-WebSocket-Key"))
            {
                this.SocketType = SocketTypes.Socket;
                /*验证用户的合法性 */
                if (this.SocketAuth != null)
                {
                    if (!this.SocketAuth(this))
                    {
                        if (this.OnDisconnected != null &&
                            this.ConnectionSocket != null &&
                            this.ConnectionSocket.RemoteEndPoint != null)
                            this.OnDisconnected(this, EventArgs.Empty);
                        return;
                    }
                }
                /*if (this.SetSocketType != null) this.SetSocketType(this);*/
                this.ReceiveSocketData(status);
                return;
            }
            else
            {
                this.SocketType = SocketTypes.WebSocket;
                /*验证用户的合法性 */
                if (this.SocketAuth != null)
                {
                    if (!this.SocketAuth(this))
                    {
                        if (this.OnDisconnected != null && this.ConnectionSocket != null && this.ConnectionSocket.RemoteEndPoint != null)
                            this.OnDisconnected(this, EventArgs.Empty);
                        return;
                    }
                }
            }
            if (HandshakeLength == 0 || HandshakeLength - 8 < 0) return;

            /*现在使用的是比较新的WebSocket协议*/
            if (RawClientHandshake.IndexOf(header) != -1)
            {
                this.IsDataMasked = true;
                string[] rawClientHandshakeLines = RawClientHandshake.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string acceptKey = "";
                foreach (string Line in rawClientHandshakeLines)
                {
                    if (Line.Contains("Sec-WebSocket-Key:"))
                    {
                        acceptKey = ComputeWebSocketHandshakeSecurityHash09(Line.Substring(Line.IndexOf(":") + 2));
                        break;
                    }
                }
                NewHandshake = NewHandshake.format(acceptKey);
                byte[] NewHandshakeText = NewHandshake.GetBytes(Encoding.UTF8);
                if (ConnectionSocket.Connected)
                    ConnectionSocket.BeginSend(NewHandshakeText, 0, NewHandshakeText.Length, 0, HandshakeFinished, null);
                return;
            }
            Array.Copy(ReceivedDataBuffer, HandshakeLength - 8, last8Bytes, 0, 8);
            string ClientHandshake = decoder.GetString(ReceivedDataBuffer, 0, HandshakeLength - 8);
            string[] ClientHandshakeLines = ClientHandshake.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            /*连接成功*/
            //if (OnConnection != null) OnConnection(this, EventArgs.Empty);
            ClientHandshakeLines.Each(Line =>
            {
                if (Line.Contains("Sec-WebSocket-Key1:"))
                    BuildServerPartialKey(1, Line.Substring(Line.IndexOf(":") + 2));
                if (Line.Contains("Sec-WebSocket-Key2:"))
                    BuildServerPartialKey(2, Line.Substring(Line.IndexOf(":") + 2));
                if (Line.Contains("Origin:"))
                    try
                    {
                        this.Handshake = this.Handshake.format(Line.Substring(Line.IndexOf(":") + 2));
                    }
                    catch
                    {
                        this.Handshake = this.Handshake.format("null");
                    }
            });
            /*创建响应头信息*/
            byte[] HandshakeText = Handshake.GetBytes(Encoding.UTF8);
            byte[] serverHandshakeResponse = new byte[HandshakeText.Length + 16];
            byte[] serverKey = BuildServerFullKey(last8Bytes);
            Array.Copy(HandshakeText, serverHandshakeResponse, HandshakeText.Length);
            Array.Copy(serverKey, 0, serverHandshakeResponse, HandshakeText.Length, 16);
            /*发送握手信息*/
            lock (ConnectionSocket)
            {
                ConnectionSocket?.BeginSend(
                        serverHandshakeResponse,
                        0,
                        HandshakeText.Length + 16,
                        0,
                        HandshakeFinished,
                        null
                        );
            }
        }
        #endregion

        #region 打包请求连接数据
        /// <summary>
        /// 打包请求连接数据
        /// </summary>
        /// <param name="secWebSocketKey">客户请求头信息</param>
        /// <returns></returns>
        private string ComputeWebSocketHandshakeSecurityHash09(String secWebSocketKey)
        {
            const string MagicKEY = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            string ret = secWebSocketKey.Trim(new char[] { '\r', '\n' }) + MagicKEY;
            //SHA1 sha = new SHA1CryptoServiceProvider();
            SHA1 sha = SHA1.Create();
            byte[] sha1Hash = sha.ComputeHash(ret.GetBytes(this.Encoding));
            return Convert.ToBase64String(sha1Hash);
        }
        #endregion

        #region 握手完成信息
        /// <summary>
        /// 握手完成信息
        /// </summary>
        /// <param name="status">状态</param>
        private void HandshakeFinished(IAsyncResult status)
        {
            try
            {
                ConnectionSocket.EndSend(status);
                ConnectionSocket?.BeginReceive(ReceivedDataBuffer, 0, ReceivedDataBuffer.Length, 0, new AsyncCallback(ReceiveWebSocketData), null);
                //OnNewConnection?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "完成握手接收失败");
            }
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        public override Boolean Send(byte[] bytes)
        {
            if (bytes.IsNullOrEmpty() || this.ConnectionSocket == null || !this.ConnectionSocket.Connected) return false;
            try
            {
                if (this.SocketType == SocketTypes.WebSocket)
                {
                    if (this.IsDataMasked)
                    {
                        this.ConnectionSocket?.Send(new DataFrame(bytes) { Encoding = this.Encoding, DataType = this.DataType, Opcode = this.OpCode }.GetBytes());
                    }
                    else
                    {
                        this.ConnectionSocket?.Send(FirstByte);
                        this.ConnectionSocket?.Send(bytes);
                        this.ConnectionSocket?.Send(LastByte);
                    }
                }
                else
                {
                    this.ConnectionSocket?.Send(bytes);
                }
                return true;
            }
            catch (SocketException ex)
            {
                OnSessionError?.Invoke(this, ex);
                this.Close();
                return false;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        public override Boolean Send(string message)
        {
            if (message.RemovePattern(@"[\r\n\s]+").IsNullOrEmpty()) return false;
            if (message.IsNullOrEmpty() || this.ConnectionSocket == null || !this.ConnectionSocket.Connected)
            {
                this.Close();
                return false;
            }
            try
            {
                if (this.IsNewLine)
                    message += Environment.NewLine;
                if (this.SocketType == SocketTypes.WebSocket)
                {
                    if (this.IsDataMasked)
                    {
                        this.ConnectionSocket?.Send(new DataFrame(message, this.Encoding, this.OpCode, this.DataType).GetBytes());
                    }
                    else
                    {
                        this.ConnectionSocket?.Send(FirstByte);
                        this.ConnectionSocket?.Send(this.GetBytes(message));
                        this.ConnectionSocket?.Send(LastByte);
                    }
                }
                else
                {
                    this.ConnectionSocket?.Send(this.GetBytes(message));
                }
                return true;
            }
            catch (SocketException ex)
            {
                OnSessionError?.Invoke(this, ex);
                this.Close();
                return false;
            }
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        public override void Close()
        {
            base.Close();
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            var netStream = new NetworkStream(this.ConnectionSocket);
            if (this.ReadTimeout > 0) netStream.ReadTimeout = this.ReadTimeout;
            if (this.WriteTimeout > 0) netStream.WriteTimeout = this.WriteTimeout;
            if (this.SslProtocols != SslProtocols.None && this.Certificate.IsNotNullOrEmpty())
            {
                this.SslStream = new SslStream(netStream, false, (sender, cert, chain, errors) =>
                {

                    return errors == SslPolicyErrors.None;
                }, (sender, targethost, localcert, remotecert, acceptableissuers) =>
                {
                    return this.Certificate;
                }, EncryptionPolicy.RequireEncryption);
                this.SslStream = new SslStream(netStream, true)
                {
                    ReadTimeout = netStream.ReadTimeout,
                    WriteTimeout = netStream.WriteTimeout
                };
                this.SslStream.AuthenticateAsServer(this.Certificate, false, this.SslProtocols, true);
                //this.SslStream.AuthenticateAsServer(this.Certificate);
                this.ReceiveMessage(this.SslStream, true);
            }
            else
                this.ReceiveMessage(netStream, true);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="first">是否是第一次连接</param>
        private void ReceiveMessage(Stream stream, Boolean first = false)
        {
            if (stream == null) return;
            int readsize = -1;
            var buffer = new MemoryStream();
            do
            {
                readsize = stream.Read(this.ReceivedDataBuffer, 0, this.ReceivedDataBuffer.Length);
                if (readsize <= 0) break;
                buffer.Write(this.ReceivedDataBuffer, 0, readsize);
            } while (readsize > 0);
            if (buffer.Length == 0) return;
            var bytes = buffer.ToArray();
            buffer.Close();
            buffer.Dispose();
            if (first)
            {
                /*验证用户的合法性 */
                if (this.SocketAuth != null && !this.SocketAuth(this))
                {
                    if (this.OnDisconnected != null &&
                        this.ConnectionSocket != null &&
                        this.ConnectionSocket.RemoteEndPoint != null)
                        this.OnDisconnected(this, EventArgs.Empty);
                    return;
                }
                var msg = bytes.GetString(this.Encoding);
                if (msg.Contains("Sec-WebSocket-Key"))
                {
                    this.SocketType = SocketTypes.WebSocket;
                    string header = "Sec-WebSocket-Version:";
                    /*现在使用的是比较新的WebSocket协议*/
                    if (msg.IndexOf(header) != -1)
                    {
                        this.IsDataMasked = true;
                        string[] rawClientHandshakeLines = msg.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        string acceptKey = "";
                        foreach (string Line in rawClientHandshakeLines)
                        {
                            if (Line.Contains("Sec-WebSocket-Key:"))
                            {
                                acceptKey = ComputeWebSocketHandshakeSecurityHash09(Line.Substring(Line.IndexOf(":") + 2));
                                break;
                            }
                        }
                        NewHandshake = NewHandshake.format(acceptKey);
                        byte[] NewHandshakeText = NewHandshake.GetBytes(Encoding.UTF8);
                        stream.Write(NewHandshakeText, 0, NewHandshakeText.Length);
                        return;
                    }
                    var last8Bytes = new byte[8];
                    Array.Copy(bytes, bytes.Length - 8, last8Bytes, 0, 8);
                    string ClientHandshake = bytes.GetString(this.Encoding, 0, bytes.Length - 8);
                    string[] ClientHandshakeLines = ClientHandshake.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    /*连接成功*/
                    ClientHandshakeLines.Each(Line =>
                    {
                        if (Line.Contains("Sec-WebSocket-Key1:"))
                            BuildServerPartialKey(1, Line.Substring(Line.IndexOf(":") + 2));
                        if (Line.Contains("Sec-WebSocket-Key2:"))
                            BuildServerPartialKey(2, Line.Substring(Line.IndexOf(":") + 2));
                        if (Line.Contains("Origin:"))
                            try
                            {
                                this.Handshake = this.Handshake.format(Line.Substring(Line.IndexOf(":") + 2));
                            }
                            catch
                            {
                                this.Handshake = this.Handshake.format("null");
                            }
                    });
                    /*创建响应头信息*/
                    byte[] HandshakeText = Handshake.GetBytes(Encoding.UTF8);
                    byte[] serverHandshakeResponse = new byte[HandshakeText.Length + 16];
                    byte[] serverKey = BuildServerFullKey(last8Bytes);
                    Array.Copy(HandshakeText, serverHandshakeResponse, HandshakeText.Length);
                    Array.Copy(serverKey, 0, serverHandshakeResponse, HandshakeText.Length, 16);
                    /*发送握手信息*/
                    stream.Write(serverHandshakeResponse, 0, serverHandshakeResponse.Length);
                    this.ReceiveWebSocketMessage(stream);
                }
                else
                {
                    this.SocketType = SocketTypes.Socket;
                    this.ReceiveMessage(stream);
                }
            }
            else
            {
                this.OnMessageByte?.Invoke(this, bytes, EventArgs.Empty);
                this.OnMessage?.Invoke(this, bytes.GetString(this.Encoding), EventArgs.Empty);
            }
        }
        /// <summary>
        /// 接收网络流
        /// </summary>
        /// <param name="stream">网络流</param>
        public void ReceiveWebSocketMessage(Stream stream)
        {
            if (!ConnectionSocket.Connected) return;
            if (this.CancelToken.IsCancellationRequested)
            {
                SocketException ex = new SocketException();
                LogHelper.Error(ex, "取消连接[WebSocket]");
                lock (this.ConnectionSocket)
                {
                    if (this.ConnectionSocket == null || this.ConnectionSocket.RemoteEndPoint == null) return;
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                }
                return;
            }
            string messageReceived = string.Empty;
            var buffer = new MemoryStream();
            int readsize;
            do
            {
                readsize = stream.Read(this.ReceivedDataBuffer, 0, this.ReceivedDataBuffer.Length);
                if (readsize <= 0) break;
                buffer.Write(this.ReceivedDataBuffer, 0, readsize);
            } while (readsize > 0);
            if (buffer.Length == 0) return;
            var bytes = buffer.ToArray();
            buffer.Close();
            buffer.Dispose();
            DataFrame dr = new DataFrame(bytes)
            {
                Encoding = this.Encoding,
                DataType = this.DataType
            };
            this.OpCode = dr.Opcode;
            if (this.OpCode == OpCode.Ping)
            {
                this.Send(new DataFrame(dr.Text, this.Encoding, OpCode.Pong, this.DataType).GetBytes());
                return;
            }
            try
            {
                var EndIndex = 0;
                if (!this.IsDataMasked)
                {
                    /*Web Socket protocol: messages are sent with 0x00 and 0xFF as padding bytes*/
                    UTF8Encoding decoder = new UTF8Encoding();
                    int startIndex = 0;
                    int endIndex = 0;
                    /*搜索开始字节*/
                    while (ReceivedDataBuffer[startIndex] == FirstByte[0]) startIndex++;
                    /*搜索结束字节*/
                    endIndex = startIndex + 1;
                    while (ReceivedDataBuffer[endIndex] != LastByte[0] && endIndex != MaxBufferSize - 1) endIndex++;
                    if (endIndex == MaxBufferSize - 1) endIndex = MaxBufferSize;
                    EndIndex = endIndex;
                    /*获取消息*/
                    if (this.DataType == SocketDataType.HexString)
                        messageReceived = ReceivedDataBuffer.ToHexString(startIndex, endIndex - startIndex);
                    else
                        messageReceived = ReceivedDataBuffer.GetString(this.Encoding, startIndex, endIndex - startIndex);
                }
                else
                {
                    messageReceived = dr.Text;
                    EndIndex = (messageReceived.GetBytes() ?? new byte[0]).Length;
                }
                if (messageReceived.Length == 0 || messageReceived.Length == MaxBufferSize && messageReceived[0] == Convert.ToChar(65533))
                {
                    if (this.ConnectionSocket != null)
                    {
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    OnMessage?.Invoke(this, messageReceived, EventArgs.Empty);
                    OnMessageByte?.Invoke(this, this.GetBytes(messageReceived), EventArgs.Empty);
                    /*if (OnMessageByte != null)
                    {
                        byte[] msgBytes = new byte[EndIndex];
                        Buffer.BlockCopy(ReceivedDataBuffer, 0, msgBytes, 0, EndIndex);
                        OnMessageByte?.Invoke(this, msgBytes, EventArgs.Empty);
                    }*/
                    Array.Clear(ReceivedDataBuffer, 0, ReceivedDataBuffer.Length);
                    if (ConnectionSocket == null || !ConnectionSocket.Connected)
                    {
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                    this.ReceiveWebSocketMessage(stream);
                }
            }
            catch (SocketException ex)
            {
                LogHelper.Error(ex, "接收信息失败[WebSocket]");
                lock (this.ConnectionSocket)
                {
                    if (this.ConnectionSocket == null || this.ConnectionSocket.RemoteEndPoint == null) return;
                    OnDisconnected?.Invoke(this, EventArgs.Empty);
                    OnSessionError?.Invoke(this, ex);
                }
            }
        }
        #endregion
    }
    #endregion
}