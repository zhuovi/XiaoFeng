using System.ComponentModel;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-13 17:01:01                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /// <summary>
    /// 响应码
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        [EnumName("Success")]
        Success = 0x0,
        /// <summary>
        /// KEY不存在
        /// </summary>
        [Description("KEY不存在")]
        [EnumName("KeyNotFound")]
        KeyNotFound = 0x01,
        /// <summary>
        /// KEY已存在
        /// </summary>
        [Description("KEY已存在")]
        [EnumName("KeyExists")]
        KeyExists = 0x02,
        /// <summary>
        /// 值太大
        /// </summary>
        [Description("值太大")]
        [EnumName("ValueTooLarge")]
        ValueTooLarge = 0x03,
        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        [EnumName("InvalidArguments")]
        InvalidArguments = 0x04,
        /// <summary>
        /// 项未存储
        /// </summary>
        [Description("项未存储")]
        [EnumName("ItemNotStored")]
        ItemNotStored = 0x05,
        /// <summary>
        /// 在非数字上执行Incr/Decr
        /// </summary>
        [Description("在非数字上执行Incr/Decr")]
        [EnumName("IncrOrDecrNonNumericValue")]
        IncrOrDecrNonNumericValue = 0x06,
        /// <summary>
        /// VBucket属于其他的Server
        /// </summary>
        [Description("VBucket属于其他的Server")]
        [EnumName("VBucket")]
        VBucket = 0x07,
        /// <summary>
        /// 认证错误
        /// </summary>
        [Description("认证错误")]
        [EnumName("AuthenticationError")]
        AuthenticationError = 0x08,
        /// <summary>
        /// 认证继续
        /// </summary>
        [Description("认证继续")]
        [EnumName("AuthenticationContinue")]
        AuthenticationContinue = 0x09,
        /// <summary>
        /// 未知命令
        /// </summary>
        [Description("未知命令")]
        [EnumName("UnknownCommand")]
        UnknownCommand = 0x81,
        /// <summary>
        /// 内存溢出
        /// </summary>
        [Description("内存溢出")]
        [EnumName("OutMemory")]
        OutMemory = 0x82
    }
}