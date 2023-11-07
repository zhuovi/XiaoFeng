using System;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-12-22 15:53:11			    *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 加密通用接口
    /// </summary>
    public interface ICryptography
    {
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="vectorKey">偏移量</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns>加密后的字符串</returns>
        String Encrypt(String data, String slatKey = "", String vectorKey = "", OutputMode outputMode = OutputMode.Base64);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns></returns>
        String Encrypt(String data, String slatKey, OutputMode outputMode);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns></returns>
        String Encrypt(String data, OutputMode outputMode);
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="vectorKey">偏移量</param>
        /// <param name="outputMode">输入模式</param>
        /// <returns>解密后的字符串</returns>
        String Decrypt(String data, String slatKey = "", String vectorKey = "", OutputMode outputMode = OutputMode.Base64);
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="outputMode">输入模式</param>
        /// <returns>解密后的字符串</returns>
        String Decrypt(String data, String slatKey, OutputMode outputMode);
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="outputMode">输入模式</param>
        /// <returns>解密后的字符串</returns>
        String Decrypt(String data, OutputMode outputMode);
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文字节</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="vectorKey">偏移量</param>
        /// <returns>加密后的字节</returns>
        byte[] Encrypt(byte[] data, String slatKey = "", String vectorKey = "");
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="data">密文字节</param>
        /// <param name="slatKey">密钥</param>
        /// <param name="vectorKey">偏移量</param>
        /// <returns>解密后的字节</returns>
        byte[] Decrypt(byte[] data, String slatKey = "", String vectorKey = "");
    }
}