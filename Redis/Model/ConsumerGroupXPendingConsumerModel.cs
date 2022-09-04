using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-02 10:24:25                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 消费者组
    /// </summary>
    public class ConsumerGroupXPendingConsumerModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConsumerGroupXPendingConsumerModel()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageID { get; set; }
        /// <summary>
        /// 消费者
        /// </summary>
        public string ConsumerName { get; set; }
        /// <summary>
        /// 自上次将此消息传递给此使用者以来经过的毫秒数。
        /// </summary>
        public long Milliseconds { get; set; }
        /// <summary>
        /// 此消息的传递次数
        /// </summary>
        public int Count { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}