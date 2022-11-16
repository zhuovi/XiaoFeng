using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-10 11:26:26                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Collections
{
    /// <summary>
    /// Socket连接池
    /// </summary>
    public class SocketPool : ObjectPool<Socket>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SocketPool() : base() { }

        #endregion

        #region 属性
        /// <summary>
        /// 寻址方案
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;
        /// <summary>
        /// 套接字类型
        /// </summary>
        public SocketType SocketType { get; set; } = SocketType.Stream;
        /// <summary>
        /// 支持协议
        /// </summary>
        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        /// <summary>
        /// 发送超时
        /// </summary>
        public int SendTimeout { get; set; } = 10000;
        /// <summary>
        /// 接收超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 10000;
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int MemorySize { get; set; } = 1024;
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 通讯端口
        /// </summary>
        public int Port { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        protected override Socket OnCreate()
        {
            var socket = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType)
            {
                SendTimeout = this.SendTimeout,
                ReceiveTimeout = this.ReceiveTimeout
            };
            if (this.Host.IsNullOrEmpty()) this.Host = "127.0.0.1";
            if (this.Port == 0) this.Port = 1006;
            try { socket.Connect(this.Host, this.Port); }
            catch (SocketException ex)
            {
                LogHelper.Error(ex, "SOCKET创建失败.");
            }
            return socket;
        }
        ///<inheritdoc/>
        public override PoolItem<Socket> Get()
        {
            var value = base.Get();
            if (!value.Value.Connected) value.Value.Connect(this.Host, this.Port);
            return value;
        }
        ///<inheritdoc/>
        protected override bool OnGet(PoolItem<Socket> value)
        {
            return value != null && value.IsWork;
        }
        ///<inheritdoc/>
        protected override bool OnPut(PoolItem<Socket> value)
        {
            return value != null && value.IsWork;
        }
        ///<inheritdoc/>
        public override void Close(Socket item)
        {
            if (item == null) return;

            if (item.Connected)
            {
                //socket.Disconnect(true);
            }
        }
        ///<inheritdoc/>
        public override void OnDispose(Socket value)
        {
            if (value == null) return;
            if (value.Connected)
            {
                value.Disconnect(true);
                value.Shutdown(SocketShutdown.Both);
                value.Close();
                value.Dispose();
            }
            base.OnDispose(value);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback"></param>
        public void Execute(Action<NetworkStream> action, Action<Byte[]> callback)
        {
            var val = this.Get();
            try
            {
                var stream = new NetworkStream(val.Value)
                {
                    ReadTimeout = this.ReceiveTimeout,
                    WriteTimeout = this.SendTimeout
                };
                action.Invoke(stream);
                stream.Flush();
                new Task(s =>
                {
                    var stm = s as NetworkStream;
                    //while (true)
                    //{
                        if (stream.DataAvailable)
                        {
                            var bs = new MemoryStream();
                            var bytes = new byte[MemorySize];
                            var count = stream.Read(bytes, 0, bytes.Length);
                            while (count > 0)
                            {
                                bs.Write(bytes, 0, count);
                                Array.Clear(bytes, 0, count);
                                count = stream.DataAvailable ? stream.Read(bytes, 0, bytes.Length) : 0;
                            }
                            callback.Invoke(bs.ToArray());
                            //break;
                        }
                    //}
                }, stream).Start();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            finally
            {
                this.Put(val);
            }
        }
        #endregion
    }
}