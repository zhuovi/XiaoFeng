using System.Collections.Generic;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-02 20:17:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// ConsumergroupXInfoStreamModel 类说明
    /// </summary>
    public class ConsumerGroupXInfoStreamFullModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConsumerGroupXInfoStreamFullModel()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 流中的条目数
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 底层基数数据结构中的键数
        /// </summary>
        public int RadixTreeKeys { get; set; }
        /// <summary>
        /// 底层基数数据结构中的节点数
        /// </summary>
        public int RadixTreeNodes { get; set; }
        /// <summary>
        /// 最近添加到流中的条目的 ID
        /// </summary>
        public string LastGeneratedID { get; set; }
        /// <summary>
        /// 从流中删除的最大条目 ID
        /// </summary>
        public string MaxDeletedEntryID { get; set; }
        /// <summary>
        /// 在其生命周期内添加到流中的所有条目的计数
        /// </summary>
        public int EntriesAdded { get; set; }
        /// <summary>
        /// 流中所有条目的 ID 和字段值元组
        /// </summary>
        public List<Entries> Entires { get; set; }
        /// <summary>
        /// 组
        /// </summary>
        public List<FullGroups> Groups { get; set; }
        #endregion

        #region 方法

        #endregion

        /// <summary>
        /// 实体类
        /// </summary>
        public class Entries
        {
            /// <summary>
            /// 消息ID
            /// </summary>
            public string ID { get; set; }
            /// <summary>
            /// 数据
            /// </summary>
            public Dictionary<string, string> Value { get; set; }
        }
        /// <summary>
        /// 全组
        /// </summary>
        public class FullGroups
        {
            #region 属性
            /// <summary>
            /// 消费者组的名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 最后一个条目的 ID 交付组的消费者
            /// </summary>
            public string LastDeliveredID { get; set; }
            /// <summary>
            /// 传递给组消费者的最后一个条目的逻辑“读取计数器”
            /// </summary>
            public int EntriesRead { get; set; }
            /// <summary>
            /// 流中仍在等待交付给组的消费者的条目数，如果无法确定该数字，则为 NULL。
            /// </summary>
            public int Lag { get; set; }
            /// <summary>
            /// 未决条目列表 (PEL) 的长度
            /// </summary>
            public int PelCount { get; set; }
            /// <summary>
            /// 组的未决条目列表 (PEL)，即已传递但尚未确认的消息
            /// </summary>
            public List<ConsumerGroupXPendingConsumerModel> Pending { get; set; }
            /// <summary>
            /// 消费者组
            /// </summary>
            public List<FullCounsumer> Consumers { get; set; }
            #endregion
        }
        /// <summary>
        /// 全消费者
        /// </summary>
        public class FullCounsumer
        {
            /// <summary>
            /// 消费者名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 自上次将此消息传递给此使用者以来经过的毫秒数
            /// </summary>
            public long SeenTime { get; set; }
            /// <summary>
            /// 未决条目列表 (PEL) 的长度
            /// </summary>
            public int PelCount { get; set; }
            /// <summary>
            /// 组的未决条目列表 (PEL)，即已传递但尚未确认的消息
            /// </summary>
            public List<ConsumerGroupXPendingModel> Pending { get; set; }
        }
    }
}