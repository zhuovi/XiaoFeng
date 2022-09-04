using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-02 10:14:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 消费者组结果
    /// </summary>
    public class ConsumerGroupXPendingModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConsumerGroupXPendingModel()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 待处理消息总数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 待处理消息中最小ID
        /// </summary>
        public string MinID { get; set; }
        /// <summary>
        /// 待处理消息中最大ID
        /// </summary>
        public string MaxID { get; set; }
        /// <summary>
        /// 消费者数据
        /// </summary>
        public List<Consumer> Consumers { get; set; } = new List<Consumer>();
        #endregion

        #region 方法

        #endregion

        /// <summary>
        /// 消费者类
        /// </summary>
        public class Consumer
        {
            /// <summary>
            /// 消费者
            /// </summary>
            public string ConsumerName { get; set; }
            /// <summary>
            /// 自上次将此消息传递给此使用者以来经过的毫秒数。
            /// </summary>
            public long Milliseconds { get; set; }
            /// <summary>
            /// 待处理数据数量
            /// </summary>
            public int Count { get; set; }
        }
    }
}