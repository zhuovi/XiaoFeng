using System.ComponentModel;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-11-17 15:32:34                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    #region 输出编码
    /// <summary>
    /// 输出编码
    /// </summary>
    public enum OutputMode
    {
        /// <summary>
        /// Base64编码
        /// </summary>
        [Description("Base64编码")]
        Base64 = 0,
        /// <summary>
        /// Hex编码
        /// </summary>
        [Description("Hex编码")]
        Hex = 1
    }
    #endregion
}