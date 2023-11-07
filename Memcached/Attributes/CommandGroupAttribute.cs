using System;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 18:35:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Attributes
{
    /// <summary>
    /// 命令分组
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandGroupAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CommandGroupAttribute()
        {

        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="flags">属性</param>
        public CommandGroupAttribute(CommandFlags flags)
        {
            Flags = flags;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 标识
        /// </summary>
        public CommandFlags Flags { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}