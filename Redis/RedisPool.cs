using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Collections;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-10 11:24:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// RedisPool 类说明
    /// </summary>
    public class RedisPool : ObjectPool<Socket>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RedisPool() : base() { }

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
        public string Host { get; set; } = "127.0.0.1";
        /// <summary>
        /// 通讯端口
        /// </summary>
        public int Port { get; set; } = 6379;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        protected override Socket OnCreate()
        {
            var SocketClient = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType)
            {
                SendTimeout = this.SendTimeout,
                ReceiveTimeout = this.ReceiveTimeout
            };
            if (this.Host.IsNullOrEmpty()) this.Host = "127.0.0.1";
            if (this.Port == 0) this.Port = 6379;
            try
            {
                SocketClient.Connect(this.Host, this.Port);
                var Stream = new NetworkStream(SocketClient)
                {
                    ReadTimeout = this.ReceiveTimeout,
                    WriteTimeout = this.SendTimeout
                };
                if (this.Password.IsNotNullOrEmpty())
                {
                    var line = new Command(CommandType.AUTH, this.Password).ToBytes();
                    Stream.Write(line, 0, line.Length);
                    Stream.Flush();
                    return this.GetCommandResult(Stream, CommandType.AUTH).OK ? SocketClient : null;
                }
                return SocketClient;
            }
            catch (SocketException ex)
            {
                LogHelper.Error(ex, $"SOCKET创建失败[{ex.Message}]");
                return null;
            }
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
        public override void Close(Socket obj)
        {
            if (obj == null) return;

            if (obj.Connected)
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
                value.Close();
                value.Dispose();
            }
            base.OnDispose(value);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="func">回调方法</param>
        /// <param name="args">参数集</param>
        /// <returns>执行结果</returns>
        public T Execute<T>(CommandType commandType, int? dbNum, Func<CommandResult, T> func, params object[] args)
        {
            var item = base.Get();
            if (item.Value == null || !item.Value.Connected) return default(T);
            var Stream = new NetworkStream(item.Value)
            {
                ReadTimeout = this.ReceiveTimeout,
                WriteTimeout = this.SendTimeout
            };
            byte[] line;
            if (dbNum.HasValue && dbNum.Value > -1)
            {
                line = new Command(CommandType.SELECT, dbNum).ToBytes();
                Stream.Write(line, 0, line.Length);
                Stream.Flush();
                if (!this.GetCommandResult(Stream, CommandType.SELECT).OK) return default(T);
            }
            line = new Command(commandType, args).ToBytes();
            Stream.Write(line, 0, line.Length);
            Stream.Flush();
            try
            {
                return func.Invoke(this.GetCommandResult(Stream, commandType));
            }
            finally
            {
                base.Put(item);
            }
        }
        /// <summary>
        /// 执行 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="func">回调方法</param>
        /// <param name="args">参数集</param>
        /// <returns>执行结果</returns>
        public async Task<T> ExecuteAsync<T>(CommandType commandType, int? dbNum, Func<CommandResult, Task<T>> func, params object[] args)
        {
            var item = base.Get();
            if (item.Value == null || !item.Value.Connected) return default(T);
            var Stream = new NetworkStream(item.Value)
            {
                ReadTimeout = this.ReceiveTimeout,
                WriteTimeout = this.SendTimeout
            };
            byte[] line;
            if (dbNum.HasValue && dbNum.Value > -1)
            {
                line = new Command(CommandType.SELECT, dbNum).ToBytes();
                await Stream.WriteAsync(line, 0, line.Length).ConfigureAwait(false);
                await Stream.FlushAsync().ConfigureAwait(false); ;
                if (!this.GetCommandResult(Stream, CommandType.SELECT).OK) return default(T);
            }
            line = new Command(commandType, args).ToBytes();
            await Stream.WriteAsync(line, 0, line.Length).ConfigureAwait(false);
            await Stream.FlushAsync().ConfigureAwait(false);
            try
            {
                return await func.Invoke(this.GetCommandResult(Stream, commandType));
            }
            finally
            {
                base.Put(item);
            }
        }
        #endregion

        #region 获取响应
        /// <summary>
        /// 获取响应
        /// </summary>
        /// <param name="Stream">网络流</param>
        /// <param name="commandType">命令</param>
        /// <returns>响应结果</returns>
        public CommandResult GetCommandResult(NetworkStream Stream, CommandType commandType)
        {
            while (Stream.CanRead)
            {
                if (Stream.DataAvailable)
                {
                    var bs = new MemoryStream();
                    var bytes = new byte[MemorySize];
                    var count = Stream.Read(bytes, 0, bytes.Length);
                    while (count > 0)
                    {
                        bs.Write(bytes, 0, count);
                        Array.Clear(bytes, 0, count);
                        count = Stream.DataAvailable ? Stream.Read(bytes, 0, bytes.Length) : 0;
                    }
                    return new CommandResult(commandType, bs.ToArray());
                }
            }
            return null;
        }
        #endregion
    }
}