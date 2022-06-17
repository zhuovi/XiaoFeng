using System;
using System.Collections.Generic;
using System.Text;
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
    /// 发布订阅(PubSub)
    /// </summary>
    public partial class RedisClient : Disposable
    {
        #region 命令订阅一个或多个符合给定模式的频道
        /// <summary>
        /// 命令订阅一个或多个符合给定模式的频道
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="patterns">频道名称</param>
        /// <returns></returns>
        public List<string> Psubscribe(int? dbNum,params string[] patterns)
        {
            if (patterns.Length == 0) return null;
            return this.Execute(CommandType.PSUBSCRIBE, dbNum, result => (List<string>)result.Value, patterns);
        }
        #endregion
    }
}