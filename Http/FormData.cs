using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-26 18:07:09                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// FormData 类说明
    /// </summary>
    public class FormData
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FormData() { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="formType">类型</param>
        public FormData(string name, string value, FormType formType)
        {
            Name = name;
            Value = value;
            FormType = formType;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public FormType FormType { get; set; } = FormType.Text;
        #endregion

        #region 方法

        #endregion
    }
}