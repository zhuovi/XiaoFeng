using System;
using System.Linq;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 验证银行卡
    /// </summary>
    public static class BankInfo
    {
        /// <summary>
        /// 验证银行卡号是否正确
        /// </summary>
        /// <param name="no">银行卡号</param>
        /// <returns></returns>
        public static bool CheckBankCardNO(string no)
        {
            //第一步：从右边第1个数字开始每隔一位乘以2；
            //第二步： 把在第一步中获得的乘积的各位数字相加，然后再与原号码中未乘2的各位数字相加；
            //第三步：对于第二步求和值中个位数求10的补数，如果个位数为0则该校验码为0。
            if (!no.IsMatch(@"^\d{7,19}$")) return false;
            var Temp = no;
            var NewTemp = Temp.ToCharArray().Reverse().ToArray();
            var sum = 0d;
            for (int i = 1, len = NewTemp.Length; i < len; i++)
            {
                var temp1 = NewTemp[i].ToCast<short>();
                if ((i + 1) % 2 == 0)
                    temp1 *= 2;
                sum += Math.Floor(temp1 / 10d) + (temp1 % 10);
            }
            var checkCode = 10 - sum % 10;
            return NewTemp[0].ToString() == checkCode.ToString("0");
        }
    }
}