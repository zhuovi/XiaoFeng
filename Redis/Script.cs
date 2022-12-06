using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-11-30 11:41:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis 脚本
    /// </summary>
    public partial class RedisClient : Disposable, IRedisClient
    {
        #region 脚本

        #region 执行 Lua 脚本
        /// <summary>
        /// 执行 Lua 脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        public RedisValue EvalScript(string script, string[] parameters = null, string[] keys = null, int? dbNum = null)
        {
            if (script.IsNullOrEmpty()) return null;
            var Params = new List<object>
            {
                script,
                keys?.Length
            };
            if (keys != null)
                Params.AddRange(keys);
            if (parameters != null) Params.AddRange(parameters);
            return this.Execute(CommandType.EVAL, dbNum, result => result.OK ? result.Value : null, Params.ToArray());
        }
        /// <summary>
        /// 异步执行 Lua 脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        public async Task<RedisValue> EvalScriptAsync(string script, string[] parameters = null, string[] keys = null, int? dbNum = null)
        {
            if (script.IsNullOrEmpty()) return null;
            var Params = new List<object>
            {
                keys?.Length
            };
            if (keys != null)
                Params.AddRange(keys);
            if (parameters != null) Params.AddRange(parameters);
            return await this.ExecuteAsync(CommandType.EVAL, dbNum, async result => await Task.FromResult(result.OK ? result.Value : null), Params.ToArray());
        }

        #endregion

        #region 执行 Lua 脚本 根据SHA1码
        /// <summary>
        /// 执行 Lua 脚本
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        public RedisValue EvalSHA(string shacode, string[] parameters = null, string[] keys = null, int? dbNum = null)
        {
            if (shacode.IsNullOrEmpty()) return null;
            var Params = new List<object>
            {
                shacode,
                keys?.Length
            };
            if (keys != null)
                Params.AddRange(keys);
            if (parameters != null) Params.AddRange(parameters);
            return this.Execute(CommandType.EVALSHA, dbNum, result => result.OK ? result.Value : null, Params.ToArray());
        }
        /// <summary>
        /// 异步执行 Lua 脚本 根据SHA1码
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        public async Task<RedisValue> EvalSHAAsync(string shacode, string[] parameters = null, string[] keys = null, int? dbNum = null)
        {
            if (shacode.IsNullOrEmpty()) return null;
            var Params = new List<object>
            {
                shacode,
                keys?.Length
            };
            if (keys != null)
                Params.AddRange(keys);
            if (parameters != null) Params.AddRange(parameters);
            return await this.ExecuteAsync(CommandType.EVALSHA, dbNum, async result => await Task.FromResult(result.OK ? result.Value : null), Params.ToArray());
        }
        #endregion

        #region 查看指定的脚本是否已经被保存在缓存当中
        /// <summary>
        /// 查看指定的脚本是否已经被保存在缓存当中
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <returns></returns>
        public RedisValue ScriptExists(params string[] shacode)
        {
            if (shacode.Length == 0) return false;
            return this.Execute(CommandType.SCRIPTEXISTS, null, result => result.OK ? result.Value : null, shacode);
        }
        /// <summary>
        /// 查看指定的脚本是否已经被保存在缓存当中
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <returns></returns>
        public async Task<RedisValue> ScriptExistsAsync(params string[] shacode)
        {
            if (shacode.Length == 0) return false;
            return await this.ExecuteAsync(CommandType.SCRIPTEXISTS, null, async result => await Task.FromResult(result.OK ? result.Value : null), shacode);
        }
        #endregion

        #region 从脚本缓存中移除所有脚本
        /// <summary>
        /// 从脚本缓存中移除所有脚本
        /// </summary>
        /// <returns></returns>
        public Boolean ScriptFlush() => this.Execute(CommandType.SCRIPTFLUSH, null, result => result.OK);
        /// <summary>
        /// 从脚本缓存中移除所有脚本
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> ScriptFlushAsync() => await this.ExecuteAsync(CommandType.SCRIPTFLUSH, null, async result => await Task.FromResult(result.OK));
        #endregion

        #region 杀死当前正在运行的 Lua 脚本
        /// <summary>
        /// 杀死当前正在运行的 Lua 脚本
        /// </summary>
        /// <returns></returns>
        public Boolean ScriptKill() => this.Execute(CommandType.SCRIPTKILL, null, result => result.OK);
        /// <summary>
        /// 杀死当前正在运行的 Lua 脚本
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> ScriptKillAsync() => await this.ExecuteAsync(CommandType.SCRIPTKILL, null, async result => await Task.FromResult(result.OK));
        #endregion

        #region 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本
        /// <summary>
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <returns>返回sha1编码</returns>
        public string ScriptLoad(string script)
        {
            if (script.IsNullOrEmpty()) return String.Empty;
            return this.Execute(CommandType.SCRIPTLOAD, null, result => result.OK ? (string)result.Value : String.Empty, script);
        }
        /// <summary>
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <returns>返回sha1编码</returns>
        public async Task<string> ScriptLoadAsync(string script)
        {
            if (script.IsNullOrEmpty()) return String.Empty;
            return await this.ExecuteAsync(CommandType.SCRIPTLOAD, null, async result => await Task.FromResult(result.OK ? (string)result.Value : String.Empty), script);
        }
        #endregion

        #endregion
    }
}