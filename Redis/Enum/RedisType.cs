/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-15 15:49:25                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis值 类型
    /// </summary>
    public enum RedisType
    {
        /// <summary>
        /// 空值
        /// </summary>
        Null = 0,
        /// <summary>
        /// 布尔值
        /// </summary>
        Boolean = 1,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 2,
        /// <summary>
        /// 整型
        /// </summary>
        Int = 3,
        /// <summary>
        /// 数组
        /// </summary>
        Array = 4,
        /// <summary>
        /// Redis Key类型
        /// </summary>
        KeyType = 5,
        /// <summary>
        /// 字典
        /// </summary>
        Dictionary = 6,
        /// <summary>
        /// 出错
        /// </summary>
        Model = 7
    }
}