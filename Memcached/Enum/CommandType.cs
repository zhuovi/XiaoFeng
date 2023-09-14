using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-06 16:43:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 命令类型
    /// </summary>
    public class CommandType : IEquatable<CommandType>, IComparable, IComparable<CommandType>, IFormattable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CommandType() { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="format">格式</param>
        /// <param name="index">索引</param>
        public CommandType(string name, string format,int index) : this(name, format,index, null) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="format">格式</param>
        /// <param name="index">索引</param>
        /// <param name="commands">命令组</param>
        public CommandType(string name, string format, int index, string[] commands)
        {
            this.Index = index;
            this.SetCommand(index);
            this.Name = name;
            this.Format = format;
            this.Commands = commands;
        }
        /// <summary>
        /// 设置命令
        /// </summary>
        /// <param name="command">命令</param>
        public CommandType(int command) => this.SetCommand(command);
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
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 命令格式
        /// </summary>
        public CommandFlags Flags { get; set; }
        #endregion

        #region 方法

        #region 设置命令
        /// <summary>
        /// 设置命令
        /// </summary>
        /// <param name="command">命令</param>
        private void SetCommand(int command)
        {
            this.Index = command;
            var cmd = "";
            if (command < 20)//StoreCommand
            {
                if (command == (int)StoreCommand.Cas)
                {
                    this.Name = StoreCommand.Cas.ToString();
                    this.Format = "cas key flags exptime bytes unique_cas_token [noreply]\r\nvalue\r\n";
                    cmd = "cas";
                }
                else
                {
                    this.Name = ((StoreCommand)command).ToString();
                    this.Format = $"{this.Name} key flags exptime bytes [noreply]\r\nvalue\r\n";
                    cmd = this.Name;
                }
                this.Flags = CommandFlags.Store;
            }
            else if (command < 40)//GetCommand
            {
                var get = (GetCommand)command;
                this.Name = get.ToString();
                cmd = this.Name;
                if ((GetCommand.Get | GetCommand.Gets).HasFlag(get))
                {
                    this.Format = $"{this.Name} <key>*";
                }
                else if((GetCommand.Gat| GetCommand.Gats).HasFlag(get))
                {
                    this.Format = $"{this.Name} <exptime> <key>*";
                }
                else if (get == GetCommand.Delete)
                    this.Format = "delete <key> [noreply]";
                else if ((GetCommand.Increment | GetCommand.Decrement).HasFlag(get))
                    this.Format = $"{cmd} <key> <value> [noreply]";
                else if (get == GetCommand.Touch)
                    this.Format = "touch <key> <exptime> [noreply]";
                this.Flags = CommandFlags.Get;
            }
            else if (command < 60)//StatsCommand
            {
                var stats = (StatsCommand)command;
                this.Name = stats.ToString();
                cmd = stats.GetEnumName();
                if ((StatsCommand.Stats | StatsCommand.Slabs | StatsCommand.Sizes | StatsCommand.Items).HasFlag(stats))
                {
                    this.Format = cmd;
                }
                this.Flags = CommandFlags.Stats;
            }
            this.Commands = new string[] { cmd };
        }
        #endregion

        #region 设置命令格式
        /// <summary>
        /// 设置命令格式
        /// </summary>
        public void SetCommandFlags()
        {
            typeof(CommandFlags).GetFields().Each(f =>
            {
                if (f.IsDefined(typeof(RangeAttribute), false))
                {
                    var range = f.GetCustomAttributeX<RangeAttribute>();

                    if (range != null)
                    {
                        if (range.Start <= this.Index && range.End >= this.Index)
                            this.Flags = f.Name.ToString().ToEnum<CommandFlags>();
                    }
                }
            });
        }
        #endregion

        #region Store
        /// <summary>
        /// 认证
        /// </summary>
        public static CommandType AUTH => new CommandType("auth", "set <key> <flags> <exptime> <bytes> [noreply]\r\nusername password\r\n", (int)StoreCommand.Set, new string[] { "set" });
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        public static CommandType SET => new CommandType("set", "set <key> <flags> <exptime> <bytes> [noreply]", (int)StoreCommand.Set, new string[] { "set" });
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        public static CommandType ADD => new CommandType("add", "add <key> <flags> <exptime> <bytes> [noreply]", (int)StoreCommand.Add, new string[] { "add" });
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        public static CommandType PREPEND => new CommandType("prepend", "prepend <key> <flags> <exptime> <bytes> [noreply]", (int)StoreCommand.Prepend, new string[] { "prepend" });
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        public static CommandType APPEND => new CommandType("append", "append <key> <flags> <exptime> <bytes> [noreply]", (int)StoreCommand.Append, new string[] { "append" });
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        public static CommandType REPLACE => new CommandType("replace", "replace <key> <flags> <exptime> <bytes> [noreply]", (int)StoreCommand.Replace, new string[] { "replace" });
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值
        /// </summary>
        public static CommandType CAS => new CommandType("cas", "cas <key> <flags> <exptime> <bytes> unique_cas_token [noreply]", (int)StoreCommand.Cas, new string[] { "cas" });
        #endregion

        #region Get
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key
        /// </summary>
        public static CommandType GET => new CommandType("get", "get <key>*", (int)GetCommand.Get, new string[] { "get" });
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        public static CommandType GETS => new CommandType("gets", "gets <key>*", (int)GetCommand.Gets, new string[] { "gets" });
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        public static CommandType GAT => new CommandType("gat", "gat <exptime> <key>*", (int)GetCommand.Gat, new string[] { "gat" });
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        public static CommandType GATS => new CommandType("gats", "gats <exptime> <key>*", (int)GetCommand.Gats, new string[] { "gats" });
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        public static CommandType DELETE => new CommandType("delete", "delete <key> [noreply]", (int)GetCommand.Delete, new string[] { "delete" });
        /// <summary>
        /// 递增
        /// </summary>
        public static CommandType INCREMENT => new CommandType("incr", "incr <key> <increment_value> [noreply]", (int)GetCommand.Increment, new string[] { "incr" });
        /// <summary>
        /// 递减
        /// </summary>
        public static CommandType DECREMENT => new CommandType("decr", "decr <key> <decrement_value> [noreply]", (int)GetCommand.Decrement, new string[] { "decr" });
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        public static CommandType TOUCH => new CommandType("touch", "touch <key> <exptime> [noreply]", (int)GetCommand.Touch, new string[] { "touch" });
        #endregion

        #region Stats
        /// <summary>
        /// 返回统计信息
        /// </summary>
        public static CommandType STATS => new CommandType("stats", "stats", (int)StatsCommand.Stats, new string[] { "stats" });
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        public static CommandType STATSITEMS => new CommandType("stats items", "stats items", (int)StatsCommand.Items, new string[] { "stats","items" });
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        public static CommandType STATSSLABS => new CommandType("stats slabs", "stats items", (int)StatsCommand.Slabs, new string[] { "stats","slabs" });
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        public static CommandType STATSSIZES => new CommandType("stats sizes", "stats sizes", (int)StatsCommand.Sizes, new string[] { "stats", "sizes" });
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        public static CommandType FLUSHALL => new CommandType("flush_all", "flush_all [time] [noreply]", (int)StatsCommand.FlushAll, new string[] { "flush_all" });
        #endregion

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
        public static implicit operator CommandType(string method) => new CommandType(method, "", 0);
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