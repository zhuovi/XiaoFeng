using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-12 15:45:05                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis.Clusters
{
    /// <summary>
    /// 集群节点
    /// </summary>
    public class ClusterNode
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ClusterNode()
        {
            
        }
        #endregion

        #region 属性
        /// <summary>
        /// 标识ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 节点
        /// </summary>
        public RedisEndPoint EndPoint { get; set; }
        /// <summary>
        /// 主节点
        /// </summary>
        public RedisEndPoint MainPoint { get; set; }
        /// <summary>
        /// 从节点集合
        /// </summary>
        public IList<ClusterNode> Slaves { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 节点地址
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.EndPoint.ToString();
        }
        #endregion
    }
}