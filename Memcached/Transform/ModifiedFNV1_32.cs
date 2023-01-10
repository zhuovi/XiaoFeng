using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 14:37:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Transform
{
    /// <summary>
    /// ModifiedFNV1_32
    /// </summary>
    public class ModifiedFNV1_32 : FNV1a_32
    {
        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal()
        {
            this.CurrentHashValue += this.CurrentHashValue << 13;
            this.CurrentHashValue ^= this.CurrentHashValue >> 7;
            this.CurrentHashValue += this.CurrentHashValue << 3;
            this.CurrentHashValue ^= this.CurrentHashValue >> 17;
            this.CurrentHashValue += this.CurrentHashValue << 5;

            return base.HashFinal();
        }
        #endregion
    }
}