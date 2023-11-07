/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-25 11:59:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Json
{
    /// <summary>
    /// Json类型
    /// </summary>
    public enum JsonType
    {
        /// <summary>
        /// 对象
        /// </summary>
        Object = 0,
        /// <summary>
        /// 数组
        /// </summary>
        Array = 1,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 2,
        /// <summary>
        /// 数字
        /// </summary>
        Number = 3,
        /// <summary>
        /// 布尔值
        /// </summary>
        Bool = 4,
        /// <summary>
        /// 日期
        /// </summary>
        DateTime = 6,
        /// <summary>
        /// Guid
        /// </summary>
        Guid = 7,
        /// <summary>
        /// 浮点
        /// </summary>
        Float = 8,
        /// <summary>
        /// 空
        /// </summary>
        Null = 10,
        /// <summary>
        /// 类型
        /// </summary>
        Type = 11,
        /// <summary>
        /// 字节
        /// </summary>
        Byte = 12,
        /// <summary>
        /// 字符
        /// </summary>
        Char = 13
    }
}