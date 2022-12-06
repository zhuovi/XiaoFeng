using System;
using System.Collections.Generic;
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
 *  Create Time : 2020-12-24 上午 12:13:19          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// RC4加密算法
    /// Version : 1.0.0
    /// CrateTime : 2020-12-24 上午 12:13:19
    /// 更新说明
    /// </summary>
    public class RC4Encryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RC4Encryption()
        {
        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7) => null;

        ///<inheritdoc/>
        public override byte[] Encrypt(byte[] data, string slatKey = "", string vectorKey = "")
        {
            if (data == null || slatKey == null) return null;
            Byte[] output = new Byte[data.Length];
            Int64 i = 0;
            Int64 j = 0;
            Byte[] mBox = GetKey(this.GetBytes(slatKey), 256);
            for (Int64 offset = 0; offset < data.Length; offset++)
            {
                i = (i + 1) % mBox.Length;
                j = (j + mBox[i]) % mBox.Length;
                Byte temp = mBox[i];
                mBox[i] = mBox[j];
                mBox[j] = temp;
                Byte a = data[offset];
                Byte b = mBox[(mBox[i] + mBox[j]) % mBox.Length];
                output[offset] = (Byte)((Int32)a ^ (Int32)b);
            }
            return output;
        }
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.OutputString(this.Encrypt(this.GetBytes(data), slatKey, ""), outputMode);
        ///<inheritdoc/>
        public override byte[] Decrypt(byte[] data, string slatKey = "", string vectorKey = "") => this.Encrypt(data, slatKey, vectorKey);
        ///<inheritdoc/>
        public override string Decrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Decrypt(this.InputBytes(data, outputMode), slatKey, vectorKey).GetString(this.Encoding);

        #region 打乱密码
        /// <summary>
        /// 打乱密码
        /// </summary>
        /// <param name="pass">密码</param>
        /// <param name="kLen">密码箱长度</param>
        /// <returns>打乱后的密码</returns>
        private Byte[] GetKey(Byte[] pass, Int32 kLen)
        {
            Byte[] mBox = new Byte[kLen];

            for (Int64 i = 0; i < kLen; i++)
            {
                mBox[i] = (Byte)i;
            }
            Int64 j = 0;
            for (Int64 i = 0; i < kLen; i++)
            {
                j = (j + mBox[i] + pass[i % pass.Length]) % kLen;
                Byte temp = mBox[i];
                mBox[i] = mBox[j];
                mBox[j] = temp;
            }
            return mBox;
        }
        #endregion

        #endregion
    }
}