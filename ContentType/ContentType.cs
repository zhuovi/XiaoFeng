﻿using System;
using System.Collections.Generic;
using XiaoFeng.Config;
using static System.Net.Mime.MediaTypeNames;
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
            return (ext.IsNullOrEmpty() || !Data.ContainsKey(ext)) ? Array.Empty<string>() : Data[ext].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
            {"bmp", "image/bmp,application/x-bmp,application/x-ms-bmp" },
            {"gif", "image/gif" },
            {"emf", "application/x-emf" },
            {"ico", "image/x-icon" },
            {"tiff", "image/tiff" },
            {"psd", "image/vnd.adobe.photoshop,application/octet-stream" },
            {"webp", "image/webp" },
            {"art", "image/x-jg" },
            /*文档*/
            {"wmf", "application/x-wmf,application/x-msmetafile" },
            {"doc", "application/msword" },
            {"docm", "application/vnd.ms-word.document.macroEnabled.12" },
            {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            {"dot", "application/msword" },
            {"dotm", "application/vnd.ms-word.template.macroEnabled.12" },
            {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
            {"pdf", "application/pdf" },
            {"ofd", "application/ofd" },
            {"xls", "application/vnd.ms-excel" },
            {"csv", "application/vnd.ms-excel,application/octet-stream,text/csv" },
            {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            {"mdb", "application/x-mdb,application/x-msaccess" },
            {"ppt", "application/powerpoint,application/vnd.ms-powerpoint" },
            {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {"xml", "text/xml" },
            {"flr", "x-world/x-vrml" },
            {"properties","text/plain,application/x-javascript" },
            /*字体*/
            {"woff", "application/font-woff,font/x-woff,font/woff" },
            {"woff2", "application/font-woff2,font/woff2" },
            {"ttf", "application/x-font-truetype,application/octet-stream,font/ttf" },
            {"svg", "image/svg+xml" },
            {"svgz", "image/svg+xml" },
            {"otf", "application/x-font-opentype,font/otf" },
            {"eot", "application/vnd.ms-fontobject,application/octet-stream" },
            /*文件*/
            {"323", "text/h323" },
            {"vml", "text/xml" },
            {"config", "text/xml" },
            {"json", "application/json,text/json" },
            {"js", "application/x-javascript,application/js,text/js" },
            {"txt", "text/plain" },
            {"log", "text/plain" },
            {"bas", "text/plain" },
            {"c", "text/plain" },
            {"h", "text/plain" },
            {"css", "text/css" },
            {"asp", "text/asp" },
            {"atom", "application/atom+xml" },
            {"axs", "application/olescript" },
            {"jsp", "text/html" },
            {"asa", "text/asa" },
            {"asm", "text/plain" },
            {"asmx", "text/plain" },
            {"aspx", "text/plain" },
            {"html", "text/html" },
            {"htm", "text/html" },
            {"conf", "text/plain" },
            {"sql", "text/plain" },
            {"m3u8", "application/vnd.apple.mpegurl" },
            /*音频*/
            {"silk", "audio/silk,application/octet-stream" },
            {"m3u", "audio/mpegurl,audio/x-mpegurl" },
            {"aac", "audio/x-aac,audio/aac" },
            {"adt", "audio/vnd.dlna.adts" },
            {"midi", "audio/mid" },
            {"aif","audio/x-aiff" },
            {"aifc","audio/aiff" },
            {"aiff","audio/aiff" },
            {"au","audio/basic" },
            {"mid", "audio/mid,audio/x-aiff,audio/midi,audio/x-midi" },
            {"wav", "audio/wav" },
            {"avi", "audio/avi,video/x-msvideo,video/avi" },
            {"wma", "audio/x-ms-wma" },
            {"mp1", "audio/mp1" },
            {"mp2", "audio/mp2,video/mpeg" },
            {"mp3", "audio/mp3,audio/mpeg" },
            {"mpga", "audio/mpeg" },
            {"mp4a", "audio/mp4" },
            /*视频*/
            {"mp4", "video/mpeg4,video/mp4" },
            {"mp4v", "video/mpeg4,vidoe/mp4" },
            {"mp2v", "video/mpeg" },
            {"mpeg", "video/mpg,video/mpeg" },
            {"mpg", "video/mpg,video/mpeg" },
            {"mpa", "video/x-mpg,video/mpeg" },
            {"wmv", "video/x-ms-wmv" },
            {"rmvb", "application/vnd.rn-realmedia-vbr" },
            {"swf", "application/x-shockwave-flash" },
            {"fla", "application/octet-stream" },
            {"wmx", "video/x-ms-wmx" },
            {"3g2", "video/3gpp2" },
            {"3gp", "video/3gpp" },
            {"3gp2", "video/3gpp2" },
            {"3gpp", "video/3gpp" },
            {"mkv", "video/x-matroska" },
            {"flv", "flv-application/octet-stream,video/x-flv" },
            {"asf","video/x-ms-asf" },
            {"asr", "video/x-ms-asf" },
            {"asx", "video/x-ms-asf" },
            /*附件*/
            {"apk", "application/vnd.android,application/octet-stream,application/vnd.android.package-archive" },
            {"ipa", "application/octet-stream.ipa,application/octet-stream,application/iphone-package-archive,application/x-itunes-ipa,application/vnd.iphone,application/iphone" },
            {"rar", "application/x-rar,application/octet-stream,application/x-compressed" },
            {"zip", "application/zip,application/octet-stream,application/x-compressed,application/x-zip-compressed" },
            {"gzip", "application/zip,application/octet-stream" },
            {"gtar", "application/x-gtar" },
            {"7z", "application/octet-stream" },
            {"dll", "application/x-msdownload" },
            {"gz", "application/x-gzip" },
            {"wgt", "application/octet-stream" },
            /*其它*/
            {"aaf","application/octet-stream" },
            {"aca","application/octet-stream" },
            {"accdb","application/msaccess" },
            {"accde","application/msaccess" },
            {"accdt","application/msaccess" },
            {"acx","application/internet-property-stream" },
            {"afm","application/octet-stream" },
            {"asd","application/octet-stream" },
            {"asi","application/octet-stream" },
            {"ai","application/postscript" },
            {"appcache","text/cache-manifest" },
            {"application","application/x-ms-application" },
            {"avif","image/avif" },
            {"bcpio", "application/x-bcpio" },
            {"bin", "application/octet-stream" },
            {"cab", "application/octet-stream,application/vnd.ms-cab-compressed" },
            {"calx", "application/vnd.ms-office.calx" },
            {"cat", "application/vnd.ms-pki.seccat" },
            {"cdf", "application/x-cdf" },
            {"chm", "application/octet-stream" },
            {"class", "application/x-java-applet" },
            {"clp", "application/x-msclip" },
            {"cmx", "image/x-cmx" },
            {"cod", "image/cis-cod" },
            {"cnf", "text/plain" },
            {"cpio", "application/x-cpio" },
            {"cpp", "text/plain" },
            {"crd", "application/x-mscardfile" },
            {"crl", "application/pkix-crl" },
            {"crt", "application/x-x509-ca-cert" },
            {"csh", "application/x-csh" },
            {"cur", "application/octet-stream" },
            {"dcr", "application/x-director" },
            {"deploy", "application/octet-stream" },
            {"der", "application/x-x509-ca-cert" },
            {"dib", "image/bmp" },
            {"dir", "application/x-director" },
            {"disco", "text/xml" },
            {"dlm", "text/dlm" },
            {"dsp", "application/octet-stream" },
            {"dtd", "text/xml" },
            {"dvi", "application/x-dvi" },
            {"dwf", "drawing/x-dwf"},
            {"dwp", "application/octet-stream" },
            {"dxr", "application/x-director" },
            {"eml", "message/rfc822" },
            {"emz", "application/octet-stream" },
            {"eps", "application/postscript" },
            {"esd", "application/vnd.ms-cab-compressed" },
            {"etx", "text/x-setext" },
            {"evy", "application/envoy" },
            {"exe", "application/octet-stream"},
            {"flb", "model/gltf-binary" },
            {"fdf", "application/vnd.fdf" },
            {"fif", "application/fractals" },
            {"hdf", "application/x-hdf"},
            {"hdml", "text/x-hdml" },
            {"hhc", "application/x-oleobject" },
            {"hhk", "application/octet-stream" },
            {"hhp", "application/octet-stream"},
            {"hlp", "application/winhlp"},
            {"hqx", "application/mac-binhex40"},
            {"hta", "application/hta"},
            {"htc", "text/x-component"},
            {"htt", "text/webviewhtml"},
            {"hxt", "text/html"},
            {"ics", "application/octet-stream"},
            {"ief", "image/ief"},
            {"iii", "application/x-iphone"},
            {"inf", "application/octet-stream"},
            {"ins", "application/x-internet-signup"},
            {"isp", "application/x-internet-signup"},
            {"ivf", "video/x-ivf"},
            {"jar", "application/java-archive"},
            {"java", "application/octet-stream"},
            {"jck", "application/liquidmotion"},
            {"jcz", "application/liquidmotion"},
            {"jfif", "image/pjpeg"},
            {"jpb", "application/octet-stream"},
            {"jsx", "text/jscript"},
            {"latex", "application/x-latex"},
            {"lit", "application/x-ms-reader"},
            {"lpk", "application/octet-stream"},
            {"lsf", "video/x-la-asf"},
            {"lsx", "video/x-la-asf"},
            {"lzh", "application/octet-stream"},
            {"m13", "application/x-msmediaview"},
            {"m14", "application/x-msmediaview"},
            {"m1v", "video/mpeg"},
            {"man", "application/x-troff-man"},
            {"manifest", "application/x-ms-manifest"},
            {"map", "text/plain"},
            {"md", "text/markdown"},
            {"mdp", "application/octet-stream"},
            {"me", "application/x-troff-me"},
            {"mht", "message/rfc822"},
            {"mhtml", "message/rfc822"},
            {"mix", "application/octet-stream"},
            {"mmf", "application/x-smaf"},
            {"mno", "text/xml"},
            {"mny", "application/x-msmoney"},
            {"mov", "video/quicktime"},
            {"movie", "video/x-sgi-movie"},
            {"mpe", "video/mpeg"},
            {"mpp", "application/vnd.ms-project"},
            {"mpv2", "video/mpeg"},
            {"ms", "application/x-troff-ms"},
            {"msi", "application/octet-stream"},
            {"mso", "application/octet-stream"},
            {"mvb", "application/x-msmediaview"},
            {"mvc", "application/x-miva-compiled"},
            {"nc", "application/x-netcdf"},
            {"nsc", "video/x-ms-asf"},
            {"nws", "message/rfc822"},
            {"ocx", "application/octet-stream"},
            {"oda", "application/oda"},
            {"odc", "text/x-ms-odc"},
            {"ods", "application/oleobject"},
            {"oga", "audio/ogg"},
            {"ogg", "audio/ogg"},
            {"ogv", "audio/ogg"},
            {"one", "application/onenote"},
            {"onea", "application/onenote"},
            {"onepkg", "application/onenote"},
            {"onetmp", "application/onenote"},
            {"onetoc", "application/onenote"},
            {"onetoc2", "application/onenote"},
            {"osdx", "application/opensearchdescription+xml"},
            {"p10", "application/pkcs10"},
            {"p12", "application/x-pkcs12"},
            {"p7b", "application/x-pkcs7-certificates"},
            {"p7c", "application/pkcs7-mime"},
            {"p7m", "application/pkcs7-mime"},
            {"p7r", "application/x-pkcs7-certreqresp"},
            {"p7s", "application/pkcs7-signature"},
            {"pbm", "image/x-portable-bitmap"},
            {"pcx", "application/octet-stream"},
            {"pcz", "application/octet-stream"},
            {"pfb", "application/octet-stream"},
            {"pfm", "application/octet-stream"},
            {"pfx", "application/x-pkcs12"},
            {"pgm", "image/x-portable-graymap"},
            {"pko", "application/vnd.ms-pki.pko"},
            {"pma", "application/x-perfmon"},
            {"pmc", "application/x-perfmon"},
            {"pml", "application/x-perfmon"},
            {"pmr", "application/x-perfmon"},
            {"pmw", "application/x-perfmon"},
            {"pnm", "image/x-portable-anymap"},
            {"pnz", "image/png"},
            {"pot", "application/vnd.ms-powerpoint"},
            {"potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
            {"potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
            {"ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
            {"ppm", "image/x-portable-pixmap"},
            {"pps", "application/vnd.ms-powerpoint"},
            {"ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
            {"ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
            {"pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},

            {"prf", "application/pics-rules"},
            {"prm", "application/octet-stream"},
            {"prx", "application/octet-stream"},
            {"ps", "application/postscript"},
            {"psm", "application/octet-stream"},
            {"psp", "application/octet-stream"},
            {"pub", "application/x-mspublisher"},
            {"qt", "video/quicktime"},
            {"qtl", "application/x-quicktimeplayer"},
            {"qxd", "application/octet-stream"},
            {"ra", "audio/x-pn-realaudio"},
            {"ram", "audio/x-pn-realaudio"},
            {"ras", "image/x-cmu-raster"},
            {"rf", "image/vnd.rn-realflash"},
            {"rgb", "image/x-rgb"},
            {"rm", "application/vnd.rn-realmedia"},
            {"rmi", "audio/mid"},
            {"roff", "application/x-troff"},
            {"rpm", "audio/x-pn-realaudio-plugin"},
            {"rtf", "application/rtf,text/richtext,text/rtf"},
            {"rtx", "text/richtext"},
            {"scd", "application/x-msschedule"},
            {"sct", "text/scriptlet"},
            {"sea", "application/octet-stream"},
            {"setpay", "application/set-payment-initiation"},
            {"setreg", "application/set-registration-initiation"},
            {"sgml", "text/sgml"},
            {"sh", "application/x-sh"},
            {"shar", "application/x-shar"},
            {"sit", "application/x-stuffit"},
            {"sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
            {"sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
            {"smd", "audio/x-smd"},
            {"smi", "application/octet-stream"},
            {"smx", "audio/x-smd"},
            {"smz", "audio/x-smd"},
            {"snd", "audio/basic"},
            {"snp", "application/octet-stream"},
            {"spc", "application/x-pkcs7-certificates"},
            {"spl", "application/futuresplash"},
            {"spx", "audio/ogg"},
            {"src", "application/x-wais-source"},
            {"ssm", "application/streamingmedia"},
            {"sst", "application/vnd.ms-pki.certstore"},
            {"stl", "application/vnd.ms-pki.stl"},
            {"sv4cpio", "application/x-sv4cpio"},
            {"sv4crc", "application/x-sv4crc"},
            {"t", "application/x-troff"},
            {"tar", "application/x-tar"},
            {"tcl", "application/x-tcl"},
            {"tex", "application/x-tex"},
            {"texi", "application/x-texinfo"},
            {"texinfo", "application/x-texinfo"},
            {"tgz", "application/x-compressed"},
            {"thmx", "application/vnd.ms-officetheme"},
            {"thn", "application/octet-stream"},
            {"tif", "image/tiff"},
            {"toc", "application/octet-stream"},
            {"tr", "application/x-troff"},
            {"trm", "application/x-msterminal"},
            {"ts", "video/vnd.dlna.mpeg-tts"},
            {"tsv", "text/tab-separated-values"},
            {"tts", "video/vnd.dlna.mpeg-tts"},
            {"u32", "application/octet-stream"},
            {"uls", "text/iuls"},
            {"ustar", "application/x-ustar"},
            {"vbs", "text/vbscript"},
            {"vcf", "text/x-vcard"},
            {"vcs", "text/plain"},
            {"vdx", "application/vnd.ms-visio.viewer"},
            {"vsd", "application/vnd.visio"},
            {"vss", "application/vnd.visio"},
            {"vst", "application/vnd.visio"},
            {"vsto", "application/x-ms-vsto"},
            {"vsw", "application/vnd.visio"},
            {"vsx", "application/vnd.visio"},
            {"vtx", "application/vnd.visio"},
            {"wax", "audio/x-ms-wax"},
            {"wasm", "application/wasm"},
            {"wbmp", "image/vnd.wap.wbmp"},
            {"wcm", "application/vnd.ms-works"},
            {"wdb", "application/vnd.ms-works"},
            {"webm", "video/webm"},
            {"wks", "application/vnd.ms-works"},
            {"wm", "video/x-ms-wm"},
            {"wmd", "application/x-ms-wmd"},
            {"wml", "text/vnd.wap.wml"},
            {"wmlc", "application/vnd.wap.wmlc"},
            {"wmls", "text/vnd.wap.wmlscript"},
            {"wmlsc", "application/vnd.wap.wmlscriptc"},
            {"wmp", "video/x-ms-wmp"},
            {"wmz", "application/x-ms-wmz"},
            {"wps", "application/vnd.ms-works"},
            {"wri", "application/x-mswrite"},
            {"wrl", "x-world/x-vrml"},
            {"wrz", "x-world/x-vrml"},
            {"wsdl", "text/xml"},
            {"wvx", "video/x-ms-wvx"},
            {"x", "application/directx"},
            {"xaf", "x-world/x-vrml"},
            {"xaml", "application/xaml+xml"},
            {"xap", "application/x-silverlight-app"},
            {"xbap", "application/x-ms-xbap"},
            {"xbm", "image/x-xbitmap"},
            {"xdr", "text/plain"},
            {"xht", "application/xhtml+xml"},
            {"xhtml", "application/xhtml+xml"},
            {"xla", "application/vnd.ms-excel"},
            {"xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
            {"xlc", "application/vnd.ms-excel"},
            {"xlm", "application/vnd.ms-excel"},
            {"xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {"xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
            {"xlt", "application/vnd.ms-excel"},
            {"xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
            {"xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
            {"xlw", "application/vnd.ms-excel"},
            {"xof", "x-world/x-vrml"},
            {"xpm", "image/x-xpixmap"},
            {"xps", "application/vnd.ms-xpsdocument"},
            {"xsd", "text/xml"},
            {"xsf", "text/xml"},
            {"xsl", "text/xml"},
            {"xslt", "text/xml"},
            {"xsn", "application/octet-stream"},
            {"xtp", "application/octet-stream"},
            {"xwd", "image/x-xwindowdump"},
            {"z", "application/x-compress"}
        };
        #endregion
    }
}