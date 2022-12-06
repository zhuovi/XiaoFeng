using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
    /// SHA加密
    /// Version : 1.0.0
    /// CrateTime : 2020-12-23 下午 09:06:44
    /// 更新说明
    /// </summary>
    public class SHAEncryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SHAEncryption()
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
        /// <param name="type">类型</param>
        /// <returns></returns>
        public byte[] Encode(byte[] data, SHAType type = SHAType.SHA1)
        {
            if (data == null) return Array.Empty<byte>();
            return HashAlgorithm.Create(type.GetDescription()).ComputeHash(data);
            /*HashAlgorithm sha;
            if (type == SHAType.SHA1)
                sha = new SHA1CryptoServiceProvider();
            else if (type == SHAType.SHA256)
                sha = new SHA256CryptoServiceProvider();
            else if (type == SHAType.SHA384)
                sha = new SHA384CryptoServiceProvider();
            else if (type == SHAType.SHA512)
                sha = new SHA512CryptoServiceProvider();
            else sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(data);*/
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inputStream">数据流</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public byte[] Encode(Stream inputStream, SHAType type = SHAType.SHA1)
        {
            if (inputStream == null || inputStream.Length == 0) return Array.Empty<byte>();
            return HashAlgorithm.Create(type.GetDescription()).ComputeHash(inputStream);
            /*HashAlgorithm sha;
            if (type == SHAType.SHA1)
                sha = new SHA1CryptoServiceProvider();
            else if (type == SHAType.SHA256)
                sha = new SHA256CryptoServiceProvider();
            else if (type == SHAType.SHA384)
                sha = new SHA384CryptoServiceProvider();
            else if (type == SHAType.SHA512)
                sha = new SHA512CryptoServiceProvider();
            else sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(inputStream);*/
        }
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="type">加密类型</param>
        /// <returns>加密后的字节</returns>
        public byte[] Encrypt(byte[] data, SHAType type = SHAType.SHA1) => this.Encode(data, type);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文字节</param>
        /// <param name="type">加密类型</param>
        /// <param name="outputMode">输出类型</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt(byte[] data, SHAType type = SHAType.SHA1, OutputMode outputMode = OutputMode.Base64) => this.OutputString(this.Encrypt(data, type), outputMode);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="type">加密类型</param>
        /// <returns>加密后的字节</returns>
        public byte[] Encrypt(Stream data, SHAType type = SHAType.SHA1) => this.Encode(data, type);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文字节</param>
        /// <param name="type">加密类型</param>
        /// <param name="outputMode">输出类型</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt(Stream data, SHAType type = SHAType.SHA1, OutputMode outputMode = OutputMode.Base64) => this.OutputString(this.Encrypt(data, type), outputMode);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="type">加密类型</param>
        /// <param name="outputMode">输出类型</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt(string data, SHAType type = SHAType.SHA1, OutputMode outputMode = OutputMode.Base64) => this.OutputString(this.Encrypt(this.GetBytes(data), type), outputMode);
        ///<inheritdoc/>
        public override byte[] Encrypt(byte[] data, string slatKey = "", string vectorKey = "") => this.Encrypt(data, SHAType.SHA1);
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Encrypt(data, SHAType.SHA1, outputMode);
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey, OutputMode outputMode) => this.Encrypt(data, SHAType.SHA1, outputMode);
        ///<inheritdoc/>
        public override string Encrypt(string data, OutputMode outputMode) => this.Encrypt(data, this.Key, outputMode);
        #endregion
    }
}