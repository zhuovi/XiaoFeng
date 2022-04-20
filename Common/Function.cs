using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Net;
namespace XiaoFeng
{
    /// <summary>
    /// 功能操作类
    /// </summary>
    public class Function
    {
        #region 加密函数
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="s">加密字符串</param>
        /// <returns></returns>
        public static string Encrypt(object s)
        {
            return Encrypt(s.ToString());
        }
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="s">加密字符串</param>
        /// <param name="Key">密钥</param>
        /// <param name="IV">向量</param>
        /// <returns></returns>
        public static string Encrypt(string s, string Key = "www.zhuovi.com", string IV = "www.zhuovi.com")
        {
            Key = Key.IsNullOrEmpty() ? "www.zhuovi.com" : Key;
            IV = IV.IsNullOrEmpty() ? Key : IV;
            using (var des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray;
                inputByteArray = Encoding.Default.GetBytes(s);
                des.Key = Encoding.ASCII.GetBytes(Key.MD5().Substring(0, 8));
                des.IV = Encoding.ASCII.GetBytes(IV.MD5().Substring(0, 8));
                MemoryStream ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                }
                StringBuilder ret = new StringBuilder();
                ms.ToArray().Each(b =>
                {
                    ret.AppendFormat("{0:X2}", b);
                });
                return ret.ToString();
            }
        }
        #endregion

        #region 解密函数
        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="s">解密字符串</param>
        /// <param name="Key">密钥</param>
        /// <param name="IV">向量</param>
        /// <returns></returns>
        public static string Decrypt(string s, string Key = "www.zhuovi.com", string IV = "www.zhuovi.com")
        {
            try
            {
                Key = Key.IsNullOrEmpty() ? "www.zhuovi.com" : Key;
                IV = IV.IsNullOrEmpty() ? Key : IV;
                using (var des = new DESCryptoServiceProvider())
                {
                    int len;
                    len = s.Length / 2;
                    byte[] inputByteArray = new byte[len];
                    int x, i;
                    for (x = 0; x < len; x++)
                    {
                        i = Convert.ToInt32(s.Substring(x * 2, 2), 16);
                        inputByteArray[x] = (byte)i;
                    }
                    des.Key = Encoding.ASCII.GetBytes(Key.MD5().Substring(0, 8));
                    des.IV = Encoding.ASCII.GetBytes(IV.MD5().Substring(0, 8));
                    MemoryStream ms = new MemoryStream();
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.Default.GetString(ms.ToArray());
                }
            }
            catch
            {
                return s;
            }
        }
        #endregion

        #region 加密函数[对应javascript里面的escape
        /// <summary>
        /// 加密函数[对应javascript里面的escape]
        /// </summary>
        /// <param name="s">要加密的字符串</param>
        /// <returns>返回加过密的字符串</returns>
        public static string Escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ba = Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));
                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion        

    }
}