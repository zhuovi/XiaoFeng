using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-02 18:51:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// XInfoGroupModel 类说明
    /// </summary>
    public class ConsumerGroupXInfoGroupsModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConsumerGroupXInfoGroupsModel()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 消费者组的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 组中消费者的数量
        /// </summary>
        public int Consumers { get; set; }
        /// <summary>
        /// 组的未决条目列表 (PEL) 的长度，即已传递但尚未确认的消息
        /// </summary>
        public int Pending { get; set; }
        /// <summary>
        /// 最后一个条目的 ID 交付组的消费者
        /// </summary>
        public string LastDeliveredID{get;set;}
        /// <summary>
        /// 传递给组消费者的最后一个条目的逻辑“读取计数器”
        /// </summary>
        public int EntriesRead { get; set; }
        /// <summary>
        /// 流中仍在等待交付给组的消费者的条目数，如果无法确定该数字，则为 NULL。
        /// </summary>
        public int Lag { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}