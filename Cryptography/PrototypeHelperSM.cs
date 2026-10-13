using System;
using System.Text;
using XiaoFeng.Cryptography;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 12:00:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 国密算法扩展方法
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 12:00:00
    /// Description : 为字符串与字节数组提供SM3摘要、SM2加密解密的扩展方法，方便直接调用国密算法。
    /// </summary>
    public static class PrototypeHelperSM
    {
        #region SM3摘要
        /// <summary>
        /// 字符串SM3摘要
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="outputMode">输出模式 默认为十六进制</param>
        /// <returns>摘要值</returns>
        public static string SM3Encrypt(this string _, OutputMode outputMode = OutputMode.Hex) =>
            _.IsNullOrEmpty() ? string.Empty : new SM3Encryption().Encrypt(_, outputMode);
        /// <summary>
        /// 字节数组SM3摘要
        /// </summary>
        /// <param name="_">字节数组</param>
        /// <param name="outputMode">输出模式 默认为十六进制</param>
        /// <returns>摘要值</returns>
        public static string SM3Encrypt(this byte[] _, OutputMode outputMode = OutputMode.Hex)
        {
            if (_ == null || _.Length == 0) return "";
            var hash = SM3Encryption.Hash(_);
            return outputMode == OutputMode.Base64 ? hash.ToBase64String() : hash.ToHexString();
        }
        #endregion

        #region SM2加密
        /// <summary>
        /// SM2加密
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="publicKey">公钥 04开头130位十六进制字符串或PEM</param>
        /// <param name="outputMode">输出模式 默认为Base64</param>
        /// <returns>密文</returns>
        public static string SM2Encrypt(this string _, string publicKey, OutputMode outputMode = OutputMode.Base64) =>
            new SM2Encryption().Encrypt(_, publicKey, outputMode);
        /// <summary>
        /// SM2解密
        /// </summary>
        /// <param name="_">密文</param>
        /// <param name="privateKey">私钥 64位十六进制字符串或PEM</param>
        /// <param name="outputMode">输出模式 默认为Base64</param>
        /// <returns>明文</returns>
        public static string SM2Decrypt(this string _, string privateKey, OutputMode outputMode = OutputMode.Base64) =>
            new SM2Encryption().Decrypt(_, privateKey, outputMode);
        #endregion
    }
}
