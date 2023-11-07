using System.ComponentModel;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/25 17:00:28          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// 传输模式:二进制类型、ASCII类型
    /// </summary>
    public enum TransferType
    {
        /// <summary>
        /// 二进制
        /// </summary>
        [Description("I")]
        Binary = 0,
        /// <summary>
        /// ASCII
        /// </summary>
        [Description("A")]
        ASCII = 1
    }
}