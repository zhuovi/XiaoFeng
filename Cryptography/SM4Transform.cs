using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-04-18 18:07:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// SM4加密转换器
    /// </summary>
    public class SM4Transform: ICryptoTransform
    {
        #region 构造器
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="iv">iv</param>
        /// <param name="encryptMode">加密还是解密 true是加密 false是解密</param>
        public SM4Transform(byte[] key, byte[] iv, bool encryptMode)
        {
            if (key == null || key.Length != 16)
            {
                throw new ArgumentException("KEY must be a 16-byte array.");
            }

            if (iv != null && iv.Length != 16)
            {
                throw new ArgumentException("IV must be a 16-byte array.");
            }
            this.encryptMode = encryptMode;
            GenerateRoundKeys(key);
            if (iv != null)
            {
                Array.Copy(iv, buffer, BLOCK_SIZE);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 输入数据块的大小（以字节为单位）。
        /// </summary>
        private const int BLOCK_SIZE = 16;
        /// <summary>
        /// BOX
        /// </summary>
        private readonly byte[] S_BOX = new byte[]
        {
            0xd6, 0x90, 0xe9, 0xfe, 0xcc, 0xe1, 0x3d, 0xb7,
            0x16, 0xb6, 0x14, 0xc2, 0x28, 0xfb, 0x2c, 0x05,
            0x2b, 0x67, 0x9a, 0x76, 0x2a, 0xbe, 0x04, 0xc3,
            0xaa, 0x44, 0x13, 0x26, 0x49, 0x86, 0x06, 0x99,
            0x9c, 0x42, 0x50, 0xf4, 0x91, 0xef, 0x98, 0x7a,
            0x33, 0x54, 0x0b, 0x43, 0xed, 0xcf, 0xac, 0x62,
            0xe4, 0xb3, 0x1c, 0xa9, 0xc9, 0x08, 0xe8, 0x95,
            0x80, 0xdf, 0x94, 0xfa, 0x75, 0x8f, 0x3f, 0xa6,
            0x47, 0x07, 0xa7, 0xfc, 0xf3, 0x73, 0x17, 0xba,
            0x83, 0x59, 0x3c, 0x19, 0xe6, 0x85, 0x4f, 0xa8,
            0x68, 0x6b, 0x81, 0xb2, 0x71, 0x64, 0xda, 0x8b,
            0xf8, 0xeb, 0x0f, 0x4b, 0x70, 0x56, 0x9d, 0x35,
            0x1e, 0x24, 0x0e, 0x5e, 0x63, 0x58, 0xd1, 0xa2,
            0x25, 0x22, 0x7c, 0x3b, 0x01, 0x21, 0x78, 0x87
        };
        /// <summary>
        /// FK
        /// </summary>
        private readonly uint[] FK = new uint[] { 0xa3b1bac6, 0x56aa3350, 0x677d9197, 0xb27022dc };
        /// <summary>
        /// CK
        /// </summary>
        private readonly uint[] CK = new uint[]
        {
            0x00070e15, 0x1c232a31, 0x383f464d, 0x545b6269,
            0x70777e85, 0x8c939aa1, 0xa8afb6bd, 0xc4cbd2d9,
            0xe0e7eef5, 0xfc030a11, 0x181f262d, 0x343b4249,
            0x50575e65, 0x6c737a81, 0x888f969d, 0xa4abb2b9,
            0xc0c7ced5, 0xdce3eaf1, 0xf8ff060d, 0x141b2229,
            0x30373e45, 0x4c535a61, 0x686f767d, 0x848b9299,
            0xa0a7aeb5, 0xbcc3cad1, 0xd8dfe6ed, 0xf4fb020f,
            0x10171e25, 0x2c333a41, 0x484f565d, 0x646b7279
        };
        /// <summary>
        /// 加密KEY
        /// </summary>
        private readonly uint[] ENCRYPT_ROUND_KEYS = new uint[32];
        /// <summary>
        /// 解密KEY
        /// </summary>
        private readonly uint[] DECRYPT_ROUND_KEYS = new uint[32];
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly byte[] buffer = new byte[BLOCK_SIZE];
        /// <summary>
        /// X
        /// </summary>
        private readonly uint[] x = new uint[BLOCK_SIZE / 4];
        /// <summary>
        /// 加密还是解密
        /// </summary>
        private readonly Boolean encryptMode;
        #endregion

        #region 方法
        /// <summary>
        /// 如果重复使用当前转换，则为 true；否则为 false。
        /// </summary>
        public Boolean CanReuseTransform => false;
        /// <summary>
        /// 如果可以转换多个块，则为 true；否则，为 false。
        /// </summary>
        public Boolean CanTransformMultipleBlocks => true;
        /// <summary>
        /// 输入数据块的大小（以字节为单位）。
        /// </summary>
        public int InputBlockSize => BLOCK_SIZE;
        /// <summary>
        /// 输出数据块的大小（以字节为单位）。
        /// </summary>
        public int OutputBlockSize => BLOCK_SIZE;
        /// <summary>
        /// 转换输入字节数组的指定区域，并将所得到的转换复制到输出字节数组的指定区域。
        /// </summary>
        /// <param name="inputBuffer">要为其计算转换的输入。</param>
        /// <param name="inputOffset">输入字节数组中的偏移量，从该位置开始使用数据。</param>
        /// <param name="inputCount">输入字节数组中用作数据的字节数。</param>
        /// <param name="outputBuffer">将转换写入的输出。</param>
        /// <param name="outputOffset">输入字节数组中的偏移量，从该位置开始使用数据。</param>
        /// <returns>写入的字节数</returns>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (inputCount != BLOCK_SIZE)
            {
                throw new ArgumentException("Input count must be equal to block size.");
            }
            Array.Copy(inputBuffer, inputOffset, buffer, 0, BLOCK_SIZE);
            BufferToWords(buffer, x);
            if (encryptMode)
            {
                EncryptBlock(x, ENCRYPT_ROUND_KEYS);
            }
            else
            {
                DecryptBlock(x, DECRYPT_ROUND_KEYS);
            }

            WordsToBuffer(x, buffer);

            Array.Copy(buffer, 0, outputBuffer, outputOffset, BLOCK_SIZE);

            return BLOCK_SIZE;
        }
        /// <summary>
        /// 转换指定字节数组的指定区域。
        /// </summary>
        /// <param name="inputBuffer">要为其计算转换的输入。</param>
        /// <param name="inputOffset">字节数组中的偏移量，从该位置开始使用数据。</param>
        /// <param name="inputCount">字节数组中用作数据的字节数。</param>
        /// <returns>计算所得的转换。</returns>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount != 0 && inputCount != BLOCK_SIZE)
            {
                throw new ArgumentException("Input count must be equal to block size or zero.");
            }

            if (inputCount == BLOCK_SIZE)
            {
                TransformBlock(inputBuffer, inputOffset, inputCount, buffer, 0);
            }

            byte[] outputBuffer = new byte[inputCount];
            Array.Copy(buffer, outputBuffer, inputCount);

            Reset();

            return outputBuffer;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Reset();
        }
        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            Array.Clear(buffer, 0, buffer.Length);
            Array.Clear(x, 0, x.Length);
        }
        /// <summary>
        /// 生成随机key
        /// </summary>
        /// <param name="key">key</param>
        private void GenerateRoundKeys(byte[] key)
        {
            uint[] k = new uint[4];
            BufferToWords(key, k);

            k[0] ^= FK[0];
            k[1] ^= FK[1];
            k[2] ^= FK[2];
            k[3] ^= FK[3];

            for (int i = 0; i < 32; i++)
            {
                k[i % 4] ^= T(k[(i + 1) % 4] ^ k[(i + 2) % 4] ^ k[(i + 3) % 4] ^ CK[i]);
                ENCRYPT_ROUND_KEYS[i] = k[i % 4];
            }

            for (int i = 0; i < 32; i++)
            {
                DECRYPT_ROUND_KEYS[i] = ENCRYPT_ROUND_KEYS[31 - i];
            }
        }
        /// <summary>
        /// 加密块
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="roundKeys">key</param>
        private void EncryptBlock(uint[] x, uint[] roundKeys)
        {
            for (int i = 0; i < 32; i++)
            {
                x[0] ^= T(x[1] ^ x[2] ^ x[3] ^ roundKeys[i]);
                uint tmp = x[0];
                x[0] = x[1];
                x[1] = x[2];
                x[2] = x[3];
                x[3] = tmp;
            }
        }
        /// <summary>
        /// 解密块
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="roundKeys">key</param>
        private void DecryptBlock(uint[] x, uint[] roundKeys)
        {
            for (int i = 0; i < 32; i++)
            {
                x[0] ^= T(x[1] ^ x[2] ^ x[3] ^ roundKeys[i]);
                uint tmp = x[0];
                x[0] = x[3];
                x[3] = x[2];
                x[2] = x[1];
                x[1] = tmp;
            }
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="x">x</param>
        /// <returns></returns>
        private uint T(uint x)
        {
            byte[] b = new byte[]
            {
            (byte)(x >> 24 & 0xff),
            (byte)(x >> 16 & 0xff),
            (byte)(x >> 8 & 0xff),
            (byte)(x & 0xff)
            };

            for (int i = 0; i < 4; i++)
            {
                b[i] = S_BOX[b[i]];
            }

            uint y = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | (uint)b[3];

            return y ^ RotateLeft(y, 2) ^ RotateLeft(y, 10) ^ RotateLeft(y, 18) ^ RotateLeft(y, 24);
        }
        /// <summary>
        /// 缓存转换字节
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="x">x</param>
        private void BufferToWords(byte[] buffer, uint[] x)
        {
            for (int i = 0; i < BLOCK_SIZE / 4; i++)
            {
                x[i] = ((uint)buffer[i * 4] << 24) | ((uint)buffer[i * 4 + 1] << 16) | ((uint)buffer[i * 4 + 2] << 8) | (uint)buffer[i * 4 + 3];
            }
        }
        /// <summary>
        /// 字节转换缓存
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="buffer">缓存</param>
        private void WordsToBuffer(uint[] x, byte[] buffer)
        {
            for (int i = 0; i < BLOCK_SIZE / 4; i++)
            {
                buffer[i * 4] = (byte)(x[i] >> 24 & 0xff);
                buffer[i * 4 + 1] = (byte)(x[i] >> 16 & 0xff);
                buffer[i * 4 + 2] = (byte)(x[i] >> 8 & 0xff);
                buffer[i * 4 + 3] = (byte)(x[i] & 0xff);
            }
        }
        /// <summary>
        /// 向左移
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="n">移动位数</param>
        /// <returns></returns>
        private uint RotateLeft(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }
        #endregion
    }
}