using System;
using System.IO;
using System.Security.Cryptography;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-12-23 下午 09:06:44          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// HMAC加密
    /// Version : 1.0.0
    /// CrateTime : 2020-12-23 下午 09:06:44
    /// 更新说明
    /// </summary>
    public class HMACEncryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public HMACEncryption()
        {
        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7) => null;
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public byte[] Encode(byte[] data, byte[] slatKey, HMACType type = HMACType.MD5)
        {
            if (data == null) return Array.Empty<byte>();
            
            var hmac = HMAC.Create(type.GetDescription());
            if (slatKey != null && slatKey.Length > 0) hmac.Key = slatKey;
            return hmac.ComputeHash(data);
            /*HMAC hmac;
            if (type == HMACType.MD5)
                hmac = new HMACMD5(slatKey);
            else if (type == HMACType.RIPEMD160)
                hmac = new HMACRIPEMD160(slatKey);
            else if (type == HMACType.SHA1)
                hmac = new HMACSHA1(slatKey);
            else if (type == HMACType.SHA256)
                hmac = new HMACSHA256(slatKey);
            else if (type == HMACType.SHA384)
                hmac = new HMACSHA384(slatKey);
            else if (type == HMACType.SHA512)
                hmac = new HMACSHA512(slatKey);
            else hmac = new HMACMD5(slatKey);
            return hmac.ComputeHash(data);*/
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inputStream">数据流</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public byte[] Encode(Stream inputStream, HMACType type = HMACType.MD5)
        {
            if (inputStream == null || inputStream.Length == 0) return Array.Empty<byte>();
            return HMAC.Create(type.GetDescription()).ComputeHash(inputStream);
            /*HMAC hmac;
            if (type == HMACType.MD5)
                hmac = new HMACMD5(slatKey);
            else if (type == HMACType.RIPEMD160)
                hmac = new HMACRIPEMD160(slatKey);
            else if (type == HMACType.SHA1)
                hmac = new HMACSHA1(slatKey);
            else if (type == HMACType.SHA256)
                hmac = new HMACSHA256(slatKey);
            else if (type == HMACType.SHA384)
                hmac = new HMACSHA384(slatKey);
            else if (type == HMACType.SHA512)
                hmac = new HMACSHA512(slatKey);
            else hmac = new HMACMD5(slatKey);
            return hmac.ComputeHash(data);*/
        }
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="type">加密类型</param>
        /// <returns>加密后的字节</returns>
        public byte[] Encrypt(byte[] data, string slatKey, HMACType type = HMACType.MD5) => this.Encode(data, this.GetBytes(slatKey), type);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文字节</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="type">加密类型</param>
        /// <param name="outputMode">输出类型</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt(byte[] data, string slatKey, HMACType type = HMACType.MD5, OutputMode outputMode = OutputMode.Base64) => this.OutputString(this.Encrypt(data, slatKey, type), outputMode);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="type">加密类型</param>
        /// <param name="outputMode">输出类型</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt(string data, string slatKey, HMACType type = HMACType.MD5, OutputMode outputMode = OutputMode.Base64) => this.OutputString(this.Encrypt(this.GetBytes(data), slatKey, type), outputMode);
        ///<inheritdoc/>
        public override byte[] Encrypt(byte[] data, string slatKey = "", string vectorKey = "") => this.Encrypt(data, slatKey, HMACType.MD5);
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Encrypt(data, slatKey, HMACType.MD5, outputMode);
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey, OutputMode outputMode) => this.Encrypt(data, slatKey, HMACType.MD5, outputMode);
        ///<inheritdoc/>
        public override string Encrypt(string data, OutputMode outputMode) => this.Encrypt(data, this.Key, outputMode);
        #endregion
    }
}