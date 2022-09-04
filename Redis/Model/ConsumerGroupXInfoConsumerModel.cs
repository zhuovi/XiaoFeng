using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-02 17:39:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// XInfoConsumerModel 类说明
    /// </summary>
    public class ConsumerGroupXInfoConsumerModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConsumerGroupXInfoConsumerModel()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 消费者的名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 客户端的未决消息数，即已传递但尚未确认的消息
        /// </summary>
        public int Pending { get; set; }
        /// <summary>
        /// 自消费者上次与服务器交互以来经过的毫秒数
        /// </summary>
        public int Idle { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}