using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-11 11:00:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis Stream
    /// </summary>
    public partial class RedisClient : Disposable, IRedisClient
    {
        #region 消息队列(Redis Stream)

        #region 添加消息到末尾
        /// <summary>
        /// 添加消息到末尾
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="id">消息 id，我们使用 * 表示由 redis 生成，可以自定义，但是要自己保证递增性。</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">记录值</param>
        /// <returns>返回添加的消息ID</returns>
        public String AddMessageEnd(string key, string id, Dictionary<string, string> values, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return String.Empty;
            if (id.IsNullOrEmpty()) id = "*";
            var list = new List<object> { key, id };
            values.Each(a =>
            {
                list.Add(a.Key);
                list.Add(this.GetValue(a.Value));
            });
            return this.Execute(CommandType.XADD, dbNum, result => result.Value.ToString(), list.ToArray());
        }
        /// <summary>
        /// 添加消息到末尾
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="id">消息 id，我们使用 * 表示由 redis 生成，可以自定义，但是要自己保证递增性。</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">记录值</param>
        /// <returns>返回添加的消息ID</returns>
        public async Task<String> AddMessageEndAsync(string key, string id, Dictionary<string, string> values, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return await Task.FromResult(String.Empty);
            if (id.IsNullOrEmpty()) id = "*";
            var list = new List<object> { key, id };
            values.Each(a =>
            {
                list.Add(a.Key);
                list.Add(this.GetValue(a.Value));
            });
            return await this.ExecuteAsync(CommandType.XADD, dbNum, async result => await Task.FromResult(result.Value.ToString()), list.ToArray());
        }
        #endregion

        #region 获取流包含的元素数量，即消息长度
        /// <summary>
        /// 获取流包含的元素数量
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息队列的数量</returns>
        public int GetMessageLength(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return 0;
            return this.Execute(CommandType.XLEN, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { key });
        }
        /// <summary>
        /// 获取流包含的元素数量
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息队列的数量</returns>
        public async Task<int> GetMessageLengthAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return await Task.FromResult(0);
            return await this.ExecuteAsync(CommandType.XLEN, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { key });
        }
        #endregion

        #region 对流进行修剪，限制长度
        /// <summary>
        /// 对流进行修剪，限制长度
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="length">长度</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回移除的数量</returns>
        public int SetMessageLength(string key, int length, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || length <= 0) return 0;
            return this.Execute(CommandType.XTRIM, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { key, "MAXLEN", length });
        }
        /// <summary>
        /// 对流进行修剪，限制长度
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="length">长度</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回移除的数量</returns>
        public async Task<int> SetMessageLengthAsync(string key, int length, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || length <= 0) return await Task.FromResult(0);
            return await this.ExecuteAsync(CommandType.XTRIM, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { key, "MAXLEN", length });
        }
        #endregion

        #region 删除消息
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="id">消息ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回删的数量</returns>
        public int DeleteMessage(string key, string id, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || id.IsNullOrEmpty()) return 0;
            return this.Execute(CommandType.XDEL, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { key, id });
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="id">消息ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回删的数量</returns>
        public async Task<int> DeleteMessageAsync(string key, string id, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || id.IsNullOrEmpty()) return await Task.FromResult(0);
            return await this.ExecuteAsync(CommandType.XDEL, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { key, id });
        }
        #endregion

        #region  获取消息列表，会自动过滤已经删除的消息
        /// <summary>
        /// 获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        public Dictionary<string, Dictionary<string, string>> GetMessageList(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            var list = new List<object>
            {
                key,
                start.HasValue ? Math.Abs(start.Value).ToString() : "-",
                end.HasValue ? Math.Abs(end.Value).ToString() : "+"
            };
            if (count.HasValue)
            {
                list.Add("COUNT");
                list.Add(Math.Abs(count.Value));
            }
            return this.Execute(CommandType.XRANGE, dbNum, result => (Dictionary<string, Dictionary<string, string>>)result.Value.Value, list.ToArray());
        }
        /// <summary>
        /// 获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        public async Task<Dictionary<string, Dictionary<string, string>>> GetMessageListAsync(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return await Task.FromResult(default(Dictionary<string, Dictionary<string, string>>));
            var list = new List<object> { key };
            list.Add(start.HasValue ? Math.Abs(start.Value).ToString() : "-");
            list.Add(end.HasValue ? Math.Abs(end.Value).ToString() : "-");
            if (count.HasValue)
            {
                list.Add("COUNT");
                list.Add(Math.Abs(count.Value));
            }
            return await this.ExecuteAsync(CommandType.XRANGE, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, string>>)result.Value.Value), list.ToArray());
        }
        #endregion

        #region  反向获取消息列表，会自动过滤已经删除的消息
        /// <summary>
        /// 反向获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        public Dictionary<string, Dictionary<string, string>> GetReverseMessageList(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            var list = new List<object>
            {
                key,
                start.HasValue ? Math.Abs(start.Value).ToString() : "-",
                end.HasValue ? Math.Abs(end.Value).ToString() : "-"
            };
            if (count.HasValue)
            {
                list.Add("COUNT");
                list.Add(Math.Abs(count.Value));
            }
            return this.Execute(CommandType.XREVRANGE, dbNum, result => (Dictionary<string, Dictionary<string, string>>)result.Value.Value, list.ToArray());
        }
        /// <summary>
        /// 反向获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        public async Task<Dictionary<string, Dictionary<string, string>>> GetReverseMessageListAsync(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return await Task.FromResult(default(Dictionary<string, Dictionary<string, string>>));
            var list = new List<object> { key };
            list.Add(start.HasValue ? Math.Abs(start.Value).ToString() : "-");
            list.Add(end.HasValue ? Math.Abs(end.Value).ToString() : "-");
            if (count.HasValue)
            {
                list.Add("COUNT");
                list.Add(Math.Abs(count.Value));
            }
            return await this.ExecuteAsync(CommandType.XREVRANGE, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, string>>)result.Value.Value), list.ToArray());
        }
        #endregion

        #region 以阻塞或非阻塞方式获取消息列表
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        /// <param name="keyIds">队列名 消息ID 集合</param>
        /// <param name="count">数量</param>
        /// <param name="milliseconds">可选，阻塞毫秒数，没有设置就是非阻塞模式</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息数据</returns>
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetMessageList(Dictionary<string, string> keyIds, int? count = null, long? milliseconds = null, int? dbNum = null)
        {
            if (keyIds == null || keyIds.Count == 0) return null;
            var list = new List<object>();
            if (count.HasValue)
            {
                if (count > 0)
                {
                    list.Add("COUNT");
                    list.Add(count);
                }
            }
            if (milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("BLOCK");
                list.Add(milliseconds.Value);
            }
            list.Add("STREAMS");
            list.AddRange(keyIds.Keys);
            list.AddRange(from v in keyIds.Values select v.IsNullOrEmpty() ? "0-0" : v);

            return this.Execute(CommandType.XREAD, dbNum, result => (Dictionary<string, Dictionary<string, Dictionary<string, string>>>)result.Value.Value, list.ToArray());
        }
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        /// <param name="keyIds">队列名 消息ID 集合</param>
        /// <param name="count">数量</param>
        /// <param name="milliseconds">可选，阻塞毫秒数，没有设置就是非阻塞模式</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息数据</returns>
        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> GetMessageListAsync(Dictionary<string, string> keyIds, int? count = null, long? milliseconds = null, int? dbNum = null)
        {
            if (keyIds == null || keyIds.Count == 0) return null;
            var list = new List<object>();
            if (count.HasValue)
            {
                if (count > 0)
                {
                    list.Add("COUNT");
                    list.Add(count);
                }
            }
            if (milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("BLOCK");
                list.Add(milliseconds.Value);
            }
            list.Add("STREAMS");
            list.AddRange(keyIds.Keys);
            keyIds.Values.Each(v =>
            {
                list.Add(v.IsNullOrEmpty() ? "0-0" : v);
            });
            return await this.ExecuteAsync(CommandType.XREAD, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, Dictionary<string, string>>>)result.Value.Value), list.ToArray());
        }
        #endregion

        #region 创建消费者组
        /// <summary>
        /// 创建消费者组
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="groupName">组名</param>
        /// <param name="position">消费位置 TOP 是表示从头开始消费 END是表示从尾部开始消费，只接受新消息，当前 Stream 消息会全部忽略。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>创建成功状态</returns>
        public Boolean CreateConsumerGroup(string key, string groupName, ConsumerGroupPosition position = ConsumerGroupPosition.TOP, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.XGROUPCREATE, dbNum, result => result.OK, new object[] { key, groupName, position == ConsumerGroupPosition.TOP ? "0-0" : "$" });
        }
        /// <summary>
        /// 创建消费者组
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="groupName">组名</param>
        /// <param name="position">消费位置 TOP 是表示从头开始消费 END是表示从尾部开始消费，只接受新消息，当前 Stream 消息会全部忽略。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>创建成功状态</returns>
        public async Task<Boolean> CreateConsumerGroupAsync(string key, string groupName, ConsumerGroupPosition position = ConsumerGroupPosition.TOP, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.XGROUPCREATE, dbNum, async result => await Task.FromResult(result.OK), new object[] { key, groupName, position == ConsumerGroupPosition.TOP ? "0-0" : "$" });
        }
        #endregion

        #region 为消费者组设置新的最后递送消息ID
        /// <summary>
        /// 为消费者组设置新的最后递送消息ID
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="id">消息队列ID $为最后一个ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        public Boolean ConsumerGroupSetID(string key, string groupName, string id = "$", int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.XGROUPSETID, dbNum, result => result.OK, new object[] { key, groupName, id });
        }
        /// <summary>
        /// 创建消费者组
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="id">消息队列ID $为最后一个ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> ConsumerGroupSetIDAsync(string key, string groupName, string id = "$", int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.XGROUPSETID, dbNum, async result => await Task.FromResult(result.OK), new object[] { key, groupName, id });
        }
        #endregion

        #region 删除消费者
        /// <summary>
        /// 删除消费者
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="consumerName">消费者名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        public Boolean ConsumerGroupDeleteConsumer(string key, string groupName, string consumerName, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty() || consumerName.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.XGROUPDELCONSUMER, dbNum, result => result.OK, new object[] { key, groupName, consumerName });
        }
        /// <summary>
        /// 删除消费者
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="consumerName">消费者名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> ConsumerGroupDeleteConsumerAsync(string key, string groupName, string consumerName, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty() || consumerName.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.XGROUPDELCONSUMER, dbNum, async result => await Task.FromResult(result.OK), new object[] { key, groupName, consumerName });
        }
        #endregion

        #region 删除消费者组
        /// <summary>
        /// 删除消费者组
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        public Boolean DeleteConsumerGroup(string key, string groupName, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.XGROUPDESTROY, dbNum, result => result.OK, new object[] { key, groupName });
        }
        /// <summary>
        /// 删除消费者组
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> DeleteConsumerGroupAsync(string key, string groupName, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.XGROUPDESTROY, dbNum, async result => await Task.FromResult(result.OK), new object[] { key, groupName });
        }
        #endregion

        #region 读取消费者组中的消息
        /// <summary>
        /// 读取消费者组中的消息
        /// </summary>
        /// <param name="groupName">消费组名称</param>
        /// <param name="consumerName">消费者名称，如果消费者不存在，会自动创建一个消费者</param>
        /// <param name="keyIds">指定队列名称 及ID  0-0从第一个开始 > 从下一个未消费的消息开始</param>
        /// <param name="count">本次查询的最大数量</param>
        /// <param name="ack">true 无需手动ACK，获取到消息后自动确认</param>
        /// <param name="milliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ReadCounsumerGroupMessage(string groupName, string consumerName, Dictionary<string, string> keyIds, int count = 1, Boolean ack = false, long? milliseconds = null, int? dbNum = null)
        {
            if (groupName.IsNullOrEmpty() || consumerName.IsNullOrEmpty() || keyIds == null || keyIds.Count == 0) return new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            var list = new List<string> { groupName, consumerName, "COUNT", count.ToString() };
            if (milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("BLOCK");
                list.Add(milliseconds.Value.ToString());
            }
            if (ack)
                list.Add("NOACK");
            list.Add("STREAMS");
            list.AddRange(keyIds.Keys);
            list.AddRange(from v in keyIds.Values select v.IsNullOrEmpty() ? ">" : v);

            return this.Execute(CommandType.XREADGROUPGROUP, dbNum, result => (Dictionary<string, Dictionary<string, Dictionary<string, string>>>)result.Value.Value, list.ToArray());
        }
        /// <summary>
        /// 读取消费者组中的消息
        /// </summary>
        /// <param name="groupName">消费组名称</param>
        /// <param name="consumerName">消费者名称，如果消费者不存在，会自动创建一个消费者</param>
        /// <param name="keyIds">指定队列名称 及ID  0-0从第一个开始 > 从下一个未消费的消息开始</param>
        /// <param name="count">本次查询的最大数量</param>
        /// <param name="ack">无需手动ACK，获取到消息后自动确认</param>
        /// <param name="milliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> ReadCounsumerGroupMessageAsync(string groupName, string consumerName, Dictionary<string, string> keyIds, int count = 1, Boolean ack = false, long? milliseconds = null, int? dbNum = null)
        {
            if (groupName.IsNullOrEmpty() || consumerName.IsNullOrEmpty() || keyIds == null || keyIds.Count == 0) return new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            var list = new List<string> { groupName, consumerName, "COUNT", count.ToString() };
            if (milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("BLOCK");
                list.Add(milliseconds.Value.ToString());
            }
            if (!ack)
                list.Add("NOACK");
            list.Add("STREAMS");
            list.AddRange(keyIds.Keys);
            list.AddRange(from v in keyIds.Values select v.IsNullOrEmpty() ? ">" : v);

            return await this.ExecuteAsync(CommandType.XREADGROUPGROUP, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, Dictionary<string, string>>>)result.Value.Value), list.ToArray());
        }
        #endregion

        #region 将消息标记为"已处理"
        /// <summary>
        /// 将消息标记为"已处理"
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="id">消息id</param>
        /// <returns>执行条数</returns>
        public int ConsumerGroupAck(string key, string groupName, int? dbNum = null, params string[] id)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.XACK, dbNum, result => result.OK ? (int)result.Value : -1, key, groupName, id);
        }
        /// <summary>
        /// 将消息标记为"已处理"
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="id">消息id</param>
        /// <returns>执行条数</returns>
        public async Task<int> ConsumerGroupAckAsync(string key, string groupName, int? dbNum = null, params string[] id)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.XACK, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), key, groupName, id);
        }
        #endregion

        #region 显示待处理消息的相关信息
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public ConsumerGroupXPendingModel ConsumerGroupPending(string key, string groupname, int? dbNum = null)
        {
            var Result = new ConsumerGroupXPendingModel();
            if (key.IsNullOrEmpty() || groupname.IsNullOrEmpty()) return Result;
            return this.Execute(CommandType.XPENDING, dbNum, result => result.OK ? (ConsumerGroupXPendingModel)result.Value.Value : Result, key, groupname);
        }
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<ConsumerGroupXPendingModel> ConsumerGroupPendingAsync(string key, string groupname, int? dbNum = null)
        {
            var Result = new ConsumerGroupXPendingModel();
            if (key.IsNullOrEmpty() || groupname.IsNullOrEmpty()) return Result;
            return await this.ExecuteAsync(CommandType.XPENDING, dbNum, async result => await Task.FromResult(result.OK ? (ConsumerGroupXPendingModel)result.Value.Value : Result), key, groupname);
        }
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="count">数量</param>
        /// <param name="consumer">消费者</param>
        /// <param name="milliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<ConsumerGroupXPendingConsumerModel> ConsumerGroupPending(string key, string groupname, string start, string end, int? count = null, string consumer = "", long? milliseconds = null, int? dbNum = null)
        {
            var Result = new List<ConsumerGroupXPendingConsumerModel>();
            if (key.IsNullOrEmpty() || groupname.IsNullOrEmpty()) return Result;
            var list = new List<object> { key, groupname };
            if (milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("IDLE");
                list.Add(milliseconds);
            }
            if (start.IsNullOrEmpty()) start = "-";
            if (end.IsNullOrEmpty()) end = "+";
            list.Add(start);
            list.Add(end);
            if (!count.HasValue || count.GetValueOrDefault() <= 0)
                count = 1;
            list.Add(count);
            if (consumer.IsNotNullOrEmpty())
                list.Add(consumer);
            return this.Execute(CommandType.XPENDING, dbNum, result => result.OK ? (List<ConsumerGroupXPendingConsumerModel>)result.Value.Value : Result, list.ToArray());
        }
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="count">数量</param>
        /// <param name="consumer">消费者</param>
        /// <param name="idleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<ConsumerGroupXPendingConsumerModel>> ConsumerGroupPendingAsync(string key, string groupname, string start, string end, int? count = null, string consumer = "", long? idleMilliseconds = null, int? dbNum = null)
        {
            var Result = new List<ConsumerGroupXPendingConsumerModel>();
            if (key.IsNullOrEmpty() || groupname.IsNullOrEmpty()) return Result;
            var list = new List<object> { key, groupname };
            if (idleMilliseconds.HasValue && idleMilliseconds.Value > 0)
            {
                list.Add("IDLE");
                list.Add(idleMilliseconds);
            }
            if (start.IsNullOrEmpty()) start = "-";
            if (end.IsNullOrEmpty()) end = "+";
            list.Add(start);
            list.Add(end);
            if (!count.HasValue || count.GetValueOrDefault() <= 0)
                count = 1;
            list.Add(count);
            if (consumer.IsNotNullOrEmpty())
                list.Add(consumer);
            return await this.ExecuteAsync(CommandType.XPENDING, dbNum, async result => await Task.FromResult(result.OK ? (List<ConsumerGroupXPendingConsumerModel>)result.Value.Value : Result), list.ToArray());
        }
        #endregion

        #region 转移消息的归属权
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> ConsumerGroupXClaim(string key, string groupName, string consumer, string id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null) => this.ConsumerGroupXClaim(key, groupName, consumer, new string[] { id }, minIdleMilliseconds, count, idleMilliseconds, timeMilliseconds, isForce, isJustID, dbNum);
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Dictionary<string, Dictionary<string, string>>> ConsumerGroupXClaimAsync(string key, string groupName, string consumer, string id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null) => await this.ConsumerGroupXClaimAsync(key, groupName, consumer, new string[] { id }, minIdleMilliseconds, count, idleMilliseconds, timeMilliseconds, isForce, isJustID, dbNum);
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> ConsumerGroupXClaim(string key, string groupName, string consumer, string[] id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null)
        {
            //XCLAIM key group consumer min-idle-time id [id ...] [IDLE ms] [TIME unix-time-milliseconds] [RETRYCOUNT count] [FORCE] [JUSTID]
            if (key.IsNullOrEmpty() || consumer.IsNullOrEmpty()) return null;
            var list = new List<object>
            {
                key,
                groupName,
                consumer,
                minIdleMilliseconds
            };
            list.AddRange(id);
            if (idleMilliseconds.HasValue && idleMilliseconds.GetValueOrDefault() > 0)
            {
                list.Add("IDLE");
                list.Add(idleMilliseconds);
            }
            if (timeMilliseconds.HasValue && timeMilliseconds.GetValueOrDefault() > 0)
            {
                list.Add("TIME");
                list.Add(timeMilliseconds);
            }
            if (count <= 0)
                count = 1;
            list.Add("RETRYCOUNT");
            list.Add(count);
            if (isForce.GetValueOrDefault())
                list.Add("FORCE");
            if (isJustID.GetValueOrDefault())
                list.Add("JUSTID");
            return this.Execute(CommandType.XCLAIM, dbNum, result => result.OK ? (Dictionary<string, Dictionary<string, string>>)result.Value.Value : null, list.ToArray());
        }
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Dictionary<string, Dictionary<string, string>>> ConsumerGroupXClaimAsync(string key, string groupName, string consumer, string[] id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null)
        {
            //XCLAIM key group consumer min-idle-time id [id ...] [IDLE ms] [TIME unix-time-milliseconds] [RETRYCOUNT count] [FORCE] [JUSTID]
            if (key.IsNullOrEmpty() || consumer.IsNullOrEmpty()) return null;
            var list = new List<object>
            {
                key,
                groupName,
                consumer,
                minIdleMilliseconds
            };
            list.AddRange(id);
            if (idleMilliseconds.HasValue && idleMilliseconds.GetValueOrDefault() > 0)
            {
                list.Add("IDLE");
                list.Add(idleMilliseconds);
            }
            if (timeMilliseconds.HasValue && timeMilliseconds.GetValueOrDefault() > 0)
            {
                list.Add("TIME");
                list.Add(timeMilliseconds);
            }
            if (count <= 0)
                count = 1;
            list.Add("RETRYCOUNT");
            list.Add(count);
            if (isForce.GetValueOrDefault())
                list.Add("FORCE");
            if (isJustID.GetValueOrDefault())
                list.Add("JUSTID");
            return await this.ExecuteAsync(CommandType.XCLAIM, dbNum, async result => await Task.FromResult(result.OK ? (Dictionary<string, Dictionary<string, string>>)result.Value.Value : null), list.ToArray());
        }
        #endregion

        #region 查看流和消费者组的相关信息
        /// <summary>
        /// 查看流和消费者组的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<ConsumerGroupXInfoConsumerModel> ConsumerGroupXInfoConsumer(string key, string groupName, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.XINFOCONSUMERS, dbNum, result => result.OK ? (List<ConsumerGroupXInfoConsumerModel>)result.Value.Value : null, key, groupName);
        }
        /// <summary>
        /// 查看流和消费者组的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<ConsumerGroupXInfoConsumerModel>> ConsumerGroupXInfoConsumerAsync(string key, string groupName, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || groupName.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.XINFOCONSUMERS, dbNum, async result => await Task.FromResult(result.OK ? (List<ConsumerGroupXInfoConsumerModel>)result.Value.Value : null), key, groupName);
        }
        #endregion

        #region 打印消费者组的信息
        /// <summary>
        /// 打印消费者组的信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<ConsumerGroupXInfoGroupsModel> ConsumerGroupXInfoGroups(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.XINFOGROUPS, dbNum, result => result.OK ? (List<ConsumerGroupXInfoGroupsModel>)result.Value.Value : null, key);
        }
        /// <summary>
        /// 打印消费者组的信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<ConsumerGroupXInfoGroupsModel>> ConsumerGroupXInfoGroupsAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.XINFOGROUPS, dbNum, async result => await Task.FromResult(result.OK ? (List<ConsumerGroupXInfoGroupsModel>)result.Value.Value : null), key);
        }
        #endregion

        #region 打印流信息
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public ConsumerGroupXInfoStreamModel ConsumerGroupXInfoStream(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.XINFOSTREAM, dbNum, result => result.OK ? (ConsumerGroupXInfoStreamModel)result.Value.Value : null, key);
        }
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<ConsumerGroupXInfoStreamModel> ConsumerGroupXInfoStreamAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.XINFOSTREAM, dbNum, async result => await Task.FromResult(result.OK ? (ConsumerGroupXInfoStreamModel)result.Value.Value : null), key);
        }
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="count">显示数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public ConsumerGroupXInfoStreamFullModel ConsumerGroupXInfoStream(string key, int? count, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            var list = new List<object> { key, "FULL" };
            if (count.HasValue)
            {
                if (count.GetValueOrDefault() > 0)
                {
                    list.Add("COUNT");
                    list.Add(count.GetValueOrDefault());
                }
            }
            return this.Execute(CommandType.XINFOSTREAM, dbNum, result => result.OK ? (ConsumerGroupXInfoStreamFullModel)result.Value.Value : null, list.ToArray());
        }
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="count">显示数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<ConsumerGroupXInfoStreamFullModel> ConsumerGroupXInfoStreamAsync(string key, int? count, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            var list = new List<object> { key, "FULL" };
            if (count.HasValue)
            {
                if (count.GetValueOrDefault() > 0)
                {
                    list.Add("COUNT");
                    list.Add(count.GetValueOrDefault());
                }
            }
            return await this.ExecuteAsync(CommandType.XINFOSTREAM, dbNum, async result => await Task.FromResult(result.OK ? (ConsumerGroupXInfoStreamFullModel)result.Value.Value : null), list.ToArray());
        }
        #endregion

        #endregion
    }
}