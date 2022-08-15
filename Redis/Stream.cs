using System;
using System.Collections.Generic;
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
    public partial class RedisClient : Disposable
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
            var list = new List<object> { key };
            values.Each(a =>
            {
                list.Add(a.Value);
                list.Add(this.GetValue(a.Key));
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
            var list = new List<object> { key };
            values.Each(a =>
            {
                list.Add(a.Value);
                list.Add(this.GetValue(a.Key));
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
        public int GetMessageLength(string key,int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return 0;
            return this.Execute(CommandType.XLEN, dbNum, result => (int)result.Value, new object[] { key });
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
            return await this.ExecuteAsync(CommandType.XLEN, dbNum, async result => await Task.FromResult((int)result.Value), new object[] { key });
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
        public int SetMessageLength(string key,int length,int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || length <= 0) return 0;
            return this.Execute(CommandType.XTRIM, dbNum, result => (int)result.Value, new object[] { key, "MAXLEN", length });
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
            return await this.ExecuteAsync(CommandType.XTRIM, dbNum, async result => await Task.FromResult((int)result.Value), new object[] { key, "MAXLEN", length });
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
        public int DeleteMessage(string key,string id,int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || id.IsNullOrEmpty()) return 0;
            return this.Execute(CommandType.XDEL, dbNum, result => (int)result.Value, new object[] { key, id });
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
            return await this.ExecuteAsync(CommandType.XDEL, dbNum, async result => await Task.FromResult((int)result.Value), new object[] { key, id });
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
            var list = new List<object> { key };
            list.Add(start.HasValue ? Math.Abs(start.Value).ToString() : "-");
            list.Add(end.HasValue ? Math.Abs(end.Value).ToString() : "-");
            if (count.HasValue)
            {
                list.Add("COUNT");
                list.Add(Math.Abs(count.Value));
            }
            return this.Execute(CommandType.XRANGE, dbNum, result => (Dictionary<string, Dictionary<string, string>>)result.Value, list.ToArray());
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
            return await this.ExecuteAsync(CommandType.XRANGE, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, string>>)result.Value), list.ToArray());
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
            var list = new List<object> { key };
            list.Add(start.HasValue ? Math.Abs(start.Value).ToString() : "-");
            list.Add(end.HasValue ? Math.Abs(end.Value).ToString() : "-");
            if (count.HasValue)
            {
                list.Add("COUNT");
                list.Add(Math.Abs(count.Value));
            }
            return this.Execute(CommandType.XREVRANGE, dbNum, result => (Dictionary<string, Dictionary<string, string>>)result.Value, list.ToArray());
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
            return await this.ExecuteAsync(CommandType.XREVRANGE, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, string>>)result.Value), list.ToArray());
        }
        #endregion

        #region 以阻塞或非阻塞方式获取消息列表
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="keyIds">队列名 消息ID 集合</param>
        /// <param name="milliseconds">可选，阻塞毫秒数，没有设置就是非阻塞模式</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息数据</returns>
        public Dictionary<string,Dictionary<string,Dictionary<string,string>>> GetMessageList(int count, Dictionary<string, string> keyIds, long? milliseconds=null,int? dbNum=null)
        {
            if (count <= 0) count = 1;
            var list= new List<object> { "COUNT",count };
            if(milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("BLOCK");
                list.Add(milliseconds.Value);
            }
            list.AddRange(keyIds.Keys);
            keyIds.Values.Each(v =>
            {
                list.Add(v.IsNullOrEmpty() ? "0-0" : v);
            });
            return this.Execute(CommandType.XREAD, dbNum, result => (Dictionary<string, Dictionary<string, Dictionary<string, string>>>)result.Value, list.ToArray());
        }
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="keyIds">队列名 消息ID 集合</param>
        /// <param name="milliseconds">可选，阻塞毫秒数，没有设置就是非阻塞模式</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息数据</returns>
        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> GetMessageListAsync(int count, Dictionary<string, string> keyIds, long? milliseconds = null, int? dbNum = null)
        {
            if (count <= 0) count = 1;
            var list = new List<object> { "COUNT", count };
            if (milliseconds.HasValue && milliseconds.Value > 0)
            {
                list.Add("BLOCK");
                list.Add(milliseconds.Value);
            }
            list.AddRange(keyIds.Keys);
            keyIds.Values.Each(v =>
            {
                list.Add(v.IsNullOrEmpty() ? "0-0" : v);
            });
            return await this.ExecuteAsync(CommandType.XREAD, dbNum, async result => await Task.FromResult((Dictionary<string, Dictionary<string, Dictionary<string, string>>>)result.Value), list.ToArray());
        }
        #endregion

         

        #endregion
    }
}