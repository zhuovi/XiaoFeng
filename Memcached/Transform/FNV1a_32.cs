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
*  Create Time : 2023-01-07 14:30:12                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Transform
{
    /// <summary>
    /// FNV1a_32
    /// </summary>
    public class FNV1a_32 : FNV1_32
    {
        #region 方法
        /// <summary>
        /// 计算Hash
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="ibStart">开始位置</param>
        /// <param name="cbSize">大小</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int length = ibStart + cbSize;
            for (int i = ibStart; i < length; i++)
            {
                CurrentHashValue = (CurrentHashValue ^ array[i]) * Prime;
            }
        }
        #endregion
    }
}