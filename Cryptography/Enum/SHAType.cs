using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-11-17 15:31:11                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    #region SHA类型
    /// <summary>
    /// SHA类型
    /// </summary>
    public enum SHAType
    {
        /// <summary>
        /// SHA1
        /// </summary>
        [Description("SHA1")]
        SHA1 = 1,
        /// <summary>
        /// SHA256
        /// </summary>
        [Description("SHA256")]
        SHA256 = 2,
        /// <summary>
        /// SHA384
        /// </summary>
        [Description("SHA384")]
        SHA384 = 3,
        /// <summary>
        /// SHA512
        /// </summary>
        [Description("SHA512")]
        SHA512 = 4,
        /// <summary>
        /// MD5
        /// </summary>
        [Description("MD5")]
        MD5 = 5
    }
    #endregion
}