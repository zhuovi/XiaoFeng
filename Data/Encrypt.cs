using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using XiaoFeng;
using XiaoFeng.Config;
using XiaoFeng.IO;
namespace XiaoFeng.Data
{
    /// <summary>
    /// 加密数据库连接操作类
    /// Version : v 1.0.0
    /// Create Date : 2015-12-26
    /// Author : jacky
    /// Site : www.zhuovi.com
    /// QQ : 7092734
    /// Email : jacky@zhuovi.com
    /// </summary>
    public class Encrypt 
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Encrypt() { }
        #endregion

        #region 属性
        /// <summary>
        /// 当前KEY
        /// </summary>
        private static string _Key = "";
        /// <summary>
        /// 当前KEY
        /// </summary>
        private static string Key
        {
            get
            {
                if (_Key.IsNullOrEmpty())
                    _Key = GetKey();
                return _Key;
            }
        }
        #endregion

        #region 方法

        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="AppName">数据库连接配置或名称或字符串</param>
        /// <returns></returns>
        public static string set(string AppName)
        {
            if (AppName.IsNullOrEmpty()) return "";
            if (AppName.IsMatch("^ZW:+")) return AppName;
            return EncryptHelper(AppName);
        }
        #endregion

        #region 获取原文
        /// <summary>
        /// 获取原文
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <returns></returns>
        public static string get(string data) { return DecryptHelper(data); }
        #endregion

        #region 加密
        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="data">原文数据</param>
        /// <returns></returns>
        private static string EncryptHelper(string data)
        {
            return "ZW:" + data.DESEncrypt(Key);
        }
        #endregion

        #region 解密
        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <returns></returns>
        private static string DecryptHelper(string data)
        {
            if (data.IsMatch(@"^ZW:+"))
                try
                {
                    data = data.RemovePattern(@"(^ZW:+|;+$)").DESDecrypt(Key);
                }
                catch
                {
                    return data;
                }
            return data.IsMatch(@"\{\s*RootPath\s*\}") ?
                    data.ReplacePattern(@"\{\s*RootPath\s*\}", FileHelper.GetCurrentDirectory()) :
                    data;
        }
        #endregion

        #region 获取Key
        /// <summary>
        /// 获取Key
        /// </summary>
        /// <returns></returns>
        private static string GetKey()
        {
            string _Key = Setting.Current.DataKey;
            if (_Key.IsNullOrEmpty()) _Key = "www.zhuovi.com";
            _Key = ("1985" + _Key.AESEncrypt(_Key).MD5() + "1006").MD5();
            return _Key;
        }
        #endregion

        #endregion
    }
}