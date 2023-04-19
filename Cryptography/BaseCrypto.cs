using System;
using System.Security.Cryptography;
using System.Text;
/************************************************************
 *  Copyright © (2020)www.fayelf.com All Rights Reserved.   *
 *  Author : jacky                                          *
 *  QQ : 7092734                                            *
 *  Email : jacky@fayelf.com                                *
 *  Site : www.fayelf.com                                   *
 *  Create Time : 2020-12-22 16:07:10                       *
 *  Version : v 1.0.0                                       *
 ************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 加密基类
    /// </summary>
    public abstract class BaseCrypto : EntityBase, ICryptography
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseCrypto()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 默认key
        /// </summary>
        public String Key { get; set; } = "07092734";
        /// <summary>
        /// 默认向量
        /// </summary>
        public String VectorKey { get; set; } = "07092734";
        /// <summary>
        /// 默认编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 默认输出编码
        /// </summary>
        public OutputMode OutputMode { get; set; } = OutputMode.Base64;
        #endregion

        #region 方法

        #region 获取字节
        /// <summary>
        /// 获取字节
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        internal byte[] GetBytes(string data) => data.GetBytes(this.Encoding);
        #endregion

        #region 获取字符串
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        internal string GetString(byte[] data) => data.GetString(this.Encoding);
        #endregion

        #region 获取key
        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="defaultKey">默认密钥</param>
        /// <returns></returns>
        protected virtual string GetKey(string key, string defaultKey = "www.fayelf.com")
        {
            if (key.IsNullOrEmpty())
                key = defaultKey;
            if (key.Length < 16)
                key = key.PadRight(16, '0');
            else if (key.Length > 16 && key.Length < 24)
                key = key.Substring(0, 16);
            else if (key.Length > 24 && key.Length < 32)
                key = key.Substring(0, 24);
            else if (key.Length > 32)
                key = key.Substring(0, 32);
            return key;
        }
        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        protected virtual byte[] GetKey(byte[] key)
        {
            if (key == null) key = this.Key.GetBytes();
            return this.GetKey(key.GetString(this.Encoding)).GetBytes();
        }
        #endregion

        #region 获取偏移量
        /// <summary>
        /// 获取偏移量
        /// </summary>
        /// <param name="iv">偏移量</param>
        /// <param name="defaultIV">默认偏移量</param>
        /// <returns></returns>
        protected internal string GetVector(string iv, string defaultIV = "www.fayelf.com")
        {
            if (iv.IsNullOrEmpty()) iv = defaultIV;
            if (iv.Length < 8) iv = iv.PadRight(8, '0');
            else if (iv.Length > 8 && iv.Length < 16) iv = iv.PadRight(16, '0');
            else if (iv.Length > 16) iv = iv.Substring(0, 16);
            return iv;
        }
        /// <summary>
        /// 获取偏移量
        /// </summary>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        protected internal byte[] GetVector(byte[] iv)
        {
            return this.GetVector(iv.GetString()).GetBytes();
        }
        #endregion

        #region 输出数据
        /// <summary>
        /// 输出数据
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <param name="mode">输出模式</param>
        /// <returns>返回字符串</returns>
        protected virtual string OutputString(byte[] bytes, OutputMode mode = OutputMode.Base64) => bytes.ToString(mode);
        #endregion

        #region 输入数据
        /// <summary>
        /// 输入数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="mode">输入模式</param>
        /// <returns>返回字节</returns>
        protected internal byte[] InputBytes(string data, OutputMode mode = OutputMode.Base64)
        {
            switch (mode)
            {
                case OutputMode.Base64:
                    return data.FromBase64StringToBytes();
                case OutputMode.Hex:
                    return data.HexStringToBytes();
                default:
                    return data.FromBase64StringToBytes();
            }
        }
        #endregion

        #region 密码方法
        /// <summary>
        /// 密码方法
        /// </summary>
        /// <param name="data">原数据</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="vector">向量</param>
        /// <param name="type">密码类型</param>
        /// <param name="cipherMode">密码模式</param>
        /// <param name="paddingMode">填充类型</param>
        /// <returns>密码</returns>
        public abstract byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7);
        ///<inheritdoc/>
        public virtual byte[] Encrypt(byte[] data, string slatKey = "", string vectorKey = "")
        {
            if (data == null || data.Length == 0) return Array.Empty<byte>();
            return this.Encode(data, this.GetBytes(this.GetKey(slatKey, this.Key)), this.GetBytes(this.GetVector(vectorKey, this.VectorKey)), CryptographyType.Encrypt);
        }
        ///<inheritdoc/>
        public virtual string Encrypt(string data, string key, string vector, CipherMode cipherMode, PaddingMode paddingModel = PaddingMode.PKCS7, OutputMode outputMode = OutputMode.Base64)
        {
            if (data.IsNullOrEmpty()) return string.Empty;
            return this.OutputString(this.Encode(this.GetBytes(data), this.GetBytes(this.GetKey(key, this.Key)), this.GetBytes(this.GetVector(vector, this.VectorKey)), CryptographyType.Encrypt, cipherMode, paddingModel), outputMode);
        }
        ///<inheritdoc/>
        public virtual string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Encrypt(data, slatKey, vectorKey, CipherMode.CBC, PaddingMode.PKCS7, outputMode);
        ///<inheritdoc/>
        public virtual string Encrypt(string data, string slatKey, OutputMode outputMode) => this.Encrypt(data, slatKey, "", outputMode);
        ///<inheritdoc/>
        public virtual string Encrypt(string data, OutputMode outputMode) => this.Encrypt(data, "", "", outputMode);
        ///<inheritdoc/>
        public virtual byte[] Decrypt(byte[] data, string slatKey = "", string vectorKey = "")
        {
            if (data == null || data.Length == 0) return Array.Empty<byte>();
            return this.Encode(data, this.GetBytes(this.GetKey(slatKey, this.Key)), this.GetBytes(this.GetVector(vectorKey, this.VectorKey)), CryptographyType.Decrypt);
        }
        ///<inheritdoc/>
        public virtual string Decrypt(string data, string key, string vector, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7, OutputMode outputMode = OutputMode.Base64)
        {
            if (data.IsNullOrEmpty()) return string.Empty;
            return this.GetString(this.Encode(this.InputBytes(data, outputMode), this.GetBytes(this.GetKey(key, this.Key)), this.GetBytes(this.GetVector(vector, this.VectorKey)), CryptographyType.Decrypt, cipherMode, paddingModel));
        }
        ///<inheritdoc/>
        public virtual string Decrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Decrypt(data, slatKey, vectorKey, CipherMode.CBC, PaddingMode.PKCS7, outputMode);
        ///<inheritdoc/>
        public virtual string Decrypt(string data, string slatKey, OutputMode outputMode) => this.Decrypt(data, slatKey, "", outputMode);
        ///<inheritdoc/>
        public virtual string Decrypt(string data, OutputMode outputMode) => this.Decrypt(data, "", "", outputMode);
        #endregion

        #endregion
    }
}