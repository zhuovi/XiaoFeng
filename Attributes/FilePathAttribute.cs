using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-02 11:12:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Attributes
{
    /// <summary>
    /// 路径配置属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class FilePathAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FilePathAttribute()
        {

        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        /// <param name="description">说明</param>
        /// <param name="length">长度</param>
        /// <param name="ext">后缀</param>
        public FilePathAttribute(string name, string path, string description, int length, string ext)
        {
            this.Name = name;
            this.Path = path;
            this.Description = description;
            this.Length = length;
            this.Ext = ext;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Ext { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}