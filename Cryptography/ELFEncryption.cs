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
 *  Create Time : 2020-12-23 上午 12:55:29          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// ELF加密算法
    /// Version : 1.0.0
    /// CrateTime : 2020-12-23 上午 12:55:29
    /// 更新说明
    /// </summary>
    public class ELFEncryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ELFEncryption()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7)
        {
            if (data == null) return Array.Empty<byte>();
            var AES = new AESEncryption();
            if(type== CryptographyType.Encrypt)
            {
               var _data = RandomHelper.GetRandomString(5, RandomType.Letter | RandomType.Number) + AES.Encrypt(this.GetString(data) , this.GetString(slatKey)) + RandomHelper.GetRandomString(5, RandomType.Letter | RandomType.Number);
                for (var i = 0; i < 9; i++)
                    _data = _Encrypt(_data);
                return this.GetBytes(_data);
            }
            else
            {
                var _data = this.GetString(data);
                for (var i = 0; i < 9; i++)
                    _data = _Decrypt(_data);
                _data = AES.Decrypt(_data.RemovePattern(@"[a-z0-9]{5}$").RemovePattern(@"^[a-z0-9]{5}"),this.GetString(slatKey));
                return this.GetBytes(_data);
            }
        }
        #region 混肴字符串
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        private string _Encrypt(string data)
        {
            if (data.IsNullOrEmpty()) return string.Empty;
            string _data = "";
            /*大写变小写 小写变大写*/
            foreach (char c in data)
            {
                if (c >= 'a' && c <= 'z')
                    _data += c.ToString().ToUpper();
                else if (c >= 'A' && c <= 'Z')
                    _data += c.ToString().ToLower();
                else if (c >= '0' && c <= '9')
                    _data += c.ToString();
                else if (c == '/')
                    _data += "-";
                else if (c == '+')
                    _data += "_";
                else
                    _data += c;
            }
            var arr = _data.ToArray().Reverse().ToArray();
            var len = data.Length;
            for (var i = 0; i < len; i += 2)
            {
                if (i < len - 1)
                {
                    var c = arr[i];
                    arr[i] = arr[i + 1];
                    arr[i + 1] = c;
                }
            }
            var _arr = arr.Reverse().ToArray();
            for (var i = 0; i < len; i += 3)
            {
                if (i < len - 2)
                {
                    var c = _arr[i];
                    _arr[i] = _arr[i + 2];
                    _arr[i + 2] = c;
                }
            }
            var __arr = _arr.Reverse().ToArray();
            for (var i = 0; i < len; i += 7)
            {
                if (i < len - 6)
                {
                    var c = __arr[i];
                    __arr[i] = __arr[i + 6];
                    __arr[i + 6] = c;
                }
            }
            return new string(__arr.Reverse().ToArray());
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        private string _Decrypt(string data)
        {
            var _data = data.ToArray().Reverse().ToArray();
            var len = data.Length;
            for (var i = 0; i < len; i += 7)
            {
                if (i < len - 6)
                {
                    var c = _data[i];
                    _data[i] = _data[i + 6];
                    _data[i + 6] = c;
                }
            }
            var __arr = _data.Reverse().ToArray();
            for (var i = 0; i < len; i += 3)
            {
                if (i < len - 2)
                {
                    var c = __arr[i];
                    __arr[i] = __arr[i + 2];
                    __arr[i + 2] = c;
                }
            }
            var _arr = __arr.Reverse().ToArray();
            for (var i = 0; i < len; i += 2)
            {
                if (i < len - 1)
                {
                    var c = _arr[i];
                    _arr[i] = _arr[i + 1];
                    _arr[i + 1] = c;
                }
            }
            var arr = new string(_arr.Reverse().ToArray());
            data = "";
            foreach (char c in arr)
            {
                if (c >= 'a' && c <= 'z')
                    data += c.ToString().ToUpper();
                else if (c >= 'A' && c <= 'Z')
                    data += c.ToString().ToLower();
                else if (c >= '0' && c <= '9')
                    data += c.ToString();
                else if (c == '-')
                    data += "/";
                else if (c == '_')
                    data += "+";
                else
                    data += c;
            }
            return data;
        }
        #endregion
        #endregion
    }
}