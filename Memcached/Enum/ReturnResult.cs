using System.ComponentModel;
/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 15:50:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 服务器响应类型
    /// </summary>
    public enum ReturnResult
    {
        /// <summary>
        /// 值存储成功
        /// </summary>
        [Description("值存储成功")]
        [EnumName("STORED")]
        STORED = 200,
        /// <summary>
        /// 值存储失败
        /// </summary>
        [Description("值存储失败")]
        [EnumName("NOT_STORED")]
        NOTSTORED = 501,
        /// <summary>
        /// cas中要存储的对象已存在
        /// </summary>
        [Description("cas中要存储的对象已存在")]
        [EnumName("EXISTS")]
        EXISTS = 300,
        /// <summary>
        /// 要修改的对象不存在
        /// </summary>
        [Description("要修改的对象不存在")]
        [EnumName("NOT_FOUND")]
        NOT_FOUND = 301,
        /// <summary>
        /// 提交了未知的命令
        /// </summary>
        [Description("提交了未知的命令")]
        [EnumName("ERROR")]
        ERROR = 500,
        /// <summary>
        /// 客户端输入有误，具体的错误信息存放在 errorstring中
        /// </summary>
        [Description("客户端输入有误")]
        [EnumName("CLIENT_ERROR")]
        CLIENTERROR = 502,
        /// <summary>
        /// 服务器端异常，具体的错误信息存放在 errorstring中
        /// </summary>
        [Description("服务器端异常")]
        [EnumName("SERVER_ERROR")]
        SERVERERROR = 503,
        /// <summary>
        /// 对象已经被删除
        /// </summary>
        [Description("对象已经被删除")]
        [EnumName("DELETED")]
        DELETED = 201,
        /// <summary>
        /// 服务器端返回结束
        /// </summary>
        [Description("服务器端返回结束")]
        [EnumName("END")]
        END = 999,
        /// <summary>
        /// 返回要查询的key对应的对象
        /// </summary>
        [Description("返回要查询的key对应的对象")]
        [EnumName("VALUE")]
        VALUE = 202,
        /// <summary>
        /// 统计信息
        /// </summary>
        [Description("统计信息")]
        [EnumName("STAT")]
        STAT = 203,
        /// <summary>
        /// 对象已更新
        /// </summary>
        [Description("对象已更新")]
        [EnumName("TOUCHED")]
        TOUCHED = 204
    }
}