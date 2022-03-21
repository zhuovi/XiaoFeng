using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng
{
    /// <summary>
    /// 通过身份证号提取身份证信息
    /// </summary>
    public class IdentityCard : Disposable
    {
        #region 构造器
        /// <summary>
        /// 身份证号
        /// </summary>
        /// <param name="cardNo"></param>
        public IdentityCard(string cardNo)
        {
            this.CardNO = cardNo;
        }
        /// <summary>
        /// 无参构造器
        /// </summary>
        public IdentityCard() { }
        #endregion

        #region 属性
        /// <summary>
        /// 静态实例
        /// </summary>
        public static IdentityCard Create { get { return new IdentityCard(); } }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNO { get; set; }
        /// <summary>
        /// 错误提示信息
        /// </summary>
        private string[] Errors = new string[] { "验证通过.", "请输入身份证号", "身份证号码位数正确.", "身份证号码出生日期超出范围或含有非法字符.", "身份证号码校验错误.", "身份证地区非法." };
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }
        #endregion

        #region 方法

        #region 验证是否是身份证号 success则验证通过
        /// <summary>
        /// 验证是否是身份证号 success则验证通过
        /// </summary>
        /// <returns></returns>
        public string IsCardNumber(string cardNo = "")
        {
            if (cardNo.IsNotNullOrEmpty()) this.CardNO = cardNo;
            if (this.CardNO.IsNullOrEmpty()) { return this.Message = this.Errors[1]; }
            if (this.CardNO.Length != 15 && this.CardNO.Length != 18)
                return this.Message = this.Errors[2];
            if (this.CardNO.Length == 15)
            {
                string ereg;
                if ((this.CardNO.Substring(6, 2).ToCast<int>() + 1900) % 4 == 0 || ((this.CardNO.Substring(6, 2).ToCast<int>() + 1900) % 100 == 0 && (this.CardNO.Substring(6, 2).ToCast<int>() + 1900) % 4 == 0))
                    ereg = @"^[1-9][0-9]{ 5}[0-9]{2}
    ((01|03|05|07|08|10|12)(0[1-9]|[1-2] [0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2] [0-9]|30)|02(0[1-9]|[1-2] [0-9]))[0-9]{3}$";//测试出生日期的合法性
                else
                    ereg = @"^[1-9] [0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2] [0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2] [0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}$";//测试出生日期的合法性
                return this.Message = this.CardNO.IsMatch(ereg) ? "success" : this.Errors[3];

            }
            else if (this.CardNO.Length == 18)
            {
                //18位身份号码检测
                //出生日期的合法性检查 
                //闰年月日:((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))
                //平年月日:((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))
                string ereg;
                if (this.CardNO.Substring(6, 4).ToCast<int>() % 4 == 0 || (this.CardNO.Substring(6, 4).ToCast<int>() % 100 == 0 && this.CardNO.Substring(6, 4).ToCast<int>() % 4 == 0))
                    ereg = @"^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$";//闰年出生日期的合法性正则表达式
                else
                    ereg = @"^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2] [0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2] [0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$";//平年出生日期的合法性正则表达式
                if (this.CardNO.IsMatch(ereg))
                {
                    //测试出生日期的合法性
                    //计算校验位
                    var M = this.CreateCehckCode(this.CardNO);
                    //检测ID的校验位
                    return this.Message = M == this.CardNO[17].ToString() ? "success" : this.Errors[4];
                }
                else
                    return this.Message = Errors[3];
            }
            else
                return this.Message = Errors[2];
        }
        #endregion

        #region 生成校验码
        /// <summary>
        /// 生成校验码
        /// </summary>
        /// <param name="CardCode">身份证号</param>
        /// <returns></returns>
        private string CreateCehckCode(string CardCode)
        {
            var M = (CardCode.Substring(0,1).ToCast<int>() * 1 + CardCode.Substring(10, 1).ToCast<int>() * 1) * 7 +
            (CardCode.Substring(1, 1).ToCast<int>() * 1 + CardCode.Substring(11, 1).ToCast<int>() * 1) * 9 +
            (CardCode.Substring(2, 1).ToCast<int>() * 1 + CardCode.Substring(12, 1).ToCast<int>() * 1) * 10 +
            (CardCode.Substring(3, 1).ToCast<int>() * 1 + CardCode.Substring(13, 1).ToCast<int>() * 1) * 5 +
            (CardCode.Substring(4, 1).ToCast<int>() * 1 + CardCode.Substring(14, 1).ToCast<int>() * 1) * 8 +
            (CardCode.Substring(5, 1).ToCast<int>() * 1 + CardCode.Substring(15, 1).ToCast<int>() * 1) * 4 +
            (CardCode.Substring(6, 1).ToCast<int>() * 1 + CardCode.Substring(16, 1).ToCast<int>() * 1) * 2 +
            CardCode.Substring(7, 1).ToCast<int>() * 1 +
            CardCode.Substring(8, 1).ToCast<int>() * 6 +
            CardCode.Substring(9, 1).ToCast<int>() * 3;
            var C = M % 11;
            return "10X98765432"[C].ToString();
        }
        #endregion

        #region 获取身份证信息
        /// <summary>
        /// 获取身份证信息
        /// </summary>
        /// <param name="cardNo">身份证号</param>
        /// <returns></returns>
        public IdentityCardData GetCardData(string cardNo = "")
        {
            var data = new IdentityCardData();
            if (cardNo.IsNotNullOrEmpty()) this.CardNO = cardNo;
            if (IsCardNumber() == "success")
            {
                data.CardNO = this.CardNO;
                data.CountyAreaID = this.CardNO.Substring(0, 6);
                if (this.CardNO.Length == 15)
                {
                    data.Year = 1900 + this.CardNO.Substring(6, 2).ToCast<int>();
                    data.Month = this.CardNO.Substring(8, 2).ToCast<int>();
                    data.Day = this.CardNO.Substring(10, 2).ToCast<int>();
                    data.Sex = this.CardNO.Substring(12,1).ToCast<int>() % 2 == 0 ? "女" : "男";
                    data.CheckCode = "";
                }
                else if (this.CardNO.Length == 18)
                {
                    data.Year = this.CardNO.Substring(6, 4).ToCast<int>();
                    data.Month = this.CardNO.Substring(10, 2).ToCast<int>();
                    data.Day = this.CardNO.Substring(12, 2).ToCast<int>();
                    data.Sex = this.CardNO.Substring(16,1).ToCast<int>() % 2 == 0 ? "女" : "男";
                    data.CheckCode = this.CardNO.Substring(17,1);
                }
            }
            else
                return null;
            return data;
        }
        #endregion

        #region 生成身份证号
        /// <summary>
        /// 生成身份证号
        /// </summary>
        /// <param name="data">输入数据</param>
        /// <returns></returns>
        public string CreateIdentityCardNumber(IdentityCardData data)
        {
            if (data == null || !data.CountyAreaID.IsMatch(@"^\d{6}$") || data.Year <= 0 || data.Month <= 0 || data.Day <= 0 || !data.Sex.IsMatch(@"^(男|女)$")) return "";
            var cardNo = data.CountyAreaID + data.Year + data.Month.ToString().PadLeft(2, '0') + data.Day.ToString().PadLeft(2, '0');
            var ran = RandomHelper.GetRandomInt(100, 998);
            if (ran % 2 == 0)
            {
                if (data.Sex == "男") ran++;
            }
            else
            {
                if (data.Sex == "女") ran++;
            }
            cardNo += ran;
            var M = CreateCehckCode(cardNo);
            return data.CardNO = cardNo + M;
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 身份证数据
    /// </summary>
    public class IdentityCardData
    {
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNO { get; set; }
        /// <summary>
        /// 省份ID
        /// </summary>
        public string ProvinceID
        {
            get
            {
                if (CountyAreaID.IsMatch(@"^\d{6}$"))
                {
                    return CountyAreaID.Substring(0, 2).PadRight(6, '0');
                }
                return "";
            }
        }
        /// <summary>
        /// 市ID
        /// </summary>
        public string CityID
        {
            get
            {
                if (CountyAreaID.IsMatch(@"^\d{6}$"))
                {
                    return CountyAreaID.Substring(0, 4).PadRight(6, '0');
                }
                return "";
            }
        }
        /// <summary>
        /// 县区ID
        /// </summary>
        public string CountyAreaID { get; set; } = "410922";
        /// <summary>
        /// 出生年份
        /// </summary>
        public int Year { get; set; } = 1900;
        /// <summary>
        /// 出生月份
        /// </summary>
        public int Month { get; set; } = 1;
        /// <summary>
        /// 出生日
        /// </summary>
        public int Day { get; set; } = 1;
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Date { get { return new DateTime(this.Year, this.Month, this.Day); } }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; } = "男";
        /// <summary>
        /// 校验码
        /// </summary>
        public string CheckCode { get; set; }
    }
}