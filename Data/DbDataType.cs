using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-07-13 11:15:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum DbDataType
    {
        /// <summary>
        /// varchar
        /// </summary>
        VARCHAR,
        /// <summary>
        /// nvarchar
        /// </summary>
        NVARCHAR,
        /// <summary>
        /// uuid guid
        /// </summary>
        UUID,
        /// <summary>
        /// text
        /// </summary>
        TEXT,
        /// <summary>
        /// ntext
        /// </summary>
        NTEXT,
        /// <summary>
        /// 时间
        /// </summary>
        DATETIME,
        /// <summary>
        /// 日期
        /// </summary>
        DATE,
        /// <summary>
        /// 时间
        /// </summary>
        TIME,
        /// <summary>
        /// 时间戳
        /// </summary>
        TIMESTAMP,
        /// <summary>
        /// 布尔值
        /// </summary>
        BOOLEAN,
        /// <summary>
        /// BIT
        /// </summary>
        BIT,
        /// <summary>
        /// bigint
        /// </summary>
        BIGINT,
        /// <summary>
        /// integer
        /// </summary>
        INTEGER,
        /// <summary>
        /// int
        /// </summary>
        INT,
        /// <summary>
        /// 小整型
        /// </summary>
        SMALLINT,
        /// <summary>
        /// double
        /// </summary>
        DOUBLE,
        /// <summary>
        /// decimal
        /// </summary>
        DECIMAL,
        /// <summary>
        /// money
        /// </summary>
        MONEY,
        /// <summary>
        /// float
        /// </summary>
        FLOAT,
        /// <summary>
        /// blob
        /// </summary>
        BLOB,
        /// <summary>
        /// 二进制
        /// </summary>
        VARBINARY,
        /// <summary>
        /// 文件流
        /// </summary>
        BFILE
    }
    /// <summary>
    /// 默认值
    /// </summary>
    public enum DefaultValueType
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        NOW,
        /// <summary>
        /// 当前时间戳
        /// </summary>
        TIMESTAMP
    }
}