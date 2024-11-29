using System;
using System.IO;
using System.Security.Cryptography;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-12-07 18:53:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 基础类
    /// </summary>
    public class BaseSymmetricAlgorithm : BaseCrypto
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseSymmetricAlgorithm()
        {

        }
        /// <summary>
        /// 设置加密类型
        /// </summary>
        /// <param name="algorithmType">加密类型</param>
        public BaseSymmetricAlgorithm(SymmetricAlgorithmType algorithmType)
        {
            this.AlgorithmType = algorithmType;
        }
        /// <summary>
        /// 加密类型
        /// </summary>
        public SymmetricAlgorithmType AlgorithmType { get; set; } = SymmetricAlgorithmType.AES;
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
        public virtual byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.AES, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (data == null) return Array.Empty<byte>();
            
            using (var encryptor = SymmetricAlgorithm.Create(algorithmType.GetDescription()))
            {
                encryptor.Mode = cipherMode;
                encryptor.Padding = paddingMode;
                //encryptor.Key = new byte[encryptor.Key.Length].Write(slatKey);
                //encryptor.IV = new byte[encryptor.IV.Length].Write(vector);
                if (slatKey.Length <= 16)
                    encryptor.Key = new byte[16].Write(slatKey);
                else if (slatKey.Length <= 24)
                    encryptor.Key = new byte[24].Write(slatKey);
                else if (slatKey.Length <= 32)
                    encryptor.Key = new byte[32].Write(slatKey);
                else encryptor.Key = new byte[32].Write(0, slatKey, 0, 32);

                if (cipherMode != CipherMode.ECB)
                {
                    if (vector.Length <= 16)
                        encryptor.IV = new byte[16].Write(vector);
                    else encryptor.IV = new byte[16].Write(0, vector, 0, 16);
                }
                if (type == CryptographyType.Encrypt)
                {
                    using (var memory = new MemoryStream())
                    {
                        using (var encoder = new CryptoStream(memory, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        //using (var encoder = new CryptoStream(memory, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            encoder.Write(data, 0, data.Length);
                            encoder.FlushFinalBlock();
                        }
                        return memory.ToArray();
                    }
                }
                else
                {
                    using (var memory = new MemoryStream(data))
                    {
                        try
                        {
                            using (var encoder = new CryptoStream(memory, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                using (var destMemory = new MemoryStream())
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
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7) => this.Encode(data, slatKey, vector, AlgorithmType, type, cipherMode, paddingMode);
    }
}