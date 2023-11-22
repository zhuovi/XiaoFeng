using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-07 11:14:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 服务器
    /// </summary>
    public partial class RedisClient : Disposable, IRedisClient
    {
        #region 服务器
        /// <summary>
        /// 异步执行一个 AOF（AppendOnly File） 文件重写操作。
        /// </summary>
        public Boolean BackgroundRewriteAOF() => this.Execute(CommandType.BGREWRITEAOF, null, result => result.OK);
        /// <summary>
        /// 后台异步保存当前数据库的数据到磁盘。
        /// </summary>
        /// <param name="dbNum">库索引</param>
        public Boolean BackgroundSave(int? dbNum = null) => this.Execute(CommandType.BGSAVE, dbNum, result => result.OK);
        /// <summary>
        /// 客户端命令
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        public RedisReader Client(string key, params object[] values)
        {
            if (key.IsNullOrEmpty()) return default;
            return this.Execute(CommandType.CLIENT, null, result => result, new object[] { key }.Concat(values).ToArray());
        }
        /// <summary>
        /// 客户端命令 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        public async Task<RedisReader> ClientAsync(string key, params object[] values)
        {
            if (key.IsNullOrEmpty()) return default;
            return await this.ExecuteAsync(CommandType.CLIENT, null, async result => await Task.FromResult(result), new object[] { key }.Concat(values).ToArray());
        }
        /// <summary>
        /// 杀死客户端
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public Boolean ClientKill(string host, int port) => this.Client("KILL", host.IfEmpty("127.0.0.1"), Math.Abs(port)).OK;
        /// <summary>
        /// 杀死客户端 异步
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public async Task<Boolean> ClientKillAsync(string host, int port) => (await this.ClientAsync("KILL", host.IfEmpty("127.0.0.1"), Math.Abs(port))).OK;
        /// <summary>
        /// 在指定时间内终止运行来自客户端的命令
        /// </summary>
        /// <param name="timeout">时间 单位为毫秒</param>
        /// <returns></returns>
        public Boolean ClientPause(long timeout) => this.Client("PAUSE", timeout).OK;
        /// <summary>
        /// 在指定时间内终止运行来自客户端的命令 异步
        /// </summary>
        /// <param name="timeout">时间 单位为毫秒</param>
        /// <returns></returns>
        public async Task<Boolean> ClientPauseAsync(long timeout) => (await this.ClientAsync("PAUSE", timeout)).OK;
        /// <summary>
        /// 客户端列表
        /// </summary>
        /// <returns></returns>
        public List<ClientInfo> ClientList()
        {
            var reader = this.Client("LIST");
            return reader.OK ? (List<ClientInfo>)reader.Value.Value : null;
        }
        /// <summary>
        /// 客户端列表 异步
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClientInfo>> ClientListAsync()
        {
            var reader = await this.ClientAsync("LIST");
            return reader.OK ? (List<ClientInfo>)reader.Value.Value : null;
        }
        /// <summary>
        /// 设置客户端名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public Boolean ClientSetName(string name)
        {
            if (name.IsNullOrEmpty()) return false;
            return this.Client("SETNAME", name).OK;
        }
        /// <summary>
        /// 设置客户端名称 异步
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public async Task<Boolean> ClientSetNameAsync(string name)
        {
            if (name.IsNullOrEmpty()) return false;
            return (await this.ClientAsync("SETNAME", name)).OK;
        }
        /// <summary>
        /// 获取客户端名称
        /// </summary>
        /// <returns></returns>
        public string ClientGetName() => this.Client("GETNAME")?.Value.ToString();
        /// <summary>
        /// 获取客户端名称 异步
        /// </summary>
        /// <returns></returns>
        public async Task<string> ClientGetNameAsync() => (await this.ClientAsync("GETNAME")).Value?.ToString();
        /// <summary>
        /// 将当前服务器转变为指定服务器的从属服务器(slave server)
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public Boolean SlavEOF(string host, int port) => this.Execute(CommandType.SLAVEOF, null, result => result.OK, host, port);
        /// <summary>
        /// 将当前服务器转变为指定服务器的从属服务器(slave server) 异步
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public async Task<Boolean> SlavEOFAsync(string host, int port) => await this.ExecuteAsync(CommandType.SLAVEOF, null, async result => await Task.FromResult(result.OK), host, port);
        /// <summary>
        /// 异步保存数据到硬盘，并关闭服务器
        /// </summary>
        /// <param name="isSave">是否保存数据</param>
        /// <returns></returns>
        public Boolean ShutDown(Boolean isSave = true) => this.Execute(CommandType.SHUTDOWN, null, result => result.OK, isSave ? "SAVE" : "NOSAVE");
        /// <summary>
        /// 异步保存数据到硬盘，并关闭服务器 异步
        /// </summary>
        /// <param name="isSave">是否保存数据</param>
        /// <returns></returns>
        public async Task<Boolean> ShutDownAsync(Boolean isSave = true) => await this.ExecuteAsync(CommandType.SHUTDOWN, null, async result => await Task.FromResult(result.OK), isSave ? "SAVE" : "NOSAVE");
        /// <summary>
        /// 一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。
        /// </summary>
        /// <returns></returns>
        public Boolean Save() => this.Execute(CommandType.SAVE, null, result => result.OK);
        /// <summary>
        /// 一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> SaveAsync() => await this.ExecuteAsync(CommandType.SAVE, null, async result => await Task.FromResult(result.OK));
        /// <summary>
        /// 查看主从实例所属的角色，角色有master, slave, sentinel。
        /// </summary>
        /// <returns></returns>
        public RedisValue Role() => this.Execute(CommandType.ROLE, null, result => result.OK ? result.Value : null);
        /// <summary>
        /// 查看主从实例所属的角色，角色有master, slave, sentinel。 异步
        /// </summary>
        /// <returns></returns>
        public async Task<RedisValue> RolesAsync() => await this.ExecuteAsync(CommandType.ROLE, null, async result => await Task.FromResult(result.OK ? result.Value : null));
        /// <summary>
        /// 返回当前数据库的 key 的数量
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public int GetDbKeySize(int? dbNum = null) => this.Execute(CommandType.DBSIZE, dbNum, result => result.OK ? result.Value.ToInt() : -1);
        /// <summary>
        /// 返回当前数据库的 key 的数量 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<int> GetDbKeySizeAsync(int? dbNum = null) => await this.ExecuteAsync(CommandType.DBSIZE, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToInt() : -1));
        /// <summary>
        /// 删除当前数据库的所有key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean DelDbKeys(int? dbNum = null) => this.Execute(CommandType.FLUSHDB, dbNum, result => result.OK);
        /// <summary>
        /// 删除当前数据库的所有key 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> DelDbKeysAsync(int? dbNum = null) => await this.ExecuteAsync(CommandType.FLUSHDB, dbNum, async result => await Task.FromResult(result.OK));
        /// <summary>
        /// 删除所有数据库的所有key
        /// </summary>
        /// <returns></returns>
        public Boolean DelAllKeys() => this.Execute(CommandType.FLUSHALL, null, result => result.OK);
        /// <summary>
        /// 删除所有数据库的所有key 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> DelAllKeysAsync() => await this.ExecuteAsync(CommandType.FLUSHALL, null, async result => await Task.FromResult(result.OK));
        /// <summary>
        /// 服务器配置命令
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        public RedisReader Config(string key, params object[] values)
        {
            if (key.IsNullOrEmpty()) return default;
            return this.Execute(CommandType.CONFIG, null, result => result, new object[] { key }.Concat(values).ToArray());
        }
        /// <summary>
        /// 服务器配置命令 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        public async Task<RedisReader> ConfigAsync(string key, params object[] values)
        {
            if (key.IsNullOrEmpty()) return default;
            return await this.ExecuteAsync(CommandType.CONFIG, null, async result => await Task.FromResult(result), new object[] { key }.Concat(values).ToArray());
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns></returns>
        public Dictionary<string, string> GetConfig(string key = "*")
        {
            var reader = this.Config("GET", key);
            if (reader == null) return new Dictionary<string, string>();
            return reader.OK ? reader.Value.ToDictionary<string, string>() : new Dictionary<string, string>();
        }
        /// <summary>
        /// 获取配置 异步
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetConfigAsync(string key = "*")
        {
            var reader = await this.ConfigAsync("GET", key);
            return reader.OK ? reader.Value.ToDictionary<string, string>() : new Dictionary<string, string>();
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public Boolean SetConfig(string key, string value)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Config("SET", key, value).OK;
        }
        /// <summary>
        /// 设置配置 异步
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<Boolean> SetConfigAsync(string key, string value)
        {
            if (key.IsNullOrEmpty()) return false;
            return (await this.ConfigAsync("SET", key, value)).OK;
        }
        /// <summary>
        /// 对启动 Redis 服务器时所指定的 redis.conf 配置文件进行改写
        /// </summary>
        /// <returns></returns>
        public Boolean ConfigRewrite() => this.Config("REWRITE").OK;
        /// <summary>
        /// 对启动 Redis 服务器时所指定的 redis.conf 配置文件进行改写 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> ConfigRewriteAsync() => (await this.ConfigAsync("REWRITE")).OK;
        #endregion
    }
}