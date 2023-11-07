/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-01 14:09:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 消费者组位置类型
    /// </summary>
    public enum ConsumerGroupPosition
    {
        /// <summary>
        /// 从头开始
        /// </summary>
        TOP = 0,
        /// <summary>
        /// 从尾开始
        /// </summary>
        END = 1
    }
}