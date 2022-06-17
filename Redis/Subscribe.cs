using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Redis;
/****************************************************************
*  Copyright © (2022-2023) www.fayelf.com All Rights Reserved.  *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-05-31 17:25:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 订阅频道
    /// </summary>
    public class Subscribe
    {
        /// <summary>
        /// 设备频道
        /// </summary>
        /// <param name="redis">Redis</param>
        public Subscribe(RedisClient redis)
        {
            this.Redis = redis;
        }

        #region 事件
        /// <summary>
        /// 接收频道消息
        /// </summary>
        public event OnMessageEventHandler OnMessage;
        /// <summary>
        /// 订阅频道消息
        /// </summary>
        public event OnSubscribeEventHandler OnSubscribe;
        /// <summary>
        /// 取消订阅频道消息
        /// </summary>
        public event OnUnSubscribeEventHandler OnUnSubscribe;
        /// <summary>
        /// 出错
        /// </summary>
        public event OnErrorEventHandler OnError;
        #endregion

        #region 属性
        /// <summary>
        /// Redis客户端
        /// </summary>
        public RedisClient Redis { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channel">频道 支持模糊订阅 如:*?[]  * 至少0个占位符  ?一个占位符 []选择字符</param>
        public void SubScribe(params string[] channel)
        {
            if (channel.IsNullOrEmpty())
            {
                this.OnError.Invoke(channel.Join(","), "无频道订阅.");
                return;
            }
            try
            {
                if (Redis.Execute(channel.Join(",").IsMatch(@"[\*\?\[\]]")
                    ? CommandType.PSUBSCRIBE : CommandType.SUBSCRIBE, null, result =>
                    {
                        return result.OK;
                    }, channel))
                    OnSubscribe?.Invoke(channel.Join(","));
                else
                    OnError?.Invoke(channel.Join(","), "订阅频道[" + channel + "]失败.");
                Task.Factory.StartNew(async () =>
                {
                    while (Redis.IsConnected.HasValue && Redis.IsConnected.Value)
                    {
                        var ms = new MemoryStream();
                        if (Redis.Stream.DataAvailable)
                        {
                            var bytes = new byte[Redis.MemorySize];
                            do
                            {
                                Array.Clear(bytes, 0, Redis.MemorySize);
                                var count = await Redis.Stream.ReadAsync(bytes, 0, bytes.Length);
                                ms.Write(bytes, 0, count);
                            } while (Redis.Stream.DataAvailable);
                            var msg = ms.ToArray().GetString();

                            this.OnMessage?.Invoke(channel.Join(","), ms.ToArray().GetString());
                        }
                       await Task.Delay(1000);
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(channel.Join(","), ex.Message);
            }
        }
        /// <summary>
        /// 取消订阅频道
        /// </summary>
        /// <param name="channel">频道</param>
        public void UnSubScribe(params string[] channel)
        {
            if (channel.IsNullOrEmpty())
            {
                this.OnError.Invoke(channel.Join(","), "无频道取消订阅.");
                return;
            }
            if (Redis.Execute(channel.Join(",").IsMatch(@"[\*\?\[\]]")
                   ? CommandType.PUNSUBSCRIBE : CommandType.UNSUBSCRIBE, null, result =>
                   {
                       return result.OK;
                   }, channel))
                OnUnSubscribe?.Invoke(channel.Join(","));
            else
                OnError?.Invoke(channel.Join(","), "取消订阅频道[" + channel.Join(",") + "]失败.");
        }
        /// <summary>
        /// 将信息发送到指定的频道
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="channel">频道</param>
        public void PubLish(string message, string channel)
        {
            if (channel.IsNullOrEmpty())
            {
                this.OnError.Invoke(channel, "发送频道信息出错.");
                return;
            }
            if (Redis.Execute(CommandType.PUBLISH , null, result =>
                   {
                       return result.OK;
                   },channel,message))
                OnSubscribe?.Invoke(channel);
            else
                OnError?.Invoke(channel, "发送频道[" + channel + "]信息失败.");
        }
        /// <summary>
        /// 查看订阅与发布系统状态
        /// </summary>
        /// <param name="channel">频道</param>
        /// <returns></returns>
        public List<string> PubSub(params string[] channel)
        {
            if (channel.IsNullOrEmpty())
            {
                this.OnError.Invoke(channel.Join(","), "频道信息出错.");
                return null;
            }
            return Redis.Execute(CommandType.PUBSUB, null, result =>
            {
                return result.OK ? (List<string>)result.Value : null;
            }, channel);
        }
        #endregion
    }
}