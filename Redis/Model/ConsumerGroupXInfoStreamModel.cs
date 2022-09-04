using System;
using System.Collections.Generic;
using System.Text;

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
    public class ConsumerGroupXInfoStreamModel
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConsumerGroupXInfoStreamModel()
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
        /// 为流定义的消费者组的数量
        /// </summary>
        public int Groups { get; set; }
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
        /// 流中第一个条目的 ID 和字段值元组
        /// </summary>
        public Entries FirstEntry { get; set; }
        /// <summary>
        /// 流中最后一个条目的 ID 和字段值元组
        /// </summary>
        public Entries LastEntry { get; set; }
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
            public Dictionary<string,string> Value { get; set; }
        }
    }
}