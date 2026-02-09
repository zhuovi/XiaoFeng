using System;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-09 19:39:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 执行命令
    /// *参数个数 CR LF
    /// $第一个参数字节长度 CR LF
    /// 第一个参数 CR LF
    /// ...
    /// $第N个参数字节长度 CR LF
    /// 第N个参数数据 CR LF
    /// </summary>
    public class CommandType : IEquatable<CommandType>, IComparable, IComparable<CommandType>, IFormattable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CommandType() { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="format">格式</param>
        public CommandType(string name, string format) : this(name, format, "") { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="format">格式</param>
        /// <param name="commands">命令组</param>
        public CommandType(string name, string format, string[] commands) : this(name, format, "", commands) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="format">格式</param>
        /// <param name="desc">说明</param>
        public CommandType(string name, string format, string desc) : this(name, format, desc, null) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="format">格式</param>
        /// <param name="desc">说明</param>
        /// <param name="commands">命令组</param>
        public CommandType(string name,string format,string desc, string[] commands)
        {
            this.Name = name;
            this.Format = format;
            this.Description = desc;
            this.Commands = commands;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 命令组
        /// </summary>
        public string[] Commands { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        #region Redis 键(key)
        /// <summary>
        /// 该命令用于在 key 存在时删除 key。
        /// </summary>
        public static CommandType DEL => new CommandType("DEL", "DEL key","删除key");
        /// <summary>
        /// 该命令用于在 key 的复制。
        /// </summary>
        public static CommandType COPY => new CommandType("COPY", "COPY source destination [DB destination-db] [REPLACE]","复制key");
        /// <summary>
        /// 序列化给定 key,并返回被序列化的值。
        /// </summary>
        public static CommandType DUMP => new CommandType("DUMP", "DUMP key", "序列化给定 key,并返回被序列化的值");
        /// <summary>
        /// 检查给定 key 是否存在。
        /// </summary>
        public static CommandType EXISTS => new CommandType("EXISTS", "EXISTS key", "检查给定 key 是否存在");
        /// <summary>
        /// 为给定 key 设置过期时间，以秒计。
        /// </summary>
        public static CommandType EXPIRE => new CommandType("EXPIRE", "EXPIRE key seconds", "为给定 key 设置过期时间，以秒计");
        /// <summary>
        /// EXPIREAT 的作用和 EXPIRE 类似，都用于为 key 设置过期时间。 不同在于 EXPIREAT 命令接受的时间参数是 UNIX 时间戳(unix timestamp)。
        /// </summary>
        public static CommandType EXPIREAT => new CommandType("EXPIREAT", "EXPIREAT key timestamp", "以秒计时间戳设置过期");
        /// <summary>
        /// 设置 key 的过期时间以毫秒计。
        /// </summary>
        public static CommandType PEXPIRE => new CommandType("PEXPIRE", "PEXPIRE key milliseconds", "以毫秒计设置过期");
        /// <summary>
        /// 设置 key 过期时间的时间戳(unix timestamp) 以毫秒计。
        /// </summary>
        public static CommandType PEXPIREAT => new CommandType("PEXPIREAT", "PEXPIREAT key milliseconds-timestamp", "以毫秒计时间戳设置过期");
        /// <summary>
        /// 查找所有符合给定模式(pattern)的 key 。
        /// </summary>
        public static CommandType KEYS => new CommandType("KEYS", "KEYS pattern", "查找所有符合给定模式(pattern)的 key");
        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中。
        /// </summary>
        public static CommandType MOVE => new CommandType("MOVE", "MOVE key db", "将当前数据库的 key 移动到给定的数据库 db 当中");
        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持。
        /// </summary>
        public static CommandType PERSIST => new CommandType("PERSIST", "PERSIST key", "移除 key 的过期时间,key 将持久保持");
        /// <summary>
        /// 以毫秒为单位返回 key 的剩余的过期时间。
        /// </summary>
        public static CommandType PTTL => new CommandType("PTTL", "PTTL key", "以毫秒为单位返回 key 的剩余的过期时间");
        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。
        /// </summary>
        public static CommandType TTL => new CommandType("TTL", "TTL key", "以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)");
        /// <summary>
        /// 从当前数据库中随机返回一个 key 。
        /// </summary>
        public static CommandType RANDOMKEY => new CommandType("RANDOMKEY", "", "从当前数据库中随机返回一个 key");
        /// <summary>
        /// 修改 key 的名称
        /// </summary>
        public static CommandType RENAME => new CommandType("RENAME", "RENAME key newkey", "修改 key 的名称");
        /// <summary>
        /// 仅当 newkey 不存在时，将 key 改名为 newkey 。
        /// </summary>
        public static CommandType RENAMENX => new CommandType("RENAMENX", "RENAMENX key newkey", "仅当 newkey 不存在时，将 key 改名为 newkey");
        /// <summary>
        /// 返回 key 所储存的值的类型。
        /// </summary>
        public static CommandType TYPE => new CommandType("TYPE", "TYPE key", "返回 key 所储存的值的类型");
        /// <summary>
        /// 迭代数据库中的数据库键。
        /// </summary>
        public static CommandType SCAN => new CommandType("SCAN", "SCAN cursor [MATCH pattern] [COUNT count]", "迭代数据库中的数据库键");
        #endregion

        #region Redis 字符串(String)
        /// <summary>
        /// 设置指定 key 的值
        /// </summary>
        public static CommandType SET => new CommandType("SET", "SET key value", "设置指定 key 的值");
        /// <summary>
        /// 获取指定 key 的值。
        /// </summary>
        public static CommandType GET => new CommandType("GET", "GET key", "获取指定 key 的值");
        /// <summary>
        /// 返回 key 中字符串值的子字符
        /// </summary>
        public static CommandType GETRANGE => new CommandType("GETRANGE", "GETRANGE key start end", "返回 key 中字符串值的子字符");
        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)。
        /// </summary>
        public static CommandType GETSET => new CommandType("GETSET", "GETSET key value", "将给定 key 的值设为 value ，并返回 key 的旧值(old value)");
        /// <summary>
        /// 获取 key 的值 并移除key的值 6.2.0后可用
        /// </summary>
        public static CommandType GETDEL => new CommandType("GETDEL", "GETDEL key", "获取 key 的值 并移除key的值");
        /// <summary>
        /// 对 key 所储存的字符串值，获取指定偏移量上的位(bit)。
        /// </summary>
        public static CommandType GETBIT => new CommandType("GETBIT", "GETBIT key offset", "对 key 所储存的字符串值，获取指定偏移量上的位(bit)");
        /// <summary>
        /// 获取所有(一个或多个)给定 key 的值。
        /// </summary>
        public static CommandType MGET => new CommandType("MGET", "MGET key1 [key2..]", "获取所有(一个或多个)给定 key 的值");
        /// <summary>
        /// 对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)。
        /// </summary>
        public static CommandType SETBIT => new CommandType("SETBIT", "SETBIT key offset value", "对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)");
        /// <summary>
        /// 将值 value 关联到 key ，并将 key 的过期时间设为 seconds (以秒为单位)。
        /// </summary>
        public static CommandType SETEX => new CommandType("SETEX", "SETEX key seconds value", "将值 value 关联到 key ，并将 key 的过期时间设为 seconds (以秒为单位)");
        /// <summary>
        /// 只有在 key 不存在时设置 key 的值。
        /// </summary>
        public static CommandType SETNX => new CommandType("SETNX", "SETNX key value", "只有在 key 不存在时设置 key 的值");
        /// <summary>
        /// 用 value 参数覆写给定 key 所储存的字符串值，从偏移量 offset 开始。
        /// </summary>
        public static CommandType SETRANGE => new CommandType("SETRANGE", "SETRANGE key offset value", "用 value 参数覆写给定 key 所储存的字符串值，从偏移量 offset 开始");
        /// <summary>
        /// 返回 key 所储存的字符串值的长度。
        /// </summary>
        public static CommandType STRLEN => new CommandType("STRLEN", "STRLEN key", "返回 key 所储存的字符串值的长度");
        /// <summary>
        /// 同时设置一个或多个 key-value 对。
        /// </summary>
        public static CommandType MSET => new CommandType("MSET", "MSET key value [key value ...]", "同时设置一个或多个 key-value 对");
        /// <summary>
        /// 同时设置一个或多个 key-value 对，当且仅当所有给定 key 都不存在。
        /// </summary>
        public static CommandType MSETNX => new CommandType("MSETNX", "	MSETNX key value [key value ...]", "同时设置一个或多个 key-value 对，当且仅当所有给定 key 都不存在");
        /// <summary>
        /// 这个命令和 SETEX 命令相似，但它以毫秒为单位设置 key 的生存时间，而不是像 SETEX 命令那样，以秒为单位。
        /// </summary>
        public static CommandType PSETEX => new CommandType("PSETEX", "PSETEX key milliseconds value", "这个命令和 SETEX 命令相似，但它以毫秒为单位设置 key 的生存时间，而不是像 SETEX 命令那样，以秒为单位");
        /// <summary>
        /// 将 key 中储存的数字值增一。
        /// </summary>
        public static CommandType INCR => new CommandType("INCR", "INCR key", "将 key 中储存的数字值增一");
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 。
        /// </summary>
        public static CommandType INCRBY => new CommandType("INCRBY", "INCRBY key increment", "将 key 所储存的值加上给定的增量值（increment）");
        /// <summary>
        /// 将 key 所储存的值加上给定的浮点增量值（increment） 。
        /// </summary>
        public static CommandType INCRBYFLOAT => new CommandType("INCRBYFLOAT", "INCRBYFLOAT key increment", "将 key 所储存的值加上给定的浮点增量值（increment）");
        /// <summary>
        /// 将 key 中储存的数字值减一。
        /// </summary>
        public static CommandType DECR => new CommandType("DECR", "DECR key", "将 key 中储存的数字值减一");
        /// <summary>
        /// key 所储存的值减去给定的减量值（decrement）。
        /// </summary>
        public static CommandType DECRBY => new CommandType("DECRBY", "DECRBY key decrement", "key 所储存的值减去给定的减量值（decrement）");
        /// <summary>
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将指定的 value 追加到该 key 原来值（value）的末尾。
        /// </summary>
        public static CommandType APPEND => new CommandType("APPEND", "APPEND key value", "如果 key 已经存在并且是一个字符串， APPEND 命令将指定的 value 追加到该 key 原来值（value）的末尾");
        #endregion

        #region Redis hash 命令
        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        public static CommandType HDEL => new CommandType("HDEL", "HDEL key field1 [field2]", "删除一个或多个哈希表字段");
        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在。
        /// </summary>
        public static CommandType HEXISTS => new CommandType("HEXISTS", "HEXISTS key field", "查看哈希表 key 中，指定的字段是否存在");
        /// <summary>
        /// 获取存储在哈希表中指定字段的值。
        /// </summary>
        public static CommandType HGET => new CommandType("HGET", "HGET key field", "获取存储在哈希表中指定字段的值");
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        public static CommandType HGETALL => new CommandType("HGETALL", "HGETALL key", "获取在哈希表中指定 key 的所有字段和值");
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment 。
        /// </summary>
        public static CommandType HINCRBY => new CommandType("HINCRBY", "HINCRBY key field increment", "为哈希表 key 中的指定字段的整数值加上增量 increment");
        /// <summary>
        /// 为哈希表 key 中的指定字段的浮点数值加上增量 increment 。
        /// </summary>
        public static CommandType HINCRBYFLOAT => new CommandType("HINCRBYFLOAT", "HINCRBYFLOAT key field increment", "为哈希表 key 中的指定字段的浮点数值加上增量 increment");
        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        public static CommandType HKEYS => new CommandType("HKEYS", "HKEYS key", "获取所有哈希表中的字段");
        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        public static CommandType HLEN => new CommandType("HLEN", "HLEN key", "获取哈希表中字段的数量");
        /// <summary>
        /// 获取所有给定字段的值
        /// </summary>
        public static CommandType HMGET => new CommandType("HMGET", "HMGET key field1 [field2]", "获取所有给定字段的值");
        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中。
        /// </summary>
        public static CommandType HMSET => new CommandType("HMSET", "HMSET key field1 value1 [field2 value2 ]", "同时将多个 field-value (域-值)对设置到哈希表 key 中");
        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value 。
        /// </summary>
        public static CommandType HSET => new CommandType("HSET", "HSET key field value", "将哈希表 key 中的字段 field 的值设为 value");
        /// <summary>
        /// 只有在字段 field 不存在时，设置哈希表字段的值。
        /// </summary>
        public static CommandType HSETNX => new CommandType("HSETNX", "HSETNX key field value", "只有在字段 field 不存在时，设置哈希表字段的值");
        /// <summary>
        /// 获取哈希表中所有值。
        /// </summary>
        public static CommandType HVALS => new CommandType("HVALS", "HVALS key", "获取哈希表中所有值");
        /// <summary>
        /// 迭代哈希表中的键值对。
        /// </summary>
        public static CommandType HSCAN => new CommandType("HSCAN", "HSCAN key cursor [MATCH pattern] [COUNT count]", "迭代哈希表中的键值对");
        #endregion

        #region Redis 列表命令
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        public static CommandType BLPOP => new CommandType("BLPOP", "BLPOP key1 [key2 ] timeout", "移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止");
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        public static CommandType BRPOP => new CommandType("BRPOP", "BRPOP key1 [key2 ] timeout", "移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止");
        /// <summary>
        /// 从列表中弹出一个值，将弹出的元素插入到另外一个列表中并返回它； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        public static CommandType BRPOPLPUSH => new CommandType("BRPOPLPUSH", "BRPOPLPUSH source destination timeout", "从列表中弹出一个值，将弹出的元素插入到另外一个列表中并返回它； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止");
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        public static CommandType LINDEX => new CommandType("LINDEX", "LINDEX key index", "通过索引获取列表中的元素");
        /// <summary>
        /// 在列表的元素前或者后插入元素
        /// </summary>
        public static CommandType LINSERT => new CommandType("LINSERT", "LINSERT key BEFORE|AFTER pivot value", "在列表的元素前或者后插入元素");
        /// <summary>
        /// 获取列表长度
        /// </summary>
        public static CommandType LLEN => new CommandType("LLEN", "LLEN key", "获取列表长度");
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        public static CommandType LPOP => new CommandType("LPOP", "	LPOP key", "移出并获取列表的第一个元素");
        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        public static CommandType LPUSH => new CommandType("LPUSH", "LPUSH key value1 [value2]", "将一个或多个值插入到列表头部");
        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        public static CommandType LPUSHX => new CommandType("LPUSHX", "LPUSHX key value", "将一个值插入到已存在的列表头部");
        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        public static CommandType LRANGE => new CommandType("LRANGE", "LRANGE key start stop", "获取列表指定范围内的元素");
        /// <summary>
        /// 移除列表元素
        /// </summary>
        public static CommandType LREM => new CommandType("LREM", "LREM key count value", "移除列表元素");
        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        public static CommandType LSET => new CommandType("LSET", "LSET key index value", "通过索引设置列表元素的值");
        /// <summary>
        /// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// </summary>
        public static CommandType LTRIM => new CommandType("LTRIM", "LTRIM key start stop", "对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除");
        /// <summary>
        /// 移除列表的最后一个元素，返回值为移除的元素。
        /// </summary>
        public static CommandType RPOP => new CommandType("RPOP", "RPOP key", "移除列表的最后一个元素，返回值为移除的元素");
        /// <summary>
        /// 移除列表的最后一个元素，并将该元素添加到另一个列表并返回
        /// </summary>
        public static CommandType RPOPLPUSH => new CommandType("RPOPLPUSH", "RPOPLPUSH source destination", "移除列表的最后一个元素，并将该元素添加到另一个列表并返回");
        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        public static CommandType RPUSH => new CommandType("RPUSH", "RPUSH key value1 [value2]", "在列表中添加一个或多个值");
        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        public static CommandType RPUSHX => new CommandType("RPUSHX", "RPUSHX key value", "为已存在的列表添加值");
        #endregion

        #region Redis 集合(Set)
        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        public static CommandType SADD => new CommandType("SADD", "SADD key member1 [member2]", "向集合添加一个或多个成员");
        /// <summary>
        /// 获取集合的成员数
        /// </summary>
        public static CommandType SCARD => new CommandType("SCARD", "SCARD key", "获取集合的成员数");
        /// <summary>
        /// 返回第一个集合与其他集合之间的差异。
        /// </summary>
        public static CommandType SDIFF => new CommandType("SDIFF", "SDIFF key1 [key2]", "返回第一个集合与其他集合之间的差异");
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        public static CommandType SDIFFSTORE => new CommandType("SDIFFSTORE", "SDIFFSTORE destination key1 [key2]", "返回给定所有集合的差集并存储在 destination 中");
        /// <summary>
        /// 返回给定所有集合的交集
        /// </summary>
        public static CommandType SINTER => new CommandType("SINTER", "SINTER key1 [key2]", "返回给定所有集合的交集");
        /// <summary>
        /// 返回给定所有集合的交集并存储在 destination 中
        /// </summary>
        public static CommandType SINTERSTORE => new CommandType("SINTERSTORE", "SINTERSTORE destination key1 [key2]", "返回给定所有集合的交集并存储在 destination 中");
        /// <summary>
        /// 判断 member 元素是否是集合 key 的成员
        /// </summary>
        public static CommandType SISMEMBER => new CommandType("SISMEMBER", "SISMEMBER key member", "判断 member 元素是否是集合 key 的成员");
        /// <summary>
        /// 返回集合中的所有成员
        /// </summary>
        public static CommandType SMEMBERS => new CommandType("SMEMBERS", "SMEMBERS key", "返回集合中的所有成员");
        /// <summary>
        /// 移除并返回集合中的一个随机元素
        /// </summary>
        public static CommandType SPOP => new CommandType("SPOP", "SPOP key", "移除并返回集合中的一个随机元素");
        /// <summary>
        /// 返回集合中一个或多个随机数
        /// </summary>
        public static CommandType SRANDMEMBER => new CommandType("SRANDMEMBER", "SRANDMEMBER key [count]", "返回集合中一个或多个随机数");
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        public static CommandType SREM => new CommandType("SREM", "SREM key member1 [member2]", "移除集合中一个或多个成员");
        /// <summary>
        /// 返回所有给定集合的并集
        /// </summary>
        public static CommandType SUNION => new CommandType("SUNION", "SUNION key1 [key2]", "返回所有给定集合的并集");
        /// <summary>
        /// 所有给定集合的并集存储在 destination 集合中
        /// </summary>
        public static CommandType SUNIONSTORE => new CommandType("SUNIONSTORE", "SUNIONSTORE destination key1 [key2]", "所有给定集合的并集存储在 destination 集合中");
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        public static CommandType SMOVE => new CommandType("SMOVE", "SMOVE source destination member", "将 member 元素从 source 集合移动到 destination 集合");
        /// <summary>
        /// 迭代集合中的元素
        /// </summary>
        public static CommandType SSCAN => new CommandType("SSCAN", "SSCAN key cursor [MATCH pattern] [COUNT count]", "迭代集合中的元素");
        #endregion

        #region Redis 有序集合命令
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        public static CommandType ZADD => new CommandType("ZADD", "ZADD key score1 member1 [score2 member2]", "向有序集合添加一个或多个成员，或者更新已存在成员的分数");
        /// <summary>
        /// 获取有序集合的成员数
        /// </summary>
        public static CommandType ZCARD => new CommandType("ZCARD", "ZCARD key", "获取有序集合的成员数");
        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        public static CommandType ZCOUNT => new CommandType("ZCOUNT", "ZCOUNT key min max", "计算在有序集合中指定区间分数的成员数");
        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        public static CommandType ZINCRBY => new CommandType("ZINCRBY", "ZINCRBY key increment member", "有序集合中对指定成员的分数加上增量 increment");
        /// <summary>
        /// 计算给定的一个或多个有序集的交集并将结果集存储在新的有序集合 destination 中
        /// </summary>
        public static CommandType ZINTERSTORE => new CommandType("ZINTERSTORE", "ZINTERSTORE destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX]", "计算给定的一个或多个有序集的交集并将结果集存储在新的有序集合");
        /// <summary>
        /// 在有序集合中计算指定字典区间内成员数量
        /// </summary>
        public static CommandType ZLEXCOUNT => new CommandType("ZLEXCOUNT", "ZLEXCOUNT key min max", "在有序集合中计算指定字典区间内成员数量");
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员
        /// </summary>
        public static CommandType ZRANGE => new CommandType("ZRANGE", "ZRANGE key start stop [WITHSCORES]", "通过索引区间返回有序集合指定区间内的成员");
        /// <summary>
        /// 通过字典区间返回有序集合的成员
        /// </summary>
        public static CommandType ZRANGEBYLEX => new CommandType("ZRANGEBYLEX", "ZRANGEBYLEX key min max [LIMIT offset count]", "通过字典区间返回有序集合的成员");
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        public static CommandType ZRANGEBYSCORE => new CommandType("ZRANGEBYSCORE", "ZRANGEBYSCORE key min max [WITHSCORES] [LIMIT offset count]", "通过分数返回有序集合指定区间内的成员");
        /// <summary>
        /// 返回有序集合中指定成员的索引
        /// </summary>
        public static CommandType ZRANK => new CommandType("ZRANK", "ZRANK key member", "返回有序集合中指定成员的索引");
        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        public static CommandType ZREM => new CommandType("ZREM", "ZREM key member [member ...]", "移除有序集合中的一个或多个成员");
        /// <summary>
        /// 移除有序集合中给定的字典区间的所有成员
        /// </summary>
        public static CommandType ZREMRANGEBYLEX => new CommandType("ZREMRANGEBYLEX", "ZREMRANGEBYLEX key min max", "移除有序集合中给定的字典区间的所有成员");
        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        public static CommandType ZREMRANGEBYRANK => new CommandType("ZREMRANGEBYRANK", "ZREMRANGEBYRANK key start stop", "移除有序集合中给定的排名区间的所有成员");
        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        public static CommandType ZREMRANGEBYSCORE => new CommandType("ZREMRANGEBYSCORE", "ZREMRANGEBYSCORE key min max", "移除有序集合中给定的分数区间的所有成员");
        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引，分数从高到低
        /// </summary>
        public static CommandType ZREVRANGE => new CommandType("ZREVRANGE", "ZREVRANGE key start stop [WITHSCORES]", "返回有序集中指定区间内的成员，通过索引，分数从高到低");
        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        public static CommandType ZREVRANGEBYSCORE => new CommandType("ZREVRANGEBYSCORE", "ZREVRANGEBYSCORE key max min [WITHSCORES]", "返回有序集中指定分数区间内的成员，分数从高到低排序");
        /// <summary>
        /// 返回有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序
        /// </summary>
        public static CommandType ZREVRANK => new CommandType("ZREVRANK", "ZREVRANK key member", "返回有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序");
        /// <summary>
        /// 返回有序集中，成员的分数值
        /// </summary>
        public static CommandType ZSCORE => new CommandType("ZSCORE", "ZSCORE key member", "返回有序集中，成员的分数值");
        /// <summary>
        /// 计算给定的一个或多个有序集的并集，并存储在新的 key 中
        /// </summary>
        public static CommandType ZUNIONSTORE => new CommandType("ZUNIONSTORE", "ZUNIONSTORE destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX]", "计算给定的一个或多个有序集的并集，并存储在新的 key 中");
        /// <summary>
        /// 迭代有序集合中的元素（包括元素成员和元素分值）
        /// </summary>
        public static CommandType ZSCAN => new CommandType("ZSCAN", "ZSCAN key cursor [MATCH pattern] [COUNT count]", "迭代有序集合中的元素（包括元素成员和元素分值）");
        #endregion

        #region Redis HyperLogLog 命令
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中。
        /// </summary>
        public static CommandType PFADD => new CommandType("PFADD", "PFADD key element [element ...]", "添加指定元素到 HyperLogLog 中");
        /// <summary>
        /// 返回给定 HyperLogLog 的基数估算值。
        /// </summary>
        public static CommandType PFCOUNT => new CommandType("PFCOUNT", "PFCOUNT key [key ...]", "返回给定 HyperLogLog 的基数估算值");
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog
        /// </summary>
        public static CommandType PFMERGE => new CommandType("PFMERGE", "PFMERGE destkey sourcekey [sourcekey ...]", "将多个 HyperLogLog 合并为一个 HyperLogLog");
        #endregion

        #region Redis 发布订阅命令
        /// <summary>
        /// 订阅一个或多个符合给定模式的频道。
        /// </summary>
        public static CommandType PSUBSCRIBE => new CommandType("PSUBSCRIBE", "	PSUBSCRIBE pattern [pattern ...]", "订阅一个或多个符合给定模式的频道");
        /// <summary>
        /// 查看订阅与发布系统状态。
        /// </summary>
        public static CommandType PUBSUB => new CommandType("PUBSUB", "PUBSUB subcommand [argument [argument ...]]", "查看订阅与发布系统状态");
        /// <summary>
        /// 将信息发送到指定的频道。
        /// </summary>
        public static CommandType PUBLISH => new CommandType("PUBLISH", "PUBLISH channel message", "将信息发送到指定的频道");
        /// <summary>
        /// 退订所有给定模式的频道。
        /// </summary>
        public static CommandType PUNSUBSCRIBE => new CommandType("PUNSUBSCRIBE", "PUNSUBSCRIBE [pattern [pattern ...]]", "退订所有给定模式的频道");
        /// <summary>
        /// 订阅给定的一个或多个频道的信息。
        /// </summary>
        public static CommandType SUBSCRIBE => new CommandType("SUBSCRIBE", "SUBSCRIBE channel [channel ...]", "订阅给定的一个或多个频道的信息");
        /// <summary>
        /// 指退订给定的频道。
        /// </summary>
        public static CommandType UNSUBSCRIBE => new CommandType("UNSUBSCRIBE", "UNSUBSCRIBE [channel [channel ...]]", "指退订给定的频道");
        #endregion

        #region Redis 事务命令
        /// <summary>
        /// 取消事务，放弃执行事务块内的所有命令。
        /// </summary>
        public static CommandType DISCARD => new CommandType("DISCARD", "", "取消事务，放弃执行事务块内的所有命令");
        /// <summary>
        /// 执行所有事务块内的命令。
        /// </summary>
        public static CommandType EXEC => new CommandType("EXEC", "", "执行所有事务块内的命令");
        /// <summary>
        /// 标记一个事务块的开始。
        /// </summary>
        public static CommandType MULTI => new CommandType("MULTI", "", "标记一个事务块的开始");
        /// <summary>
        /// 取消 WATCH 命令对所有 key 的监视。
        /// </summary>
        public static CommandType UNWATCH => new CommandType("UNWATCH", "", "取消 WATCH 命令对所有 key 的监视");
        /// <summary>
        /// 监视一个(或多个) key ，如果在事务执行之前这个(或这些) key 被其他命令所改动，那么事务将被打断。
        /// </summary>
        public static CommandType WATCH => new CommandType("WATCH", "WATCH key [key ...]", "监视一个(或多个) key ，如果在事务执行之前这个(或这些) key 被其他命令所改动，那么事务将被打断");
        #endregion

        #region GEO 地理空间位置
        /// <summary>
        /// 用于存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中
        /// </summary>
        public static CommandType GEOADD => new CommandType("GEOADD", "GEOADD key longitude latitude member [longitude latitude member ...]", "用于存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中");
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。
        /// </summary>
        public static CommandType GEOPOS => new CommandType("GEOPOS", "GEOPOS key member [member ...]", "用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil");
        /// <summary>
        /// 用于返回两个给定位置之间的距离。
        /// </summary>
        public static CommandType GEODIST => new CommandType("GEODIST", "GEODIST key member1 member2 [m|km|ft|mi]", "用于返回两个给定位置之间的距离");
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        public static CommandType GEORADIUS => new CommandType("GEORADIUS", "GEORADIUS key longitude latitude radius m|km|ft|mi [WITHCOORD] [WITHDIST] [WITHHASH] [COUNT count] [ASC|DESC] [STORE key] [STOREDIST key]", "以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素");
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        public static CommandType GEORADIUSBYMEMBER => new CommandType("GEORADIUSBYMEMBER", "GEORADIUSBYMEMBER key member radius m|km|ft|mi [WITHCOORD] [WITHDIST] [WITHHASH] [COUNT count] [ASC|DESC] [STORE key] [STOREDIST key]", "以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素");
        /// <summary>
        /// 搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        public static CommandType GEOSEARCH => new CommandType("GEOSEARCH", "GEOSEARCH key [FROMMEMBER member] [FROMLONLAT longitude latitude] [BYRADIUS radius m|km|ft|mi] [BYBOX width height m|km|ft|mi] [ASC|DESC] [COUNT count [ANY]] [WITHCOORD] [WITHDIST] [WITHHASH]", "搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素");
        /// <summary>
        /// 搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素存储到列表中。
        /// </summary>
        public static CommandType GEOSEARCHSTORE => new CommandType("GEOSEARCHSTORE", "GEOSEARCHSTORE destination source [FROMMEMBER member] [FROMLONLAT longitude latitude] [BYRADIUS radius m|km|ft|mi] [BYBOX width height m|km|ft|mi] [ASC|DESC] [COUNT count [ANY]] [STOREDIST]", "搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素存储到列表中");
        /// <summary>
        /// 保存地理位置的坐标。
        /// </summary>
        public static CommandType GEOHASH => new CommandType("GEOHASH", "GEOHASH key member [member ...]", "保存地理位置的坐标");
        #endregion

        #region Redis 连接命令
        /// <summary>
        /// 密码认证
        /// </summary>
        public static CommandType AUTH => new CommandType("AUTH", "AUTH password", "密码认证");
        /// <summary>
        /// 输出指定字符串
        /// </summary>
        public static CommandType ECHO => new CommandType("ECHO", "ECHO message", "输出指定字符串");
        /// <summary>
        /// 测试服务器连接
        /// </summary>
        public static CommandType PING => new CommandType("PING", "", "测试服务器连接");
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        public static CommandType QUIT => new CommandType("QUIT", "", "关闭客户端连接");
        /// <summary>
        /// 切换数据库
        /// </summary>
        public static CommandType SELECT => new CommandType("SELECT", "SELECT index", "切换数据库");
        /// <summary>
        /// 交换两个数据库
        /// </summary>
        public static CommandType SWAPDB => new CommandType("SWAPDB", "SWAPDB oldindex newindex", "交换两个数据库");
        #endregion

        #region Redis 消息队列

        #region 消息队列相关命令
        /// <summary>
        /// 使用 XADD 向队列添加消息，如果指定的队列不存在，则创建一个队列
        /// </summary>
        public static CommandType XADD => new CommandType("XADD", "XADD key ID field value [field value ...]", "使用 XADD 向队列添加消息，如果指定的队列不存在，则创建一个队列");
        /// <summary>
        /// 对流进行修剪，限制长度
        /// </summary>
        public static CommandType XTRIM => new CommandType("XTRIM", "XTRIM key MAXLEN [~] count", "对流进行修剪，限制长度");
        /// <summary>
        /// 删除消息
        /// </summary>
        public static CommandType XDEL => new CommandType("XDEL", "XDEL key ID [ID ...]", "删除消息");
        /// <summary>
        /// 获取流包含的元素数量，即消息长度
        /// </summary>
        public static CommandType XLEN => new CommandType("XLEN", "XLEN key", "获取流包含的元素数量，即消息长度");
        /// <summary>
        /// 获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        public static CommandType XRANGE => new CommandType("XRANGE", "XRANGE key start end [COUNT count]", "获取消息列表，会自动过滤已经删除的消息");
        /// <summary>
        /// 反向获取消息列表，ID 从大到小 会自动过滤已经删除的消息
        /// </summary>
        public static CommandType XREVRANGE => new CommandType("XREVRANGE", "XREVRANGE key end start [COUNT count]", "反向获取消息列表，ID 从大到小 会自动过滤已经删除的消息");
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        public static CommandType XREAD => new CommandType("XREAD", "XREAD [COUNT count] [BLOCK milliseconds] STREAMS key [key ...] id [id ...]", "以阻塞或非阻塞方式获取消息列表");
        #endregion

        #region 消费者组相关命令
        /// <summary>
        /// 创建消费者组
        /// </summary>
        public static CommandType XGROUPCREATE => new CommandType("XGROUP CREATE", "XGROUP [CREATE key groupname id-or-$] [SETID key groupname id-or-$] [DESTROY key groupname] [DELCONSUMER key groupname consumername]", "创建消费者组", new string[] { "XGROUP", "CREATE" });
        /// <summary>
        /// 读取消费者组中的消息
        /// </summary>
        public static CommandType XREADGROUPGROUP => new CommandType("XREADGROUP GROUP", "XREADGROUP GROUP group consumer [COUNT count] [BLOCK milliseconds] [NOACK] STREAMS key [key ...] ID [ID ...]", "读取消费者组中的消息", new string[] { "XREADGROUP", "GROUP" });
        /// <summary>
        /// 将消息标记为"已处理"
        /// </summary>
        public static CommandType XACK => new CommandType("XACK", "XACK key group id [id ...]", "将消息标记为\"已处理\"");
        /// <summary>
        /// 为消费者组设置新的最后递送消息ID
        /// </summary>
        public static CommandType XGROUPSETID => new CommandType("XGROUP SETID", "XGROUP SETID key groupname id-or-$", "为消费者组设置新的最后递送消息ID", new string[] { "XGROUP", "SETID" });
        /// <summary>
        /// 删除消费者
        /// </summary>
        public static CommandType XGROUPDELCONSUMER => new CommandType("XGROUP DELCONSUMER", "XGROUP DELCONSUMER key groupname consumername", "删除消费者", new string[] { "XGROUP", "DELCONSUMER" });
        /// <summary>
        /// 删除消费者组
        /// </summary>
        public static CommandType XGROUPDESTROY => new CommandType("XGROUP DESTROY", "XGROUP DESTROY key groupname", "删除消费者组", new string[] { "XGROUP", "DESTROY" });
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        public static CommandType XPENDING => new CommandType("XPENDING", "XPENDING key group [[IDLE min-idle-time] start end count [consumer]]", "显示待处理消息的相关信息");
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        public static CommandType XCLAIM => new CommandType("XCLAIM", "XCLAIM key group consumer min-idle-time id [id ...] [IDLE ms] [TIME unix-time-milliseconds] [RETRYCOUNT count] [FORCE] [JUSTID]", "转移消息的归属权");
        /// <summary>
        /// 查看流和消费者组的相关信息
        /// </summary>
        public static CommandType XINFOCONSUMERS => new CommandType("XINFO CONSUMERS", "XINFO CONSUMERS key groupname", "查看流和消费者组的相关信息", new string[] { "XINFO", "CONSUMERS" });
        /// <summary>
        /// 打印消费者组的信息
        /// </summary>
        public static CommandType XINFOGROUPS => new CommandType("XINFO GROUPS", "XINFO GROUPS key", "打印消费者组的信息", new string[] { "XINFO", "GROUPS" });
        /// <summary>
        /// 打印流信息
        /// </summary>
        public static CommandType XINFOSTREAM => new CommandType("XINFO STREAM", "XINFO STREAM key [FULL [COUNT count]]", "打印流信息", new string[] { "XINFO", "STREAM" });
        #endregion

        #endregion

        #region Redis 服务器
        /// <summary>
        /// 异步执行一个 AOF（AppendOnly File） 文件重写操作。
        /// </summary>
        public static CommandType BGREWRITEAOF => new CommandType("BGREWRITEAOF", "", "异步执行一个 AOF（AppendOnly File） 文件重写操作");
        /// <summary>
        /// 将当前服务器转变为指定服务器的从属服务器(slave server)。
        /// </summary>
        public static CommandType SLAVEOF => new CommandType("SLAVEOF", "SLAVEOF host port", "将当前服务器转变为指定服务器的从属服务器(slave server)");
        /// <summary>
        /// 执行客户端命令
        /// </summary>
        public static CommandType CLIENT => new CommandType("CLIENT", "", "执行客户端命令");
        
        /// <summary>
        /// 如果需要恢复数据，只需将备份文件 (dump.rdb) 移动到 redis 安装目录并启动服务即可。获取 redis 目录可以使用 CONFIG 命令
        /// </summary>
        public static CommandType CONFIG => new CommandType("CONFIG", "CONFIG GET dir", "如果需要恢复数据，只需将备份文件 (dump.rdb) 移动到 redis 安装目录并启动服务即可。获取 redis 目录可以使用 CONFIG 命令");
        /// <summary>
        /// 异步保存数据到硬盘，并关闭服务器
        /// </summary>
        public static CommandType SHUTDOWN => new CommandType("SHUTDOWN", "SHUTDOWN [NOSAVE] [SAVE]", "异步保存数据到硬盘，并关闭服务器");
        /// <summary>
        /// 查看主从实例所属的角色，角色有master, slave, sentinel。
        /// </summary>
        public static CommandType ROLE => new CommandType("ROLE", "", "查看主从实例所属的角色，角色有master, slave, sentinel");
        /// <summary>
        /// 返回当前数据库的 key 的数量。
        /// </summary>
        public static CommandType DBSIZE => new CommandType("DBSIZE", "", "返回当前数据库的 key 的数量");
        /// <summary>
        /// 删除所有数据库的所有key。
        /// </summary>
        public static CommandType FLUSHALL => new CommandType("FLUSHALL", "", "删除所有数据库的所有key");
        /// <summary>
        /// 删除当前数据库的所有key。
        /// </summary>
        public static CommandType FLUSHDB => new CommandType("FLUSHDB", "", "删除当前数据库的所有key");
        #endregion

        #region 服务器
        /// <summary>
        /// 查看服务器信息
        /// </summary>
        public static CommandType INFO => new CommandType("INFO", "", "查看服务器信息");
        /// <summary>
        /// 排序
        /// </summary>
        public static CommandType SORT => new CommandType("SORT", "SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]");
        #endregion

        #region 持久化
        /// <summary>
        /// 查看最近一次 RDB 持久化时间戳
        /// </summary>
        public static CommandType LASTSAVE => new CommandType("LASTSAVE", "", "查看最近一次 RDB 持久化时间戳");
        /// <summary>
        /// 同步执行 RDB 持久化。
        /// </summary>
        public static CommandType SAVE => new CommandType("SAVE", "", "同步执行 RDB 持久化");
        /// <summary>
        /// 异步执行 RDB 持久化。
        /// </summary>
        public static CommandType BGSAVE => new CommandType("BGSAVE", "", "异步执行 RDB 持久化");
        #endregion

        #region 脚本
        /// <summary>
        /// 执行 Lua 脚本
        /// </summary>
        public static CommandType EVAL => new CommandType("EVAL", "EVAL script numkeys key [key ...] arg [arg ...]", "执行 Lua 脚本");
        /// <summary>
        /// 执行已缓存的 Lua 脚本
        /// </summary>
        public static CommandType EVALSHA => new CommandType("EVALSHA", "EVALSHA sha1 numkeys key [key ...] arg [arg ...]", "执行已缓存的 Lua 脚本");
        /// <summary>
        /// 检查脚本是否在缓存
        /// </summary>
        public static CommandType SCRIPTEXISTS => new CommandType("SCRIPT EXISTS", "SCRIPT EXISTS sha1 [sha1 ...]", "检查脚本是否在缓存", new string[] { "SCRIPT", "EXISTS" });
        /// <summary>
        /// 清空脚本缓存
        /// </summary>
        public static CommandType SCRIPTFLUSH => new CommandType("SCRIPT FLUSH", "SCRIPT FLUSH", "清空脚本缓存", new string[] { "SCRIPT", "FLUSH" });
        /// <summary>
        /// 终止正在执行的 Lua 脚本
        /// </summary>
        public static CommandType SCRIPTKILL => new CommandType("SCRIPT KILL", "SCRIPT KILL", "终止正在执行的 Lua 脚本", new string[] { "SCRIPT", "KILL" });
        /// <summary>
        /// 加载 Lua 脚本到缓存
        /// </summary>
        public static CommandType SCRIPTLOAD => new CommandType("SCRIPT LOAD", "SCRIPT LOAD script", "加载 Lua 脚本到缓存", new string[] { "SCRIPT", "LOAD" });
        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other">请求类型</param>
        /// <returns>是否相等</returns>
        public bool Equals(CommandType other) => this.Name.EqualsIgnoreCase(other.Name);
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="left">请求类型</param>
        /// <param name="right">请求类型</param>
        /// <returns>summary</returns>
        public static bool operator ==(CommandType left, CommandType right) => left.Equals(right);
        /// <summary>
        /// 是否不相等
        /// </summary>
        /// <param name="left">请求类型</param>
        /// <param name="right">请求类型</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(CommandType left, CommandType right) => !left.Equals(right);
        /// <summary>
        /// 显示转换
        /// </summary>
        /// <param name="method">请求类型</param>
        public static explicit operator string(CommandType method) => method.ToString();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="method">请求类型</param>
        public static implicit operator CommandType(string method) => new CommandType(method, "");
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">请求类型</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is CommandType cmdType)
                return this.Equals(cmdType);
            return false;
        }
        /// <summary>
        /// HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => this.Name.GetHashCode();
        /// <summary>
        /// 转换命令
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.Name;
        /// <summary>
        /// 比较器
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is CommandType ct && this.Name == ct.Name && this.Format == ct.Format && this.Commands == ct.Commands) return 0;
            return -1;
        }
        /// <summary>
        /// 比较器
        /// </summary>
        /// <param name="other">对象</param>
        /// <returns></returns>
        public int CompareTo(CommandType other)
        {
            if (other == null) return 1;
            if (this.Name == other.Name && this.Format == other.Format && this.Commands == other.Commands) return 0;
            return -1;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="formatProvider">格式驱动</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format.IsNullOrEmpty()) return this.ToString();
            var val = this.Name.ToString(formatProvider);
            switch (format.ToUpper())
            {
                case "UPPER":
                    return val.ToUpper();
                case "LOWER":
                    return val.ToLower();
                default:
                    return val;
            }
        }
        #endregion
    }
}