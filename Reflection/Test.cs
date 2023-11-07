using System;
using System.Data;
using System.Reflection;
using System.Text;
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
    /// 检查C#中语法属性操作类
    /// </summary>
    public static class Test
    {
        #region 获取实例类中的所有属性及值
        /// <summary>
        /// 获取实例类中的所有属性及值
        /// </summary>
        /// <typeparam name="T">泛类型</typeparam>
        /// <param name="model">实例对象</param>
        /// <returns></returns>
        public static string GetProperties<T>(T model)
        {
            string str = string.Empty;
            if (model == null) return str;
            PropertyInfo[] properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (properties.Length <= 0) return str;
            foreach (PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(model, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String", StringComparison.OrdinalIgnoreCase))
                    str += string.Format("{0}:{1},", name, value);
                else
                    GetProperties(value);
            }
            return str;
        }
        #endregion

        #region 获取DataTable 数据结构
        /// <summary>
        /// 获取DataTable 数据结构
        /// </summary>
        /// <param name="dt">DataTable 数据</param>
        /// <returns></returns>
        public static StringBuilder GetDataStructure(DataTable dt)
        {
            StringBuilder Sbr = new StringBuilder("\n\n<table border=\"1\" cellpadding=\"2\" cellspacing=\"2\">\n");
            if (dt != null)
            {
                Sbr.AppendLine("<tr>");
                for (int i = 0; i < dt.Columns.Count; i++)
                    Sbr.AppendLine("<th>" + dt.Columns[i].ColumnName + "</th>");
                Sbr.AppendLine("</tr>");
                for (int a = 0; a < dt.Rows.Count; a++)
                {
                    Sbr.AppendLine("<tr>");
                    for (int b = 0; b < dt.Columns.Count; b++)
                    {
                        Sbr.AppendLine("<td>" + (dt.Rows[a][b] ?? "&nbsp;") + "</td>");
                    }
                    Sbr.AppendLine("</tr>");
                }
            }
            else
            {
                Sbr.AppendLine("<tr><td>暂无数据</td></tr>");
            }
            Sbr.AppendLine("</table>");
            return Sbr;
        }
        #endregion

        #region 获取DataReader数据结构
        /// <summary>
        /// 获取DataReader数据结构
        /// </summary>
        /// <param name="sdr">DataReader 数据</param>
        /// <returns></returns>
        public static StringBuilder GetDataStructure(System.Data.Common.DbDataReader sdr)
        {
            StringBuilder Sbr = new StringBuilder("\n\n<table border=\"1\" cellpadding=\"2\" cellspacing=\"2\">\n<tr>");
            if (sdr == null) return Sbr.Append("<td>空对象</td></tr></table>");
            for (int i = 0; i < sdr.FieldCount; i++)
                Sbr.AppendLine("<th>" + sdr.GetName(i) + "</th>");
            Sbr.AppendLine("</tr>");
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    Sbr.AppendLine("<tr>");
                    for (int b = 0; b < sdr.FieldCount; b++)
                        Sbr.AppendLine("<td>" + (sdr[b].ToString().IsNullOrEmpty() ? "&nbsp;" : sdr[b].ToString()) + "</td>");
                    Sbr.AppendLine("</tr>");
                }
            }
            Sbr.AppendLine("</table>");
            return Sbr;
        }
        #endregion
    }
}