using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public partial class RedisClient : IRedisClient
    {
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

        #region 方法

        #region 订阅频道
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
                if (this.Execute(channel.Join(",").IsMatch(@"[\*\?\[\]]")
                    ? CommandType.PSUBSCRIBE : CommandType.SUBSCRIBE, null, result =>
                    {
                        return result.OK;
                    }, channel))
                    OnSubscribe?.Invoke(channel.Join(","));
                else
                    OnError?.Invoke(channel.Join(","), "订阅频道[" + channel + "]失败.");
                Task.Factory.StartNew(() =>
                {
                    while (this.Redis.IsConnected)
                    {
                        var reader = new RedisReader(CommandType.SUBSCRIBE, this.Redis.GetStream());
                        this.OnMessage?.Invoke(channel.Join(","), reader.OK ? reader.Value : new RedisValue());
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                this.OnError?.Invoke(channel.Join(","), ex.Message);
            }
        }
        #endregion

        #region 取消订阅频道
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
            if (this.Execute(channel.Join(",").IsMatch(@"[\*\?\[\]]")
                   ? CommandType.PUNSUBSCRIBE : CommandType.UNSUBSCRIBE, null, result =>
                   {
                       return result.OK;
                   }, channel))
                OnUnSubscribe?.Invoke(channel.Join(","));
            else
                OnError?.Invoke(channel.Join(","), "取消订阅频道[" + channel.Join(",") + "]失败.");
        }
        #endregion

        #region 将信息发送到指定的频道
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
            if (this.Execute(CommandType.PUBLISH, null, result =>
            {
                return result.OK;
            }, channel, message))
                OnSubscribe?.Invoke(channel);
            else
                OnError?.Invoke(channel, "发送频道[" + channel + "]信息失败.");
        }
        #endregion

        #region 查看订阅与发布系统状态
        /// <summary>
        /// 查看订阅与发布系统状态
        /// </summary>
        /// <param name="cmd">子命令</param>
        /// <param name="channel">频道</param>
        /// <returns></returns>
        public RedisReader PubSub(PubSubCommand cmd, params string[] channel)
        {
            if (channel.IsNullOrEmpty())
            {
                this.OnError.Invoke(channel.Join(","), "频道信息出错.");
                return null;
            }
            return this.Execute(CommandType.PUBSUB, null, result => result, new object[] { cmd.ToString() }.Concat(channel).ToArray());
        }
        /// <summary>
        /// 查询系统中符合模式的频道信息，pattern为空，则查询系统中所有存在的频道
        /// </summary>
        /// <param name="pattern">模式</param>
        /// <returns></returns>
        public List<string> PubsubChannels(params string[] pattern)
        {
            var reader = this.PubSub(PubSubCommand.CHANNELS, pattern);
            return reader.OK ? reader.Value.ToList<string>() : null;
        }
        /// <summary>
        /// 查询一个或多个频道的订阅数
        /// </summary>
        /// <param name="channels">频道</param>
        /// <returns></returns>
        public Dictionary<string, int> PubsubNum(params string[] channels)
        {
            var reader = this.PubSub(PubSubCommand.NUMSUB, channels);
            return reader.OK ? reader.Value.ToDictionary<string, int>() : new Dictionary<string, int>();
        }
        /// <summary>
        /// 查询当前客户端订阅了多少频道
        /// </summary>
        /// <returns></returns>
        public List<string> PubsubPats()
        {
            var reader = this.PubSub(PubSubCommand.NUMPAT);
            return reader.OK ? reader.Value.ToList<string>() : null;
        }
        #endregion

        #endregion
    }
}