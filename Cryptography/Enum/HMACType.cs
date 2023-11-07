using System.ComponentModel;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-11-17 15:30:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    #region HMAC类型
    /// <summary>
    /// HMAC类型
    /// </summary>
    public enum HMACType
    {
        /// <summary>
        /// MD5
        /// </summary>
        [Description("HMACMD5")]
        MD5 = 0,
        /// <summary>
        /// SHA1
        /// </summary>
        [Description("HMACSHA1")]
        SHA1 = 1,
        /// <summary>
        /// SHA256
        /// </summary>
        [Description("HMACSHA256")]
        SHA256 = 2,
        /// <summary>
        /// SHA384
        /// </summary>
        [Description("HMACSHA384")]
        SHA384 = 3,
        /// <summary>
        /// SHA512
        /// </summary>
        [Description("HMACSHA512")]
        SHA512 = 4,
        /// <summary>
        /// RIPEMD160
        /// </summary>
        [Description("HMACRIPEMD160")]
        RIPEMD160 = 5,
        /// <summary>
        /// MACTripleDES
        /// </summary>
        [Description("MACTripleDES")]
        MACTripleDES = 6
    }
    #endregion
}