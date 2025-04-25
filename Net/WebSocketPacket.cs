using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-08-01 15:50:06                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /*
  详见<5.2 基本帧协议>和<11.8.WebSocket 操作码注册>
   0                   1                   2                   3
   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +-+-+-+-+-------+-+-------------+-------------------------------+
   |F|R|R|R| opcode|M| Payload len |    Extended payload length    |
   |I|S|S|S|  (4)  |A|     (7)     |             (16/64)           |
   |N|V|V|V|       |S|             |   (if payload len==126/127)   |
   | |1|2|3|       |K|             |                               |
   +-+-+-+-+-------+-+-------------+ - - - - - - - - - - - - - - - +
   |     Extended payload length continued, if payload len == 127  |
   + - - - - - - - - - - - - - - - +-------------------------------+
   |                               |Masking-key, if MASK set to 1  |
   +-------------------------------+-------------------------------+
   | Masking-key (continued)       |          Payload Data         |
   +-------------------------------- - - - - - - - - - - - - - - - +
   :                     Payload Data continued ...                :
   + - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - +
   |                     Payload Data continued ...                |
   +---------------------------------------------------------------+
*/
    /// <summary>
    /// WebSocket消息包
    /// </summary>
    public class WebSocketPacket : Disposable
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="client">网络流</param>
        /// <param name="requestMessage">请求或响应数据</param>
        public WebSocketPacket(ISocketClient client, string requestMessage)
        {
            this.Client = client;
            this.Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (requestMessage.IndexOf("Sec-WebSocket-Key", StringComparison.OrdinalIgnoreCase) > -1)
            {
                this.RequestMessage = requestMessage;
                this.PacketType = PacketType.Request;
            }
            else
            {
                this.ResponseMessage = requestMessage;
                this.PacketType = PacketType.Response;
            }
            StringReader sr = new StringReader(requestMessage);
            string line;
            this.Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            do
            {
                line = sr.ReadLine();
                if (line == null) break;
                var index = line.IndexOf(":");
                if (index == -1) continue;
                var key = line.Substring(0, index).Trim();
                var val = line.Substring(index + 1).Trim();
                if (this.Data.TryGetValue(key, out var _))
                {
                    this.Data[key] = val;
                }
                else
                    this.Data.Add(key, val);
            } while (line != null);
            if (this.Data.TryGetValue("Host", out var host))
                this.Host = host;
            if (this.Data.TryGetValue("User-Agent", out var useragent))
                this.UserAgent = useragent;
            if (this.Data.TryGetValue("Origin", out var origin))
                this.Origin = origin;
            if (this.Data.TryGetValue("Sec-WebSocket-Protocol", out var secwebsocketprotocol))
            {
                var ssl = secwebsocketprotocol.IndexOf(",") > -1 ? secwebsocketprotocol.Split(',')[0] : secwebsocketprotocol;
                this.SecWebSocketProtocol = ssl;
            }
            if (this.Data.TryGetValue("Sec-WebSocket-Key", out var secwebsocketkey))
            {
                this.SecWebSocketKey = secwebsocketkey;
                this.OpCode = OpCode.Text;
            }
            if (this.Data.TryGetValue("Sec-WebSocket-Accept", out var secwebsocketaccept))
            {
                this.SecWebSocketAccept = secwebsocketaccept;
                this.OpCode = OpCode.Handshake;
            }
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="client">网络流</param>
        /// <param name="options">请求配置</param>
        public WebSocketPacket(ISocketClient client, WebSocketRequestOptions options = null)
        {
            this.Client = client;
            if (options != null) this.WebSocketRequestOptions = options;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 请求配置
        /// </summary>
        private WebSocketRequestOptions WebSocketRequestOptions { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        private Dictionary<string, string> Data { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public SocketDataType DataType { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>
        public ISocketClient Client { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// 请求包
        /// </summary>
        public string RequestMessage { get; set; }
        /// <summary>
        /// 响应包
        /// </summary>
        public string ResponseMessage { get; set; }
        /// <summary>
        /// WebSocketKey
        /// </summary>
        public string SecWebSocketKey { get; set; }
        /// <summary>
        /// WebSocketAccept
        /// </summary>
        public string SecWebSocketAccept { get; set; }
        /// <summary>
        /// WebSocket Protocol
        /// </summary>
        public string SecWebSocketProtocol { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public OpCode OpCode { get; set; }
        /// <summary>
        /// FIN
        /// </summary>
        public Boolean FIN { get; set; }
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestUri { get; set; } = "/";
        /// <summary>
        /// 包类型
        /// </summary>
        public PacketType PacketType { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取请求包数据
        /// </summary>
        /// <returns></returns>
        public string GetRequestData()
        {
            this.OpCode = OpCode.Text;
            if (this.WebSocketRequestOptions == null)
                this.WebSocketRequestOptions = new WebSocketRequestOptions()
                {
                    Uri = this.Uri,
                    SecWebSocketKey = this.SecWebSocketKey
                };
            return this.WebSocketRequestOptions.ToString();
            /*
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine($"GET {this.RequestUri} HTTP/1.1");
            sbr.AppendLine($"Host: {this.Host}");
            sbr.AppendLine($"Connection: Upgrade");
            sbr.AppendLine($"Pragma: no-cache");
            sbr.AppendLine($"Cache-Control: no-cache");
            sbr.AppendLine($"User-Agent: {this.UserAgent.Multivariate("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36")}");
            sbr.AppendLine($"Upgrade: websocket");
            sbr.AppendLine($"Origin: {this.Origin.Multivariate("https://www.eelf.cn")}");
            sbr.AppendLine($"Sec-WebSocket-Version: 13");
            sbr.AppendLine($"Accept-Encoding: gzip, deflate, br");
            sbr.AppendLine($"Accept-Language: zh-CN,zh;q=0.9");
            sbr.AppendLine($"Sec-WebSocket-Key: {this.SecWebSocketKey.Multivariate(RandomHelper.GetRandomString(16).ToBase64String())}");
            sbr.AppendLine($"Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits");
            sbr.AppendLine();
            return sbr.ToString();*/
        }
        /// <summary>
        /// 获取握手包
        /// </summary>
        /// <returns></returns>
        public string GetHandshakeData()
        {
            this.OpCode = OpCode.Handshake;
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("HTTP/1.1 101 Switching Protocols");
            sbr.AppendLine("Connection: Upgrade");
            sbr.AppendLine("Upgrade: WebSocket");
            sbr.AppendLine($"Date: {DateTime.Now.ToString("ddd, dd-MMM-yyyy HH:mm:ss.fff 'GMT'zzz", System.Globalization.CultureInfo.GetCultureInfo("en-US"))}");
            sbr.AppendLine($"Server: ELF v2.0({OS.Platform.GetOSPlatform()})");
            sbr.AppendLine("Author: Jacky(QQ:7092734,Email:jacky@eelf.cn,Site:www.eelf.cn)");
            sbr.AppendLine("Copyright: 未经授权禁止使用,盗版必究.");

            if (this.Data.TryGetValue("Sec-WebSocket-Key", out var key))
            {
                this.SecWebSocketAccept = key;
                sbr.AppendLine($"Sec-WebSocket-Accept: {this.ComputeWebSocketHandshakeSecurityHash09(this.SecWebSocketKey)}");
            }
            else return string.Empty;

            if (this.SecWebSocketProtocol.IsNotNullOrEmpty())
                sbr.AppendLine($"Sec-WebSocket-Protocol: {this.SecWebSocketProtocol}");

            sbr.AppendLine();
            return sbr.ToString();
        }
        #region 打包请求连接数据
        /// <summary>
        /// 打包请求连接数据
        /// </summary>
        /// <param name="secWebSocketKey">客户请求头信息</param>
        /// <returns>SecWebScoektAcceptCode</returns>
        public string ComputeWebSocketHandshakeSecurityHash09(String secWebSocketKey)
        {
            const string MagicKEY = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            string ret = secWebSocketKey.Trim(new char[] { '\r', '\n' }) + MagicKEY;
            //SHA1 sha = new SHA1CryptoServiceProvider();
            SHA1 sha = SHA1.Create();
            byte[] sha1Hash = sha.ComputeHash(ret.GetBytes(this.Encoding));
            return sha1Hash.ToBase64String();
        }
        #endregion
        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="opCode">操作码</param>
        /// <returns></returns>
        public byte[] Packet(byte[] bytes, OpCode opCode = OpCode.Text)
        {
            return new DataFrame(bytes, opCode).GetBytes();
        }
        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public byte[] UnPacket(byte[] bytes)
        {
            DataFrame dr = new DataFrame(bytes)
            {
                Encoding = this.Encoding,
                DataType = this.DataType
            };
            this.OpCode = dr.Opcode;
            return dr.Body;
            /*
            bool fin = (bytes[0] & 0b10000000) != 0,
                    mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
            this.FIN = fin;
            int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                offset = 2;
            this.OpCode = (OpCode)opcode;
            ulong msglen = (ulong)bytes[1] & 0b01111111;

            if (msglen == 126)
            {
                // bytes are reversed because websocket will print them in Big-Endian, whereas
                // BitConverter will want them arranged in little-endian on windows
                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering
                // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                // websocket frame available through client.Available).
                msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] }, 0);
                offset = 10;
            }

            if (msglen == 0)
            {
                return Array.Empty<byte>();
            }
            else if (mask)
            {
                byte[] decoded = new byte[msglen];
                byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                offset += 4;

                for (ulong i = 0; i < msglen; ++i)
                    decoded[i] = (byte)(bytes[(ulong)offset + i] ^ masks[i % 4]);
                return decoded;
            }
            else
                return Array.Empty<byte>();*/
        }
        /// <summary>
        /// 握手
        /// </summary>
        /// <returns></returns>
        public async Task<int> HandshakeAsync()
        {
            var stream = this.Client.GetSslStream();
            if (stream == null)
            {
                await Task.FromException(new Exception("客户端网络已经断开."));
                return await Task.FromResult(0).ConfigureAwait(false);
            }

            var handshakedata = this.GetHandshakeData();
            if (handshakedata.IsNullOrEmpty()) return await Task.FromResult(-1);
            var bytes = handshakedata.GetBytes(this.Encoding);

            await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
            return await Task.FromResult(bytes.Length);
        }
        #endregion
    }
}