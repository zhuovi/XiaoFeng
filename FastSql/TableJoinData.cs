using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Data.SQL;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-18 15:16:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// 联表数据
    /// </summary>
    public class TableJoinData
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public TableJoinData()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="joinType">关联表类型</param>
        /// <param name="tableTypeA">关联表A</param>
        /// <param name="tableTypeB">关联表B</param>
        public TableJoinData(JoinType joinType, TableType tableTypeA, TableType tableTypeB)
        {
            JoinType = joinType;
            TableTypeA = tableTypeA;
            TableTypeB = tableTypeB;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 关联表类型
        /// </summary>
        public JoinType JoinType { get; set; }
        /// <summary>
        /// 关联表A
        /// </summary>
        public TableType TableTypeA { get; set; }
        /// <summary>
        /// 关联表B
        /// </summary>
        public TableType TableTypeB { get; set; }
        /// <summary>
        /// 关联字段
        /// </summary>
        private List<string> OnString { get; set; }
        /// <summary>
        /// 前缀
        /// </summary>
        private string _Prefix = string.Empty;
        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix
        {
            get
            {
                if (this._Prefix.IsNullOrEmpty())
                {
                    this._Prefix = $"{this.TableTypeA}{this.TableTypeB}".OrderBy();
                }
                return this._Prefix;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加on字符串
        /// </summary>
        /// <param name="onString">on字符串</param>
        public void AddOnString(string onString)
        {
            if (this.OnString == null) this.OnString = new List<string>();
            onString = onString.RemovePattern(@"^\s*on\s+");
            if (onString.IsMatch(@"\s+and\s+"))
            {
                onString.SplitPattern(@"\s+and\s+").Each(p =>
                {
                    if (this.OnString.Contains(p)) return;
                    this.OnString.Add(p);
                });
            }
            else
            {
                if (this.OnString.Contains(onString)) return;
                this.OnString.Add(onString);
            }
        }
        /// <summary>
        /// 获取on字符串
        /// </summary>
        /// <returns></returns>
        public string GetOnString()
        {
            if (this.OnString == null || this.OnString.Count == 0) return string.Empty;
            return this.OnString.Join(" and ");
        }
        #endregion
    }
}