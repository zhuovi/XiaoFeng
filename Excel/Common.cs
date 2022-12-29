using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Excel.Model;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 22:24:26                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 共用类
    /// </summary>
    public static class Common
    {
        #region 属性
        /// <summary>
        /// workbook路径
        /// </summary>
        public const string XL_WORKBOOK_XML = "xl/workbook.xml";
        /// <summary>
        /// workbook Rels路径
        /// </summary>
        public const string XL_RELS_WORKBOOK_XML_RELS = "xl/_rels/workbook.xml.rels";
        /// <summary>
        /// sharedstrings路径
        /// </summary>
        public const string XL_SHAREDSTRINGS_XML = "xl/sharedStrings.xml";
        /// <summary>
        /// styles路径
        /// </summary>
        public const string XL_STYLES_XML = "xl/styles.xml";
        #endregion

        #region 方法
        /// <summary>
        /// 获取列索引
        /// </summary>
        /// <param name="name">列名称</param>
        /// <returns></returns>
        public static int GetColumnIndex(string name)
        {
            int index = 0;
            int pow = 1;
            for (var i = name.Length - 1; i >= 0; i--)
            {
                index += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return index;
        }
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string GetColumnName(int value)
        {
            int index = value;
            int pow = 1;
            var _ = new List<string>();
            while (index > 0)
            {
                pow *= 26;
                var mod = index % pow;
                if (mod % 26 == 0) { index = 0; mod = 1; }
                var a = (char)(mod + 'A' - 1);
                _.Add(a.ToString());
                index -= mod;
            }
            _.Reverse();
            return _.Join();
        }
        #endregion
    }
}