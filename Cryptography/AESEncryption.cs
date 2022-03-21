using System;
using System.Collections.Generic;
using System.IO;
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
 *  Create Time : 2020-12-22 下午 11:31:16          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// AES加密算法
    /// Version : 1.0.0
    /// CrateTime : 2020-12-22 下午 11:31:16
    /// Author : Jacky
    /// 更新说明
    /// </summary>
    public class AESEncryption : BaseSymmetricAlgorithm
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public AESEncryption() : base(SymmetricAlgorithmType.AES)
        { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}