using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using XiaoFeng.IO;
using System.IO;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-05 15:43:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// RSA与PEM互转类
    /// </summary>
    public class RSAPEM
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RSAPEM()
        {

        }
        /// <summary>
        /// 赋值算法标准参数
        /// </summary>
        /// <param name="parameters">算法标准参数</param>
        public RSAPEM(RSAParameters parameters)
        {
            this.RSAParameters = parameters;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 算法标准参数
        /// </summary>
        public RSAParameters RSAParameters { get; set; }
        /// <summary>
        /// 公钥PEM
        /// </summary>
        public StringBuilder PublicKey { get; set; }
        /// <summary>
        /// 私钥PEM
        /// </summary>
        public StringBuilder PrivateKey { get; set; }
        /// <summary>
        /// oid
        /// </summary>
        private readonly byte[] SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        /// <summary>
        /// 版本
        /// </summary>
        private readonly byte[] Ver = new byte[] { 0x02, 0x01, 0x00 };
        #endregion

        #region 方法
        /// <summary>
        /// 转PEM
        /// </summary>
        /// <param name="pKCSType">PKCS类型</param>
        public void ToPEM(PKCSType pKCSType =  PKCSType.PKCS1)
        {
            var ms = new MemoryStream();
            //写入一个长度字节码
            Action<int> writeLenByte = (len) =>
            {
                if (len < 0x80)
                {
                    ms.WriteByte((byte)len);
                }
                else if (len <= 0xff)
                {
                    ms.WriteByte(0x81);
                    ms.WriteByte((byte)len);
                }
                else
                {
                    ms.WriteByte(0x82);
                    ms.WriteByte((byte)(len >> 8 & 0xff));
                    ms.WriteByte((byte)(len & 0xff));
                }
            };
            //写入一块数据
            Action<byte[]> writeBlock = (byts) =>
            {
                var addZero = (byts[0] >> 4) >= 0x8;
                ms.WriteByte(0x02);
                var len = byts.Length + (addZero ? 1 : 0);
                writeLenByte(len);

                if (addZero)
                {
                    ms.WriteByte(0x00);
                }
                ms.Write(byts, 0, byts.Length);
            };
            //根据后续内容长度写入长度数据
            Func<int, byte[], byte[]> writeLen = (index, byts) =>
            {
                var len = byts.Length - index;

                ms.SetLength(0);
                ms.Write(byts, 0, index);
                writeLenByte(len);
                ms.Write(byts, index, len);

                return ms.ToArray();
            };
            Action<MemoryStream, byte[]> writeAll = (stream, byts) =>
            {
                stream.Write(byts, 0, byts.Length);
            };
            Func<string, int, string> TextBreak = (text, line) =>
            {
                var idx = 0;
                var len = text.Length;
                var str = new StringBuilder();
                while (idx < len)
                {
                    if (idx > 0)
                    {
                        str.Append('\n');
                    }
                    if (idx + line >= len)
                    {
                        str.Append(text.Substring(idx));
                    }
                    else
                    {
                        str.Append(text.Substring(idx, line));
                    }
                    idx += line;
                }
                return str.ToString();
            };
            /****生成公钥****/

            //写入总字节数，不含本段长度，额外需要24字节的头，后续计算好填入
            ms.WriteByte(0x30);
            var index1 = (int)ms.Length;

            //PKCS8 多一段数据
            int index2 = -1, index3 = -1;
            if (pKCSType == PKCSType.PCKS8)
            {
                //固定内容
                // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                writeAll(ms, SeqOID);

                //从0x00开始的后续长度
                ms.WriteByte(0x03);
                index2 = (int)ms.Length;
                ms.WriteByte(0x00);

                //后续内容长度
                ms.WriteByte(0x30);
                index3 = (int)ms.Length;
            }

            //写入Modulus
            writeBlock(RSAParameters.Modulus);

            //写入Exponent
            writeBlock(RSAParameters.Exponent);


            //计算空缺的长度
            var bytes = ms.ToArray();

            if (index2 != -1)
            {
                bytes = writeLen(index3, bytes);
                bytes = writeLen(index2, bytes);
            }
            bytes = writeLen(index1, bytes);


            var flag = " PUBLIC KEY";
            if (pKCSType != PKCSType.PCKS8)
            {
                flag = " RSA" + flag;
            }
            this.PublicKey = new StringBuilder("-----BEGIN" + flag + "-----\n" + TextBreak(Convert.ToBase64String(bytes), 64) + "\n-----END" + flag + "-----");
            if (RSAParameters.D != null)
            {
                /****生成私钥****/

                //写入总字节数，后续写入
                ms.WriteByte(0x30);
                index1 = (int)ms.Length;

                //写入版本号
                writeAll(ms, Ver);

                //PKCS8 多一段数据
                index2 = -1; index3 = -1;
                if (pKCSType == PKCSType.PCKS8)
                {
                    //固定内容
                    writeAll(ms, SeqOID);

                    //后续内容长度
                    ms.WriteByte(0x04);
                    index2 = (int)ms.Length;

                    //后续内容长度
                    ms.WriteByte(0x30);
                    index3 = (int)ms.Length;

                    //写入版本号
                    writeAll(ms, Ver);
                }

                //写入数据
                writeBlock(RSAParameters.Modulus);
                writeBlock(RSAParameters.Exponent);
                writeBlock(RSAParameters.D);
                writeBlock(RSAParameters.P);
                writeBlock(RSAParameters.Q);
                writeBlock(RSAParameters.DP);
                writeBlock(RSAParameters.DQ);
                writeBlock(RSAParameters.InverseQ);


                //计算空缺的长度
                bytes = ms.ToArray();

                if (index2 != -1)
                {
                    bytes = writeLen(index3, bytes);
                    bytes = writeLen(index2, bytes);
                }
                bytes = writeLen(index1, bytes);


                flag = " PRIVATE KEY";
                if (pKCSType != PKCSType.PCKS8)
                {
                    flag = " RSA" + flag;
                }
                this.PrivateKey = new StringBuilder("-----BEGIN" + flag + "-----\n" + TextBreak(Convert.ToBase64String(bytes), 64) + "\n-----END" + flag + "-----");
            }
        }
        #endregion
    }
}