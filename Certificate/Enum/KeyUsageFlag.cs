using System;
using System.ComponentModel;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:00:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// 证书密钥用法
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:00:00
    /// </summary>
    [Flags]
    public enum KeyUsageFlag
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 数字签名
        /// </summary>
        [Description("数字签名")]
        DigitalSignature = 1,
        /// <summary>
        /// 不可否认
        /// </summary>
        [Description("不可否认")]
        NonRepudiation = 2,
        /// <summary>
        /// 密钥加密
        /// </summary>
        [Description("密钥加密")]
        KeyEncipherment = 4,
        /// <summary>
        /// 数据加密
        /// </summary>
        [Description("数据加密")]
        DataEncipherment = 8,
        /// <summary>
        /// 密钥协商
        /// </summary>
        [Description("密钥协商")]
        KeyAgreement = 16,
        /// <summary>
        /// 签发证书签名
        /// </summary>
        [Description("签发证书")]
        KeyCertSign = 32,
        /// <summary>
        /// 吊销列表签名
        /// </summary>
        [Description("吊销列表签名")]
        CRLSign = 64,
        /// <summary>
        /// 仅加密
        /// </summary>
        [Description("仅加密")]
        EncipherOnly = 128,
        /// <summary>
        /// 仅解密
        /// </summary>
        [Description("仅解密")]
        DecipherOnly = 256
    }
}
