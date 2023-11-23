using System;
using System.IO;
using System.Security.Cryptography;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-04-18 18:48:34                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 基础类
    /// </summary>
    public class BaseSMSymmetricAlgorithm : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseSMSymmetricAlgorithm()
        {

        }
        /// <summary>
        /// 设置加密类型
        /// </summary>
        /// <param name="algorithmType">加密类型</param>
        public BaseSMSymmetricAlgorithm(SMType algorithmType)
        {
            this.AlgorithmType = algorithmType;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 加密类型
        /// </summary>
        public SMType AlgorithmType { get; set; } = SMType.FOUR;
        #endregion

        #region 方法
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="slatKey">key</param>
        /// <param name="vector">向量</param>
        /// <param name="algorithmType">加密类型</param>
        /// <param name="type">加密解密类型</param>
        /// <param name="cipherMode">密码模式</param>
        /// <param name="paddingMode">填充类型</param>
        /// <returns></returns>
        public byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, SMType algorithmType = SMType.FOUR, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.ECB, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (data == null) return null;
#if NETSTANDARD2_0
            using (var encryptor = Activator.CreateInstance(Type.GetType(algorithmType.GetDescription() + "Cipher")) as SymmetricAlgorithm)
#else
            using var encryptor = Activator.CreateInstance(Type.GetType(algorithmType.GetDescription() + "Cipher")) as SymmetricAlgorithm;
#endif
            {
                encryptor.Mode = cipherMode;
                encryptor.Padding = paddingMode;
                encryptor.Key = slatKey;
                encryptor.IV = vector;
                if (type == CryptographyType.Encrypt)
                {
#if NETSTANDARD2_0
                    using (var memory = new MemoryStream())
#else
                    using var memory = new MemoryStream();
#endif
                    {
#if NETSTANDARD2_0
                        using (var encoder = new CryptoStream(memory, encryptor.CreateEncryptor(new byte[encryptor.Key.Length].Write(slatKey), new byte[encryptor.IV.Length].Write(vector)), CryptoStreamMode.Write))
#else
                        using var encoder = new CryptoStream(memory, encryptor.CreateEncryptor(new byte[encryptor.Key.Length].Write(slatKey), new byte[encryptor.IV.Length].Write(vector)), CryptoStreamMode.Write);
#endif
                        {
                            encoder.Write(data, 0, data.Length);
                            encoder.FlushFinalBlock();
                        }
                        return memory.ToArray();
                    }
                }
                else
                {
#if NETSTANDARD2_0
                    using (var memory = new MemoryStream(data))
#else
                    using var memory = new MemoryStream(data);
#endif
                    {
                        try
                        {
#if NETSTANDARD2_0
                            using (var encoder = new CryptoStream(memory, encryptor.CreateDecryptor(new byte[encryptor.Key.Length].Write(slatKey), new byte[encryptor.IV.Length].Write(vector)), CryptoStreamMode.Read))
#else
                            using var encoder = new CryptoStream(memory, encryptor.CreateDecryptor(new byte[encryptor.Key.Length].Write(slatKey), new byte[encryptor.IV.Length].Write(vector)), CryptoStreamMode.Read);
#endif
                            {
#if NETSTANDARD2_0
                                using (var destMemory = new MemoryStream())
#else
                                using var destMemory = new MemoryStream();
#endif
                                {
                                    byte[] Buffer = new byte[1024];
                                    int readBytes = 0;
                                    while ((readBytes = encoder.Read(Buffer, 0, Buffer.Length)) > 0)
                                    {
                                        destMemory.Write(Buffer, 0, readBytes);
                                    }
                                    return destMemory.ToArray();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(ex);
                            return data;
                        }
                    }
                }
            }
        }
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.ECB, PaddingMode paddingMode = PaddingMode.PKCS7) => this.Encode(data, slatKey, vector, AlgorithmType, type, cipherMode, paddingMode);
#endregion
    }
}