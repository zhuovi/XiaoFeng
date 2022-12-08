using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Collections;
using XiaoFeng.Redis.IO;
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
    /// Redis线程池
    /// </summary>
    public class RedisPool : ObjectPool<IRedisSocket>
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
        protected override IRedisSocket OnCreate()
        {
            return new RedisSocket(new RedisConfig
            {
                Host = this.Host,
                Port = this.Port,
                ReadTimeout = this.ReceiveTimeout,
                CommandTimeOut = this.SendTimeout
            });
        }
        ///<inheritdoc/>
        public override PoolItem<IRedisSocket> Get()
        {
            var value = base.Get();
            if (value.Value.IsConnected) value.Value.Connect();
            return value;
        }
        ///<inheritdoc/>
        protected override bool OnGet(PoolItem<IRedisSocket> value)
        {
            return value != null && value.IsWork;
        }
        ///<inheritdoc/>
        protected override bool OnPut(PoolItem<IRedisSocket> value)
        {
            return value != null && value.IsWork;
        }
        ///<inheritdoc/>
        public override void Close(IRedisSocket obj)
        {
            if (obj == null) return;

            if (obj.IsConnected)
            {
                //obj.Disconnect(true);
            }
        }
        ///<inheritdoc/>
        public override void OnDispose(IRedisSocket value)
        {
            if (value == null) return;
            if (value.IsConnected)
            {
                value.Close();
                value.Dispose();
            }
            base.OnDispose(value);
        }
        #endregion
    }
}