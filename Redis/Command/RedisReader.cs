using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using XiaoFeng;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-12 16:33:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 数据读
    /// </summary>
    public class RedisReader
    {
        /*
        * +OK\r\n 成功
        * -ERR 出错  -开头的为错误
        * $1\r\n数据\r\n 单条数据
        * *2\r\n 多条数据
        * :2\r\n 整型数字
        */
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RedisReader() { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="stream">网络流</param>
        /// <param name="args">参数</param>
        public RedisReader(CommandType commandType, Stream stream, object[] args = null)
        {
            this.CommandType = commandType;
            this.Arguments = args;
            this.Reader = stream;
            this.GetValue();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 参数
        /// </summary>
        public object[] Arguments { get; set; }
        /// <summary>
        /// 锁
        /// </summary>
        public object StreamLock { get; set; } = new object();
        /// <summary>
        /// 换行
        /// </summary>
        private const string EOF = "\r\n";
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 流
        /// </summary>
        public Stream Reader { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public RedisValue Value { get; set; }
        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultType Status { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public Boolean OK { get; set; }
        #endregion

        #region 方法

        #region 获取数据
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="resultType">结果类型</param>
        /// <returns></returns>
        public RedisValue ReadValue(ResultType? resultType=null)
        {
            if (!resultType.HasValue)
                resultType = this.ReadType();
            RedisValue result;
            switch (resultType.Value)
            {
                case ResultType.Status:
                    result = this.ReadStatus();
                    break;
                case ResultType.Error:
                    result = this.ReadError();
                    break;
                case ResultType.Int:
                    result = this.ReadInt();
                    break;
                case ResultType.Bulk:
                    result = this.ReadBulkString();
                    break;
                case ResultType.MultiBulk:
                   result = this.ReadMultiBulk();
                    break;
                default:
                    result = new RedisValue();
                    break;
            }
            return result;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public RedisValue GetValue()
        {
            var type = this.ReadType();
            if (this.CommandType.Name.EqualsIgnoreCase("publish"))
            {
                this.Status = ResultType.Status;
                if (type == 0)
                {
                    this.OK = false;
                    this.Value = null;
                }
                else
                {
                    this.OK = true;
                    var size = new byte[] { (byte)type }.GetString().ToCast<int>();
                    this.Value = this.ReadMultiBulk(size);
                }
                return this.Value;
            }
            this.Status = type;
            var result = this.ReadValue(type);
            switch (this.CommandType.Name)
            {
                /*
                 * +OK\r\n 成功
                 */
                case "PING":/*CommandType.PING*/
                case "AUTH":/*CommandType.AUTH*/
                case "QUIT":/*CommandType.QUIT*/
                case "SELECT":/*CommandType.SELECT*/
                case "RENAME":/*CommandType.RENAME*/
                case "SETEX":/*CommandType.SETEX*/
                case "SET":/*CommandType.SET*/
                case "MSET":/*CommandType.MSET*/
                case "COPY":/*CommandType.COPY*/
                case "HMSET":/*CommandType.HMSET*/
                case "LSET":/*CommandType.LSET*/
                case "LTRIM":/*CommandType.LTRIM*/
                case "PFMERGE":/*CommandType.PFMERGE*/
                case "BGREWRITEAOF":/*CommandType.BGREWRITEAOF*/
                case "BGSAVE":/*CommandType.BGSAVE*/
                case "SLAVEOF":/*CommandType.SLAVEOF*/
                case "SHUTDOWN":/*CommandType.SHUTDOWN*/
                case "SAVE":/*CommandType.SAVE*/
                case "FLUSHDB":/*CommandType.FLUSHDB*/
                case "FLUSHALL":/*CommandType.FLUSHALL*/
                case "XGROUP CREATE":/*CommandType.XGROUPCREATE*/
                case "XGROUP SETID":/*CommandType.XGROUPSETID*/
                case "SCRIPT KILL":/*CommandType.SCRIPTKILL*/
                case "SCRIPT FLUSH":/*CommandType.SCRIPTFLUSH*/
                    this.OK = type == ResultType.Status;
                    this.Value = result;
                    break;
                case "TYPE":/*CommandType.TYPE*/
                    this.OK = type == ResultType.Status;
                    this.Value = result.Value.ToCast<RedisKeyType>();
                    break;
                /*
                 * :2\r\n 整型数字
                 */
                case "DEL":/*CommandType.DEL*/
                case "EXISTS":/*CommandType.EXISTS*/
                case "EXPIRE":/*CommandType.EXPIRE*/
                case "PEXPIRE":/*CommandType.PEXPIRE*/
                case "EXPIREAT":/*CommandType.EXPIREAT*/
                case "PEXPIREAT":/*CommandType.PEXPIREAT*/
                case "RENAMENX":/*CommandType.RENAMENX*/
                case "MOVE":/*CommandType.MOVE*/
                case "PERSIST":/*CommandType.PERSIST*/
                case "TTL":/*CommandType.TTL*/
                case "PTTL":/*CommandType.PTTL*/
                case "SETNX":/*CommandType.SETNX*/
                case "MSETNX":/*CommandType.MSETNX*/
                case "SETRANGE":/*CommandType.SETRANGE*/
                case "APPEND":/*CommandType.APPEND*/
                case "STRLEN":/*CommandType.STRLEN*/
                case "INCRBY":/*CommandType.INCRBY*/
                case "INCR":/*CommandType.INCR*/
                case "DECR":/*CommandType.DECR*/
                case "DECRBY":/*CommandType.DECRBY*/
                case "HSET":/*CommandType.HSET*/
                case "HSETNX":/*CommandType.HSETNX*/
                case "HLEN":/*CommandType.HLEN*/
                case "HDEL":/*CommandType.HDEL*/
                case "HEXISTS":/*CommandType.HEXISTS*/
                case "HINCRBY":/*CommandType.HINCRBY*/
                case "LPUSH":/*CommandType.LPUSH*/
                case "RPUSH":/*CommandType.RPUSH*/
                case "LINSERT":/*CommandType.LINSERT*/
                case "LLEN":/*CommandType.LLEN*/
                case "RPUSHX":/*CommandType.RPUSHX*/
                case "LPUSHX":/*CommandType.LPUSHX*/
                case "LREM":/*CommandType.LREM*/
                case "SADD":/*CommandType.SADD*/
                case "SMOVE":/*CommandType.SMOVE*/
                case "SREM":/*CommandType.SREM*/
                case "SCARD":/*CommandType.SCARD*/
                case "SDIFFSTORE":/*CommandType.SDIFFSTORE*/
                case "SINTERSTORE":/*CommandType.SINTERSTORE*/
                case "SUNIONSTORE":/*CommandType.SUNIONSTORE*/
                case "SISMEMBER":/*CommandType.SISMEMBER*/
                case "ZADD":/*CommandType.ZADD*/
                case "ZCARD":/*CommandType.ZCARD*/
                case "ZLEXCOUNT":/*CommandType.ZLEXCOUNT*/
                case "ZCOUNT":/*CommandType.ZCOUNT*/
                case "ZINTERSTORE":/*CommandType.ZINTERSTORE*/
                case "ZUNIONSTORE":/*CommandType.ZUNIONSTORE*/
                case "ZREVRANK":/*CommandType.ZREVRANK*/
                case "ZRANK":/*CommandType.ZRANK*/
                case "ZINCRBY":/*CommandType.ZINCRBY*/
                case "ZREM":/*CommandType.ZREM*/
                case "ZREMRANGEBYLEX":/*CommandType.ZREMRANGEBYLEX*/
                case "ZREMRANGEBYSCORE":/*CommandType.ZREMRANGEBYSCORE*/ 
                case "ZREMRANGEBYRANK":/*CommandType.ZREMRANGEBYRANK*/
                case "PFADD":/*CommandType.PFADD*/
                case "PFCOUNT":/*CommandType.PFCOUNT*/
                case "GEOADD":/*CommandType.GEOADD*/
                case "DBSIZE":/*CommandType.DBSIZE*/
                case "PUBLISH":/*CommandType.PUBLISH*/
                case "XLEN":/*CommandType.XLEN*/
                case "XTRIM":/*CommandType.XTRIM*/
                case "XDEL":/*CommandType.XDEL*/
                case "XGROUP DESTROY":/*CommandType.XGROUPDESTROY*/
                case "XGROUP DELCONSUMER":/*CommandType.XGROUPDELCONSUMER*/
                case "XACK":/*CommandType.XACK*/
                    if (type == ResultType.Int)
                    {
                        this.Value = result;
                        this.OK = true;
                    }
                    else
                    {
                        this.Value = result.Value?.ToString();
                        this.OK = false;
                    }
                    break;
                /*
                 * $1\r\n数据\r\n 单条数据
                 */
                case "ECHO":/*CommandType.ECHO*/
                case "DUMP":/*CommandType.DUMP*/
                case "RANDOMKEY":/*CommandType.RANDOMKEY*/
                case "GET":/*CommandType.GET*/
                case "GETRANGE":/*CommandType.GETRANGE*/
                case "GETSET":/*CommandType.GETSET*/
                case "INCRBYFLOAT":/*CommandType.INCRBYFLOAT*/
                case "DECRBYFLOAT":/*CommandType.DECRBYFLOAT*/
                case "HGET":/*CommandType.HGET*/
                case "HINCRBYFLOAT":/*CommandType.HINCRBYFLOAT*/
                case "BRPOPLPUSH":/*CommandType.BRPOPLPUSH*/
                case "LINDEX":/*CommandType.LINDEX*/
                case "LPOP":/*CommandType.LPOP*/
                case "RPOP":/*CommandType.RPOP*/
                case "ZSCORE":/*CommandType.ZSCORE*/
                case "GEODIST":/*CommandType.GEODIST*/
                case "XADD":/*CommandType.XADD*/
                case "SCRIPT LOAD":/*CommandType.SCRIPTLOAD*/
                    this.OK = type == ResultType.Bulk;
                    this.Value = result;
                    break;
                /* 
                 * *2\r\n 多条数据
                 * *2\r\n$2\r\n13\r\n*2\r\n$5\r\nTest3\r\n$5\r\nTest4\r\n
                 */
                case "SCAN":/*CommandType.SCAN*/
                case "HSCAN":/*CommandType.HSCAN*/
                case "SSCAN":/*CommandType.SSCAN*/
                case "ZSCAN":/*CommandType.ZSCAN*/
                    this.OK = type == ResultType.MultiBulk;
                    this.Value = result[1];
                    if (this.CommandType.Name == "HSCAN")
                    {
                        this.Value = new RedisValue(this.Value.ToDictionary<RedisValue, RedisValue>());
                    }
                    break;
                /*
                 * *3\r\n$5\r\nTest2\r\n$5\r\nTest1\r\n$5\r\nTest4\r\n
                 */
                case "KEYS":/*CommandType.KEYS*/
                case "MGET":/*CommandType.MGET*/
                case "HKEYS":/*CommandType.HKEYS*/
                case "HVALS":/*CommandType.HVALS*/
                case "HMGET":/*CommandType.HMGET*/
                case "LRANGE":/*CommandType.LRANGE*/
                case "SDIFF":/*CommandType.SDIFF*/
                case "SINTER":/*CommandType.SINTER*/
                case "SUNION":/*CommandType.SUNION*/
                case "SMEMBERS":/*CommandType.SMEMBERS*/
                case "SPOP":/*CommandType.SPOP*/
                case "SRANDMEMBER":/*CommandType.SRANDMEMBER*/
                case "ZRANGE":/*CommandType.ZRANGE*/
                case "ZREVRANGE":/*CommandType.ZREVRANGE*/
                case "ZRANGEBYLEX":/*CommandType.ZRANGEBYLEX*/
                case "ZRANGEBYSCORE":/*CommandType.ZRANGEBYSCORE*/
                case "ZREVRANGEBYSCORE":/*CommandType.ZREVRANGEBYSCORE*/
                case "GEOHASH":/*CommandType.GEOHASH*/
                case "ROLE":/*CommandType.ROLE*/
                case "PSUBSCRIBE":/*CommandType.PSUBSCRIBE*/
                case "SUBSCRIBE":/*CommandType.SUBSCRIBE*/
                case "UNSUBSCRIBE":/*CommandType.UNSUBSCRIBE*/
                    this.OK = type == ResultType.MultiBulk;
                    this.Value = result;
                    break;
                case "PUBSUB":/*CommandType.PUBSUB*/
                    var subcmd = this.Arguments.First().ToCast<PubSubCommand>();
                    this.OK = type == ResultType.MultiBulk;
                    switch (subcmd)
                    {
                        case PubSubCommand.CHANNELS:
                            this.Value = result;
                            break;
                        case PubSubCommand.NUMSUB:
                            this.Value = new RedisValue(result.ToDictionary<RedisValue, RedisValue>());
                            break;
                        case PubSubCommand.NUMPAT:
                            this.Value = result;
                            break;
                    }
                    break;
                case "HGETALL":/*CommandType.HGETALL*/
                case "BLPOP":/*CommandType.BLPOP*/
                case "BRPOP":/*CommandType.BRPOP*/
                    this.OK = type == ResultType.MultiBulk;
                    this.Value = new RedisValue(result.ToDictionary<RedisValue,RedisValue>());
                    break;
                case "GEOPOS":/*CommandType.GEOPOS*/
                    this.OK = type == ResultType.MultiBulk && result.RedisType == RedisType.Array;
                    if (result.RedisType == RedisType.Array)
                    {
                        var list = new List<GeoModel>();
                        result.ToList().Each(r =>
                        {
                            if (r.RedisType == RedisType.Array)
                            {
                                var rs= r.ToArray();
                                if (rs.Length <= 1)
                                    list.Add(null);
                                else
                                    list.Add(new GeoModel
                                    {
                                        Longitude = rs[0].ToDouble(),
                                        Latitude = rs[1].ToDouble()
                                    });
                            }
                            else
                                list.Add(null);
                        });
                        this.Value = new RedisValue(RedisType.Model, list);
                    }
                    break;
                case "GEORADIUS":/*CommandType.GEORADIUS*/
                case "GEORADIUSBYMEMBER":/*CommandType.GEORADIUSBYMEMBER*/
                case "GEOSEARCHSTORE":/*CommandType.GEOSEARCHSTORE*/
                    this.OK = type == ResultType.MultiBulk && result.RedisType == RedisType.Array;
                    if (result.RedisType == RedisType.Array)
                    {
                        var list = new List<GeoRadiusModel>();
                        result.ToList().Each(r =>
                        {
                            if (r.RedisType == RedisType.Array)
                            {
                                var rs = r.ToArray();
                                if (rs.Length == 0)
                                    list.Add(null);
                                else
                                {
                                    var model = new GeoRadiusModel();
                                    var index = 0;
                                    model.Address = rs[index++].ToString();
                                    if (this.Arguments.Contains("WITHDIST"))
                                    {
                                        model.Dist = rs[index++].ToDouble();
                                    }
                                    if (this.Arguments.Contains("WITHHASH"))
                                        model.Hash = rs[index++].ToString();
                                    if (this.Arguments.Contains("WITHCOORD"))
                                    {
                                        var rso = rs[index++].ToArray();
                                        model.Longitude = rso[0].ToDouble();
                                        model.Latitude = rso[1].ToDouble();
                                    }
                                    list.Add(model);
                                }
                            }
                            else
                                list.Add(null);
                        });
                        this.Value = new RedisValue(RedisType.Model, list);
                    }
                    else this.Value = null;
                    break;
                case "CLIENT":/*CommandType.CLIENT*/
                    switch (this.Arguments.First())
                    {
                        case "PAUSE":
                        case "KILL":
                        case "SETNAME":
                            this.OK = type == ResultType.Status;
                            this.Value = result;
                            break;
                        case "LIST":
                            this.OK = type == ResultType.Bulk;
                            var list = new List<ClientInfo>();
                            result.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Each(a =>
                            {
                                list.Add(a.ReplacePattern(@"\s+", "&").JsonToObject<ClientInfo>());
                            });
                            this.Value = new RedisValue(RedisType.Model, list);
                            break;
                        case "GETNAME":
                            this.OK = type == ResultType.Bulk;
                            break;
                    }
                    break;
                case "CONFIG":
                    switch (this.Arguments.First())
                    {
                        case "GET":
                            this.OK = type == ResultType.MultiBulk;
                            this.Value = new RedisValue(result.ToDictionary<RedisValue, RedisValue>());
                            break;
                        case "SET":
                        case "REWRITE":
                        case "RESETSTAT":
                            this.OK = type == ResultType.Status;
                            this.Value = result;
                            break;
                        default:

                            break;
                    }
                    break;
                case "SORT":/*CommandType.SORT*/
                    if (this.Arguments.Contains("STORE"))
                        this.OK = type == ResultType.MultiBulk;
                    else
                        this.OK = type == ResultType.Bulk;
                    this.Value = result;
                    break;
                case "XRANGE":/*CommandType.XRANGE*/
                case "XREVRANGE":/*CommandType.XREVRANGE*/
                case "XCLAIM":/*CommandType.XCLAIM*/
                    this.OK = type == ResultType.MultiBulk;
                    if (result.Length == 0)
                    {
                        this.Value = new RedisValue();
                    }
                    else
                    {
                        var dic = new Dictionary<string, Dictionary<string, string>>();
                        if (result.RedisType == RedisType.Array)
                        {
                            result.ToList().Each(r =>
                            {
                                if (r.RedisType == RedisType.Array)
                                {
                                    var arr = r.ToArray();

                                    var id = arr[0].ToString();
                                    dic.Add(id, arr.Length > 1 ? arr[1].ToDictionary<string, string>() : null);
                                }
                            });
                            this.Value = new RedisValue(RedisType.Model, dic);
                        }
                    }
                    break;
                case "XREAD":/*CommandType.XREAD*/
                case "XREADGROUP GROUP":/*CommandType.XREADGROUPGROUP*/
                    this.OK = type == ResultType.MultiBulk;
                    if (result.Length == 0)
                    {
                        this.Value = new RedisValue();
                    }
                    else
                    {
                        var dic = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                        result.ToList().Each(r =>
                        {
                            if (r.RedisType == RedisType.Array)
                            {
                                var arr = r.ToArray();
                                if (arr.Length <= 1) return;
                                var key = arr[0].ToString();
                                if (arr[1].RedisType == RedisType.Array)
                                {
                                    var dicdata = new Dictionary<string, Dictionary<string, string>>();
                                    r[1].ToArray().Each(rs =>
                                    {
                                        if (rs.RedisType == RedisType.Array)
                                        {
                                            var arrs = rs.ToArray();
                                            if (arrs.Length <= 1) return;
                                            var id = arrs[0].ToString();
                                            var arrValues = arrs[1].ToDictionary<string, string>();
                                            dicdata.Add(id, arrValues);
                                        }
                                    });
                                    dic.Add(key, dicdata);
                                }
                            }
                        });
                        this.Value = new RedisValue(RedisType.Model, dic);
                    }
                    break;
                case "XPENDING":/*CommandType.XPENDING*/
                    this.OK = type == ResultType.MultiBulk;
                    if (this.Arguments.Length == 2)
                    {
                        var model = new ConsumerGroupXPendingModel();
                        if (result.RedisType == RedisType.Array)
                        {
                            var arr = result.ToArray();
                            if (arr.Length == 4)
                            {
                                model.Count = arr[0].ToInt();
                                model.MinID = arr[1].ToString();
                                model.MaxID = arr[2].ToString();
                                model.Consumers = new List<ConsumerGroupXPendingModel.Consumer>();
                                if (arr[3].RedisType == RedisType.Array)
                                {
                                    var arrs = arr[3].ToArray();
                                    if (arrs.Length > 0)
                                    {
                                        arrs.Each(a =>
                                        {
                                            var consumer = new ConsumerGroupXPendingModel.Consumer();
                                            if (a.RedisType == RedisType.Array && a.Length > 0)
                                            {
                                                var rs = a.ToArray();
                                                consumer.ConsumerName = rs[0].ToString();
                                                consumer.Count = rs[1].ToInt();
                                                model.Consumers.Add(consumer);
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        this.Value = new RedisValue(RedisType.Model, model);
                    }
                    else
                    {
                        var list = new List<ConsumerGroupXPendingConsumerModel>();
                        if (result.RedisType == RedisType.Array)
                        {
                            result.ToList().Each(a =>
                            {
                                if (a.RedisType == RedisType.Array)
                                {
                                    var rs = a.ToArray();
                                    if (rs.Length == 4)
                                    {
                                        list.Add(new ConsumerGroupXPendingConsumerModel
                                        {
                                            MessageID = rs[0].ToString(),
                                            ConsumerName = rs[1].ToString(),
                                            Milliseconds = rs[2].ToLong(),
                                            Count = rs[3].ToInt()
                                        });
                                    }
                                }
                            });
                        }
                        this.Value = new RedisValue(RedisType.Model, list);
                    }
                    break;
                case "XINFO CONSUMERS":/*CommandType.XINFOCONSUMERS*/
                    this.OK = type == ResultType.MultiBulk;
                    if (this.OK)
                    {
                        var list = new List<ConsumerGroupXInfoConsumerModel>();
                        result.ToList().Each(r =>
                        {
                            if(r.RedisType == RedisType.Array)
                            {
                                var arr = r.ToArray();
                                if (arr.Length == 6)
                                {
                                    list.Add(new ConsumerGroupXInfoConsumerModel
                                    {
                                         Name = arr[1].ToString(),
                                         Pending = arr[3].ToInt(),
                                         Idle = arr[5].ToInt()
                                    });
                                }
                            }
                        });
                        this.Value = new RedisValue(RedisType.Model, list);
                    }
                    break;
                case "XINFO GROUPS":/*CommandType.XINFOGROUPS*/
                    this.OK = type == ResultType.MultiBulk;
                    if (this.OK)
                    {
                        var list = new List<ConsumerGroupXInfoGroupsModel>();
                        result.ToList().Each(r =>
                        {
                            if (r.RedisType == RedisType.Array)
                            {
                                var arr = r.ToDictionary<string, string>();
                                if (arr == null || arr.Count == 0) return;
                                var groups = new ConsumerGroupXInfoGroupsModel();
                                if (arr.ContainsKey("name"))
                                    groups.Name = arr["name"];
                                if (arr.ContainsKey("consumers"))
                                    groups.Consumers = arr["consumers"].ToCast<int>();
                                if (arr.ContainsKey("pending"))
                                    groups.Pending = arr["pending"].ToCast<int>();
                                if (arr.ContainsKey("last-delivered-id"))
                                    groups.LastDeliveredID = arr["last-delivered-id"];
                                if (arr.ContainsKey("entries-read"))
                                    groups.EntriesRead = arr["entries-read"].ToCast<int>();
                                if (arr.ContainsKey("lag"))
                                    groups.Lag = arr["lag"].ToCast<int>();
                                list.Add(groups);
                            }
                        });
                        this.Value = new RedisValue(RedisType.Model, list);
                    }
                    break;
                case "XINFO STREAM":/*CommandType.XINFOSTREAM*/
                    this.OK = type == ResultType.MultiBulk;
                    if (this.OK)
                    {
                        if (this.Arguments.Length == 1)
                        {
                            if (result.RedisType == RedisType.Array)
                            {
                                var stream = new ConsumerGroupXInfoStreamModel();
                                var arr = result.ToDictionary<string, RedisValue>();
                                if (arr?.Count > 0)
                                {
                                    if (arr.ContainsKey("length"))
                                        stream.Length = arr["length"].ToInt();
                                    if (arr.ContainsKey("radix-tree-keys"))
                                        stream.RadixTreeKeys = arr["radix-tree-keys"].ToCast<int>();
                                    if (arr.ContainsKey("radix-tree-nodes"))
                                        stream.RadixTreeNodes = arr["radix-tree-nodes"].ToCast<int>();
                                    if (arr.ContainsKey("last-generated-id"))
                                        stream.LastGeneratedID = arr["last-generated-id"].ToString();
                                    if (arr.ContainsKey("max-deleted-entry-id"))
                                        stream.MaxDeletedEntryID = arr["max-deleted-entry-id"].ToString();
                                    if (arr.ContainsKey("entries-added"))
                                        stream.EntriesAdded = arr["entries-added"].ToCast<int>();
                                    if (arr.ContainsKey("groups"))
                                        stream.Groups = arr["groups"].ToCast<int>();
                                    if (arr.ContainsKey("first-entry"))
                                    {
                                        var vals = arr["first-entry"];
                                        if (vals.RedisType == RedisType.Array)
                                        {
                                            var first = new ConsumerGroupXInfoStreamModel.Entries();
                                            var val = vals.ToArray();
                                            if (val.Length == 2)
                                            {
                                                first.ID = val[0].ToString();
                                                first.Value = val[1].ToDictionary<string, string>();
                                                stream.FirstEntry = first;
                                            }
                                        }
                                    }
                                    if (arr.ContainsKey("last-entry"))
                                    {
                                        var vals = arr["last-entry"];
                                        if (vals.RedisType == RedisType.Array)
                                        {
                                            var last = new ConsumerGroupXInfoStreamModel.Entries();
                                            var val = vals.ToArray();
                                            if (val.Length == 2)
                                            {
                                                last.ID = val[0].ToString();
                                                last.Value = val[1].ToDictionary<string, string>();
                                                stream.LastEntry = last;
                                            }
                                        }
                                    }
                                }
                                this.Value = new RedisValue(RedisType.Model, stream);
                            }
                        }
                        else
                        {
                            var stream = new ConsumerGroupXInfoStreamFullModel();
                            var arr = result.ToDictionary<string, RedisValue>();
                            if (arr?.Count > 0)
                            {
                                if (arr.ContainsKey("length"))
                                    stream.Length = arr["length"].ToInt();
                                if (arr.ContainsKey("radix-tree-keys"))
                                    stream.RadixTreeKeys = arr["radix-tree-keys"].ToCast<int>();
                                if (arr.ContainsKey("radix-tree-nodes"))
                                    stream.RadixTreeNodes = arr["radix-tree-nodes"].ToCast<int>();
                                if (arr.ContainsKey("last-generated-id"))
                                    stream.LastGeneratedID = arr["last-generated-id"].ToString();
                                if (arr.ContainsKey("max-deleted-entry-id"))
                                    stream.MaxDeletedEntryID = arr["max-deleted-entry-id"].ToString();
                                if (arr.ContainsKey("entries-added"))
                                    stream.EntriesAdded = arr["entries-added"].ToCast<int>();
                                if (arr.ContainsKey("entries"))
                                {
                                    var entires = new List<ConsumerGroupXInfoStreamFullModel.Entries>(); ;
                                    stream.Entires = entires;
                                    var vals = arr["entries"];
                                    if(vals.RedisType== RedisType.Array)
                                    {
                                        vals.ToList().Each(r =>
                                        {
                                            if(r.RedisType== RedisType.Array)
                                            {
                                                var val = r.ToArray();
                                                entires.Add(new ConsumerGroupXInfoStreamFullModel.Entries
                                                {
                                                    ID = r[0].ToString(),
                                                    Value= val.Length > 1 ? val[1].ToDictionary<string, string>() : null
                                                });
                                            }
                                        });
                                    }
                                }
                                if (arr.ContainsKey("groups"))
                                {
                                    var groups = arr["groups"];
                                    var list = new List<ConsumerGroupXInfoStreamFullModel.FullGroups>();
                                    stream.Groups = list;
                                    if(groups.RedisType== RedisType.Array)
                                    {
                                        groups.ToList().Each(r =>
                                        {
                                            var dic = r.ToDictionary<string, RedisValue>();
                                            var g = new ConsumerGroupXInfoStreamFullModel.FullGroups();
                                            if (dic.ContainsKey("name"))
                                                g.Name = dic["name"].ToString();
                                            if (dic.ContainsKey("last-delivered-id"))
                                                g.LastDeliveredID = dic["last-delivered-id"].ToString();
                                            if (dic.ContainsKey("entries-read"))
                                                g.EntriesRead = dic["entries-read"].ToInt();
                                            if (dic.ContainsKey("lag"))
                                                g.Lag = dic["lag"].ToInt();
                                            if (dic.ContainsKey("pel-count"))
                                                g.PelCount = dic["pel-count"].ToInt();
                                            if (dic.ContainsKey("pending"))
                                            {
                                                var p = dic["pending"];
                                                if(p.RedisType== RedisType.Array)
                                                {
                                                    var pending = new List<ConsumerGroupXPendingConsumerModel>();
                                                    g.Pending = pending;
                                                    p.ToList().Each(ra =>
                                                    {
                                                        if (ra.RedisType == RedisType.Array)
                                                        {
                                                            var raArr = ra.ToArray();
                                                            if (raArr.Length == 4)
                                                            {
                                                                pending.Add(new ConsumerGroupXPendingConsumerModel
                                                                {
                                                                    MessageID = raArr[0].ToString(),
                                                                    ConsumerName = raArr[1].ToString(),
                                                                    Milliseconds = raArr[2].ToLong(),
                                                                    Count = raArr[3].ToInt()        
                                                                });
                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                            if (dic.ContainsKey("consumers"))
                                            {
                                                var csv = dic["consumers"];
                                                var cs = new List<ConsumerGroupXInfoStreamFullModel.FullCounsumer>();
                                                g.Consumers = cs;
                                                if(csv.RedisType== RedisType.Array)
                                                {
                                                    csv.ToList().Each(rs =>
                                                    {
                                                        if(rs.RedisType== RedisType.Array)
                                                        {
                                                            var cdic = rs.ToDictionary<string, RedisValue>();
                                                            var c = new ConsumerGroupXInfoStreamFullModel.FullCounsumer();
                                                            if (cdic.ContainsKey("name"))
                                                                c.Name = cdic["name"].ToString();
                                                            if (cdic.ContainsKey("seen-time"))
                                                                c.SeenTime = cdic["seen-time"].ToLong();
                                                            if (cdic.ContainsKey("pel-count"))
                                                                c.PelCount = cdic["pel-count"].ToInt();
                                                            if (cdic.ContainsKey("pending"))
                                                            {
                                                                var pending = new List<ConsumerGroupXPendingConsumerModel>();
                                                                g.Pending = pending;
                                                                var p = cdic["pending"];
                                                                p.ToList().Each(ra =>
                                                                {
                                                                    if (ra.RedisType == RedisType.Array)
                                                                    {
                                                                        var raArr = ra.ToArray();
                                                                        if (raArr.Length == 4)
                                                                        {
                                                                            pending.Add(new ConsumerGroupXPendingConsumerModel
                                                                            {
                                                                                MessageID = raArr[0].ToString(),
                                                                                ConsumerName = raArr[1].ToString(),
                                                                                Milliseconds = raArr[2].ToLong(),
                                                                                Count = raArr[3].ToInt()
                                                                            });
                                                                        }
                                                                    }
                                                                });
                                                            }
                                                            cs.Add(c);
                                                        }
                                                    });
                                                }
                                            }
                                            list.Add(g);
                                        });
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:
                    /*
                     * CommandType.EVAL
                     * CommandType.SCRIPTEXISTS
                     */
                    this.OK = true;
                    this.Value = result;
                    break;
            }
            return this.Value;
        }
        #endregion

        #endregion

        #region 读取类型
        /// <summary>
        /// 读取类型
        /// +OK\r\n 成功
        /// -ERR 出错  -开头的为错误
        /// $1\r\n数据\r\n 单条数据
        /// *2\r\n 多条数据
        /// :2\r\n 整型数字
        /// </summary>
        /// <returns>类型</returns>
        public ResultType ReadType() {
            try
            {
                lock (StreamLock)
                    return (ResultType)this.Reader.ReadByte();
            }
            catch (IOException ie)
            {
                LogHelper.Error(ie);
                return ResultType.Error;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ResultType.Error;
            }
        }
        #endregion

        #region 读取状态
        /// <summary>
        /// 读取状态
        /// +OK\r\n 成功
        /// </summary>
        /// <returns>状态</returns>
        public RedisValue ReadStatus() => this.ReadLine();
        #endregion

        #region 读取错误信息
        /// <summary>
        /// 读取错误信息
        /// -ERR 出错  -开头的为错误
        /// </summary>
        /// <returns>错误信息</returns>
        public RedisValue ReadError() => this.ReadLine().RemovePattern(@"ERR\s+");
        #endregion

        #region 读取整型
        /// <summary>
        /// 读取整型
        /// :2\r\n 整型数字
        /// </summary>
        /// <returns>整型数</returns>
        public RedisValue ReadInt() => this.ReadLine().ToCast<long>();
        #endregion

        #region 是否结尾
        /// <summary>
        /// 是否结尾
        /// </summary>
        /// <exception cref="Exception">抛出异常</exception>
        public void ReadEOF()
        {
            var r = this.Reader.ReadByte();
            var n = this.Reader.ReadByte();
            if (r == -1 && n == -1) return;
            if (r != 13 || n != 10)
                throw new Exception($"结尾字符不是\r\n而是{(char)r}{(char)n}.");
        }
        #endregion

        #region 读取单条数据
        /// <summary>
        /// 读取单条数据
        /// $1\r\n数据\r\n 单条数据
        /// </summary>
        /// <returns>单条数据</returns>
        public RedisValue ReadBulkString() => this.ReadBulkBytes().GetString();
        #endregion

        #region 读取单条数据
        /// <summary>
        /// 读取单条数据
        /// $1\r\n数据\r\n 单条数据
        /// </summary>
        /// <returns>单条数据</returns>
        public byte[] ReadBulkBytes()
        {
            var size = this.ReadInt().ToInt();
            var vals = this.ReadLineBytes();
            return vals;
        }
        #endregion

        #region 读取多行数据
        /// <summary>
        /// 读取多行数据
        /// *2\r\n 多条数据
        /// </summary>
        /// <returns>数据</returns>
        public List<String> ReadMultiBulkString() => (from a in this.ReadMultiBulkBytes() select a.GetString()).ToList();
        #endregion

        #region 读取多行数据
        /// <summary>
        /// 读取多行数据
        /// *2\r\n 多条数据
        /// </summary>
        /// <returns>数据</returns>
        public List<byte[]> ReadMultiBulkBytes()
        {
            var size = this.ReadInt().ToInt();
            if (size <= -1) return null;
            var list = new List<byte[]>();
            for (int i = 0; i < size; i++)
                list.Add(this.ReadBulkBytes());
            return list;
        }
        /// <summary>
        /// 读取多行数据
        /// </summary>
        /// <param name="size">数据块数量</param>
        /// <returns></returns>
        public RedisValue ReadMultiBulk(int size = -1)
        {
            size = size == -1 ? this.ReadInt().ToInt() : size;
            if (size <= -1) return new RedisValue();
            var value = new RedisValue(new List<RedisValue>());
            for(var i = 0; i < size; i++)
                value.Add(this.ReadValue());
            return value;
        }
        #endregion

        #region 读取内容
        /// <summary>
        /// 读取内容
        /// </summary>
        /// <returns>一行数据</returns>
        public string ReadLine() => this.ReadLineBytes().GetString();
        #endregion

        #region 读取内容
        /// <summary>
        /// 读取内容
        /// </summary>
        /// <returns>一行数据</returns>
        public byte[] ReadLineBytes()
        {
            /*if (this.Reader.Position == this.Reader.Length) return Array.Empty<byte>();
            var should_break = false;
            var length = this.Reader.Length;
            var list = new List<byte>();
            while (this.Reader.Position < length)
            {
                var b = this.Reader.ReadByte();
                var c = (char)b;
                if (c == '\r')
                    should_break = true;
                else if (c == '\n' && should_break)
                    break;
                else
                {
                    list.Add((byte)b);
                    should_break = false;
                }
            }
            return list.ToArray();*/
            var reader = this.Reader as NetworkStream;
            var should_break = false;
            var list = new List<byte>();
            while (reader.DataAvailable && reader.CanRead)
            {
                var b = this.Reader.ReadByte();
                var c = (char)b;
                if (c == '\r')
                    should_break = true;
                else if (c == '\n' && should_break)
                    break;
                else
                {
                    list.Add((byte)b);
                    should_break = false;
                }
            }
            return list.ToArray();
        }
        #endregion
    }
}