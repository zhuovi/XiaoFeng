using System.ComponentModel;
/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-04-18 18:49:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 国产加密类型
    /// </summary>
    public enum SMType
    {
        /// <summary>
        /// SM1
        /// </summary>
        [Description("SM1")]
        ONE = 0,
        /// <summary>
        /// SM2
        /// </summary>
        [Description("SM2")]
        TWO = 1,
        /// <summary>
        /// SM3
        /// </summary>
        [Description("SM3")]
        THREE = 0,
        /// <summary>
        /// SM4
        /// </summary>
        [Description("SM4")]
        FOUR = 1
    }
}