using System;
using System.Security.Cryptography;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 14:46:21                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Transform
{
    /// <summary>
    /// FNV1_64
    /// </summary>
    public class FNV1_64 : HashAlgorithm, IMemcachedTransform
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FNV1_64()
        {
            base.HashSizeValue = 64;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 初始值
        /// </summary>
        protected const ulong Init = 0xcbf29ce484222325L;
        /// <summary>
        /// 主要的
        /// </summary>
        protected const ulong Prime = 0x100000001b3L;
        /// <summary>
        /// Hash
        /// </summary>
        protected ulong CurrentHashValue;
        #endregion

        #region 方法
        /// <summary>
		/// 初始化
		/// </summary>
		public override void Initialize()
        {
            this.CurrentHashValue = Init;
        }
        /// <summary>计算Hash</summary>
        /// <param name="array">数组</param>
        /// <param name="ibStart">开始位置</param>
        /// <param name="cbSize">大小</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int end = ibStart + cbSize;

            for (int i = ibStart; i < end; i++)
            {
                this.CurrentHashValue *= Prime;
                this.CurrentHashValue ^= array[i];
            }
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <returns>Hash Code</returns>
        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(this.CurrentHashValue);
        }
        #endregion
    }
}