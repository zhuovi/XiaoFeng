using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-06 10:18:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 身份证验证类
    /// </summary>
    public class CardInfo: Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CardInfo() { }
        /// <summary>
        /// 身份证号
        /// </summary>
        /// <param name="cardNumber">身份证号</param>
        public CardInfo(string cardNumber)
        {
            CardNumber = cardNumber;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public Dictionary<string, string> Provinces { get; set; } = new Dictionary<string, string>
        {
            {"11","北京" },
            {"12","天津" },
            {"13","河北" },
            {"14","山西" },
            {"15","内蒙古" },
            {"21","辽宁" },
            {"22","吉林" },
            {"23","黑龙江" },
            {"31","上海" },
            {"32","江苏" },
            {"33","浙江" },
            {"34","安徽" },
            {"35","福建" },
            {"36","江西" },
            {"37","山东" },
            {"41","河南" },
            {"42","湖北" },
            {"43","湖南" },
            {"44","广东" },
            {"45","广西" },
            {"46","海南" },
            {"50","重庆" },
            {"51","四川" },
            {"52","贵州" },
            {"53","云南" },
            {"54","西藏" },
            {"61","陕西" },
            {"62","甘肃" },
            {"63","青海" },
            {"64","宁夏" },
            {"65","新疆" },
            {"71","台湾" },
            {"81","香港" },
            {"82","澳门" },
            {"91","国外" }
        };
        #endregion

        #region 方法
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="cardNumber">身份证号</param>
        /// <returns></returns>
        public Boolean Valid(string cardNumber= "")
        {
            if (cardNumber.IsNullOrEmpty())
                cardNumber = this.CardNumber;
            var length = cardNumber.Length;
            if (length != 18 && length != 15) return false;
            /*
             * 验证省份
             */
            if (!this.Provinces.TryGetValue(cardNumber.Substring(0, 2), out var _)) return false;
            /*
             * 验证出生年月日
             */
            int year = length == 15?(cardNumber.Substring(6, 2).ToInt32() + 1900): cardNumber.Substring(6, 4).ToInt32();
            string pattern;
            if (year % 4 == 0 || (year % 4 == 0 && year % 100 == 0))
                pattern = @"^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$";
            else
                pattern = @"^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2] [0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$";
            if (!cardNumber.IsMatch(pattern)) return false;
            if (length == 18)
            {
                var M = CreateCode(cardNumber);
                return M.EqualsIgnoreCase(cardNumber[17].ToString());                
            }
            return true;
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <param name="sex">性别 0 男 1女</param>
        /// <returns></returns>
        private string CreateRandom(int sex)
        {
            sex = sex == 1 ? 1 : 0;
            int[][] vals = { new int[] { 1, 3, 5, 7, 9 }, new int[] { 0, 2, 4, 6, 8 } };
            return RandomHelper.GetRandomInt(10, 99).ToString() + vals[sex][RandomHelper.GetRandomInt(0, 4)];
        }
        /// <summary>
        /// 创建检验码
        /// </summary>
        /// <param name="cardNumber">身份证号</param>
        /// <returns></returns>
        private string CreateCode(string cardNumber)
        {
            if (cardNumber.Length < 17) return "";
            cardNumber = cardNumber.Substring(0, 17);
            int[] factor = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            var parity = "10X98765432";
            var total = 0;
            for(var i = 0; i < cardNumber.Length; i++)
                total += cardNumber.Substring(i,1).ToCast<int>() * factor[i];
            return parity[total % parity.Length].ToString();
        }
        /// <summary>
        /// 生成身份证号
        /// </summary>
        /// <param name="areaCode">区码</param>
        /// <param name="birthday">出生日期</param>
        /// <param name="sex">性别 0 男 1女</param>
        /// <param name="count">数量 最小1个 最多一次 100个</param>
        /// <returns></returns>
        public List<string> Create(string areaCode, DateTime birthday, int sex, int count = 5)
        {
            var list = new List<string>();
            if (count <= 0) count = 1;
            else if (count > 100) count = 100;
            if (birthday.Year == 0) birthday = "19851006".ToDateTime();
            var prefix = areaCode + birthday.ToString("yyyyMMdd");
            for (var i = 0; i < count; i++)
            {
                var rand = this.CreateRandom(sex);
                var M = this.CreateCode(prefix + rand);
                var number = prefix + rand + M;
                if (list.Contains(number))
                    i--;
                else
                    list.Add(number);
            }
            return list;
        }
        #endregion
    }
    /// <summary>
    /// 身份证数据
    /// </summary>
    public class CardData
    {
        /// <summary>
        /// 身份证号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 省编码
        /// </summary>
        public string ProvinceCode { get; set; }
        /// <summary>
        /// 市编码
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// 县区编码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 性别 0 男 1女
        /// </summary>
        public int Sex { get; set; }
    }
}