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
    /// 证书注销原因
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:00:00
    /// </summary>
    public enum RevocationReason
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [Description("未指定")]
        Unspecified = 0,
        /// <summary>
        /// 私钥泄露
        /// </summary>
        [Description("私钥泄露")]
        KeyCompromise = 1,
        /// <summary>
        /// 签发机构私钥泄露
        /// </summary>
        [Description("签发机构私钥泄露")]
        CACompromise = 2,
        /// <summary>
        /// 主体信息变更
        /// </summary>
        [Description("主体信息变更")]
        AffiliationChanged = 3,
        /// <summary>
        /// 已被新证书替换
        /// </summary>
        [Description("已被新证书替换")]
        Superseded = 4,
        /// <summary>
        /// 停止使用
        /// </summary>
        [Description("停止使用")]
        CessationOfOperation = 5,
        /// <summary>
        /// 证书挂起
        /// </summary>
        [Description("证书挂起")]
        CertificateHold = 6,
        /// <summary>
        /// 用户主动注销
        /// </summary>
        [Description("用户主动注销")]
        UserRequest = 7,
        /// <summary>
        /// 证书续期
        /// </summary>
        [Description("证书续期")]
        Renew = 8
    }
}
