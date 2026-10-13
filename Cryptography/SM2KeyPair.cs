using System;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 10:30:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// SM2密钥对
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 10:30:00
    /// Description : SM2椭圆曲线密钥对，私钥为32字节(64个十六进制字符)，公钥为65字节未压缩格式(04||X||Y 共130个十六进制字符)。
    /// </summary>
    public class SM2KeyPair
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SM2KeyPair()
        {
        }
        /// <summary>
        /// 设置密钥对
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="publicKey">公钥</param>
        public SM2KeyPair(string privateKey, string publicKey)
        {
            this.PrivateKey = privateKey;
            this.PublicKey = publicKey;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 私钥 64个十六进制字符
        /// </summary>
        public string PrivateKey { get; set; } = "";
        /// <summary>
        /// 公钥 130个十六进制字符 04||X||Y
        /// </summary>
        public string PublicKey { get; set; } = "";
        /// <summary>
        /// 密钥长度
        /// </summary>
        public int KeySize => 256;
        /// <summary>
        /// 私钥字节
        /// </summary>
        public byte[] PrivateKeyBytes => SM2Encryption.FromHex(this.PrivateKey);
        /// <summary>
        /// 公钥字节
        /// </summary>
        public byte[] PublicKeyBytes => SM2Encryption.FromHex(this.PublicKey);
        #endregion

        #region 方法
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="userId">用户ID</param>
        /// <returns>64字节签名值 r||s</returns>
        public byte[] Sign(byte[] data, string userId = null) => SM2Encryption.Sign(data, this.PrivateKey, this.PublicKey, userId);
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">签名值</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否一致</returns>
        public Boolean Verify(byte[] data, byte[] signature, string userId = null) => SM2Encryption.Verify(data, signature, this.PublicKey, userId);
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>密文</returns>
        public byte[] Encrypt(byte[] data) => SM2Encryption.Encrypt(data, this.PublicKey);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <returns>明文</returns>
        public byte[] Decrypt(byte[] data) => SM2Encryption.Decrypt(data, this.PrivateKey);
        #endregion
    }
}
