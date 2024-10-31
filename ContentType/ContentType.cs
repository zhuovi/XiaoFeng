using System;
using System.Collections.Generic;
using XiaoFeng.Config;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 内容类型
    /// </summary>
    public class ContentTypes
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        static ContentTypes()
        {
            var list = ContentTypeMapping.Current.List;
            if (list != null && list.Count > 0)
            {
                Data.Clear();
                list.Each(a =>
                {
                    Data.Add(a.Ext.Trim('.'), a.Mime);
                });
            }
        }
        /// <summary>
        /// 获取后缀名Mime数组
        /// </summary>
        /// <param name="ext">后缀名</param>
        /// <returns></returns>
        public static string[] Get(string ext)
        {
            return (ext.IsNullOrEmpty() || !Data.ContainsKey(ext)) ? null : Data[ext].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// 获取后缀名Mime
        /// </summary>
        /// <param name="ext">后缀名</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        public static string get(string ext)
        {
            ext = ext.Trim('.');
            return (ext.IsNullOrEmpty() || !Data.ContainsKey(ext)) ? "" : Data[ext];
        }

        #region 内容类型字典
        /// <summary>
        /// 内容类型字典
        /// </summary>
        public static Dictionary<string, string> Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            /*图片*/
            {"jpg", "image/jpeg,image/pjpeg" },
            {"jpeg", "image/jpeg,image/pjpeg" },
            {"jpe", "image/jpeg,image/pjpeg" },
            {"png", "application/x-png,image/png,image/x-png" },
            {"bmp", "application/x-bmp,application/x-ms-bmp" },
            {"gif", "image/gif" },
            {"emf", "application/x-emf" },
            {"ico", "image/x-icon" },
            {"tiff", "image/tiff" },
            {"psd", "image/vnd.adobe.photoshop" },
            /*文档*/
            {"wmf", "application/x-wmf" },
            {"doc", "application/msword" },
            {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            {"pdf", "application/pdf" },
            {"xls", "application/vnd.ms-excel" },
            {"csv", "application/vnd.ms-excel" },
            {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            {"mdb", "application/x-mdb" },
            {"ppt", "application/powerpoint" },
            {"xml", "text/xml" },
            /*字体*/
            {"woff", "application/font-woff" },
            {"woff2", "application/font-woff2" },
            {"ttf", "application/x-font-truetype" },
            {"svg", "image/svg+xml" },
            {"otf", "application/x-font-opentype" },
            {"eot", "application/vnd.ms-fontobject" },
            /*文件*/
            {"vml", "text/xml" },
            {"json", "application/json,text/json" },
            {"js", "application/x-javascript,application/js,text/js" },
            {"txt", "text/plain" },
            {"log", "text/plain" },
            {"css", "text/css" },
            {"asp", "text/asp" },
            {"jsp", "text/html" },
            {"asa", "text/asa" },
            {"html", "text/html" },
            {"htm", "text/html" },
            {"m3u8", "application/vnd.apple.mpegurl" },
            /*音频*/
            {"silk", "audio/silk,application/octet-stream" },
            {"m3u", "audio/mpegurl" },
            {"aac", "audio/x-aac" },
            {"midi", "audio/mid" },
            {"mid", "audio/mid,audio/x-aiff,audio/midi,audio/x-midi" },
            {"wav", "audio/wav" },
            {"avi", "audio/avi,video/x-msvideo" },
            {"wma", "audio/x-ms-wma" },
            {"mp1", "audio/mp1" },
            {"mp2", "audio/mp2" },
            {"mp3", "audio/mp3" },
            {"mpga", "audio/mpeg" },
            {"mp4a", "audio/mp4" },
            /*视频*/
            {"mp4", "video/mpeg4" },
            {"mp2v", "video/mpeg" },
            {"mpeg", "video/mpg" },
            {"mpg", "video/mpg" },
            {"mpa", "video/x-mpg" },
            {"wmv", "video/x-ms-wmv" },
            {"rmvb", "application/vnd.rn-realmedia-vbr" },
            {"swf", "application/x-shockwave-flash" },
            {"wmx", "video/x-ms-wmx" },
            {"3gp", "video/3gpp" },
            {"mkv", "video/x-matroska" },
            {"flv", "flv-application/octet-stream,video/x-flv" },
            /*附件*/
            {"apk", "application/vnd.android,application/octet-stream,application/vnd.android.package-archive" },
            {"ipa", "application/octet-stream.ipa,application/octet-stream,application/iphone-package-archive,application/x-itunes-ipa,application/vnd.iphone,application/iphone" },
            {"rar", "application/x-rar,application/octet-stream" },
            {"zip", "application/zip,application/octet-stream,application/x-compressed,application/x-zip-compressed" },
            {"gzip", "application/zip,application/octet-stream" },
            {"7z", "application/octet-stream" },
            {"dll", "application/x-msdownload" },
            {"gz", "application/x-gzip" },
            {"wgt", "application/octet-stream" }
        };
        #endregion
    }
}