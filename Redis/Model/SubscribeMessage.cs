using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Model;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-09 08:54:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 订阅消息
    /// </summary>
    public class SubscribeMessage : Entity<SubscribeMessage>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SubscribeMessage()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 频道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}