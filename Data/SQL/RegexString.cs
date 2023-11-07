using System.Collections.Generic;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-22 09:32:07                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 正则
    /// </summary>
    public static class RegexString
    {
        /// <summary>
        /// 匹配输入值正则 如参数 (a,b,c)=>{}
        /// </summary>
        public const string MatchInputTag = @"(^|\(|\s+|,|\+|-|\*|/)";
        /// <summary>
        /// 标识符
        /// </summary>
        public static Dictionary<char, char> Mark = new Dictionary<char, char>
        {
            {'{','}' },
            {'[',']' },
            {'(',')'},
            {'<','>' }
        };
    }
}