using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-31 09:00:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// CRCAlgorithm 类说明
    /// </summary>
    public class CRCAlgorithm
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CRCAlgorithm()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// CRC16算法
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static long GetCRC16(string _)
        {
            if (_.IsNullOrEmpty()) return 0;
            var data = _.GetBytes(Encoding.ASCII);
            if (data.Length == 0) return 0;
            ushort crc = 0xFFFF;
            for (var i = 0; i < data.Length; i++)
            {
                crc = (ushort)(crc ^ (data[i]));
                for (var j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                }
            }
            var high = (byte)((crc & 0xFF00) >> 8);
            var low = (byte)(crc & 0x00FF);
            return BitConverter.ToInt64(new byte[] { high, low }, 0);
        }
        
        #endregion
    }
}