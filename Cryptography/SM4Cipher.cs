using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-04-18 18:03:46                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// SM4加密器
    /// </summary>
    public class SM4Cipher: SymmetricAlgorithm
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SM4Cipher()
        {
            this.KeySize = 128;
            this.BlockSize = 128;
            this.Mode = CipherMode.ECB;
            this.Padding = PaddingMode.PKCS7;
        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 创建对称加密器对象。
        /// </summary>
        /// <returns>对称加密器对象。</returns>
        public override ICryptoTransform CreateEncryptor()
        {
            if (this.Key == null) this.GenerateKey();
            if (this.IV == null) this.GenerateIV();
            return this.CreateEncryptor(this.Key, this.IV);
        }
        /// <summary>
        /// 创建对称加密器对象。
        /// </summary>
        /// <param name="rgbKey">用于对称算法的密钥。</param>
        /// <param name="rgbIV">用于对称算法的初始化向量。</param>
        /// <returns></returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) => new SM4Transform(rgbKey, rgbIV, true);
        /// <summary>
        /// 创建对称解密器对象。
        /// </summary>
        /// <param name="rgbKey">用于对称算法的密钥。</param>
        /// <param name="rgbIV">用于对称算法的初始化向量。</param>
        /// <returns>对称解密器对象。</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) => new SM4Transform(rgbKey, rgbIV, false);
        /// <summary>
        /// 创建对称解密器对象。
        /// </summary>
        /// <returns>对称解密器对象。</returns>
        public override ICryptoTransform CreateDecryptor() => this.CreateDecryptor(this.Key, this.IV);
        /// <summary>
        /// 生成用于该算法的随机密钥 
        /// </summary>
        public override void GenerateKey()
        {
            this.Key = new byte[this.KeySize];
            this.Key.For(0, this.KeySize, i =>
            {
                this.Key[i] = (byte)RandomHelper.GetRandomInt(0, 255);
            });
        }
        /// <summary>
        /// 生成用于该算法的随机初始化向量 
        /// </summary>
        public override void GenerateIV()
        {
            this.IV = new byte[this.BlockSize];
            this.IV.For(0, this.BlockSize, i =>
            {
                this.IV[i] = (byte)RandomHelper.GetRandomInt(0, 255);
            });
        }
        #endregion
    }
}