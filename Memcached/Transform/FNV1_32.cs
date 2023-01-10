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
*  Create Time : 2023-01-07 14:26:06                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Transform
{
    /// <summary>
    /// FNV1_32 
    /// </summary>
    public class FNV1_32: HashAlgorithm,IMemcachedTransform
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FNV1_32()
        {
            HashSizeValue = 32;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 首要的
        /// </summary>
        protected const uint Prime = 16777619;
        /// <summary>
        /// 初始化
        /// </summary>
        protected const uint Init = 2166136261;
        /// <summary>
        /// hash
        /// </summary>
        protected uint CurrentHashValue;
        #endregion

        #region 方法
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            CurrentHashValue = Init;
        }
        /// <summary>
        /// 计算Hash
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="ibStart">开始位</param>
        /// <param name="cbSize">大小</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int length = ibStart + cbSize;
            for (int i = ibStart; i < length; i++)
            {
                CurrentHashValue = (CurrentHashValue * Prime) ^ array[i];
            }
        }
        /// <summary>
        /// 获取hash
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(CurrentHashValue);
        }
        #endregion
    }
}