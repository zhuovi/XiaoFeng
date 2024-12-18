using System;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-05-30 上午 12:44:25          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 加密文件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class EncryptFileAttribute : Attribute
    {

        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public EncryptFileAttribute() : this(true) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="isEncrypt">是否加密</param>
        public EncryptFileAttribute(Boolean isEncrypt)
        {
            this.IsEncrypt = isEncrypt;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否加密
        /// </summary>
        public Boolean IsEncrypt { get; set; }
        #endregion
    }
}