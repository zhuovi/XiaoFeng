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
    /// FormData 操作类
    /// </summary>
    public class FormData
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public FormData() { }
        /// <summary>
        /// 初始化一个新实例
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
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public FormData(string name,string value):this(name,value,FormType.Text) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">文件流</param>
        public FormData(string name, byte[] data)
        {
            this.Name = name;
            this.Data = data;
            this.FormType = FormType.File;
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
        /// 文件流
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public FormType FormType { get; set; } = FormType.Text;
        #endregion

        #region 方法

        #endregion
    }
}