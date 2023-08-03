using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-28 11:56:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /*
     * ----------------------------------------------------------------------
     * | Byte |     0       |       1       |       2       |       3       |
     * ----------------------------------------------------------------------
     * |  0   |     Magic   |    Opcode     |    Status     |    DataType   |
     * ----------------------------------------------------------------------
     * |  4   |                    Payload Length                           |
     * ----------------------------------------------------------------------
     * |  8   |        Header Length        |             预留              |
     * ----------------------------------------------------------------------
     * |  12  |                      Header Data                            |
     * ----------------------------------------------------------------------
     * |  M   |                     Payload Data                            |
     * ----------------------------------------------------------------------
     * Magic 魔法数字 区别 是请求包还是响应包  0x10为请求包    0x20为响应包
     * Opcode 操作符 对应的命令符   0x00 普通消息    0x01 订阅消息
     * Status 状态    0x00成功  0x01失败
     * DataType 数据类型 0x00 文本
     * Payload Length 数据长度
     * Payload Data 数据
     */

    /// <summary>
    /// 网络协议包
    /// </summary>
    public class Packet : IPacket
    {
        /// <summary>
        /// 包类型
        /// </summary>
        public PacketType PacketType { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public DataFormatType DataType { get; set; }
        /// <summary>
        /// 头长
        /// </summary>
        public long HeaderLength { get; set; }
        /// <summary>
        /// 头
        /// </summary>
        public byte[] Header { get; set; }
        /// <summary>
        /// body长
        /// </summary>
        public long PayloadLength { get; set; }
        /// <summary>
        /// body
        /// </summary>
        public byte[] Payload { get; set; }
    }
    /// <summary>
    /// 包类型
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// 请求包
        /// </summary>
        Request = 0,
        /// <summary>
        /// 响应包
        /// </summary>
        Response =1
    }
    /// <summary>
    /// 数据格式类型
    /// </summary>
    public enum DataFormatType
    {
        /// <summary>
        /// Text
        /// </summary>
        TEXT = 0,
        /// <summary>
        /// file
        /// </summary>
        FILE = 1,
        #region 图片
        /// <summary>
        /// jpeg
        /// </summary>
        JPEG = 11,
        /// <summary>
        /// gif
        /// </summary>
        GIF = 12,
        /// <summary>
        /// jpg
        /// </summary>
        JPG = 13,
        /// <summary>
        /// bmp
        /// </summary>
        BMP = 14,
        /// <summary>
        /// svg
        /// </summary>
        SVG = 15,
        /// <summary>
        /// psd
        /// </summary>
        PSD = 16,
        /// <summary>
        /// tiff
        /// </summary>
        TIFF = 17,
        /// <summary>
        /// webp
        /// </summary>
        WEBP = 18,
        /// <summary>
        /// pdt
        /// </summary>
        PDT = 19,
        /// <summary>
        /// dds
        /// </summary>
        DDS = 20,
        /// <summary>
        /// xmp
        /// </summary>
        XMP = 21,
        /// <summary>
        /// ico
        /// </summary>
        ICO = 22,
        /// <summary>
        /// png
        /// </summary>
        PNG = 23,
        /// <summary>
        /// emf
        /// </summary>
        EMF = 24,
        /// <summary>
        /// jpe
        /// </summary>
        JPE = 25,
        #endregion

        #region 文档
        /// <summary>
        /// doc
        /// </summary>
        DOC = 51,
        /// <summary>
        /// docx
        /// </summary>
        DOCX = 52,
        /// <summary>
        /// xls
        /// </summary>
        XLS = 53,
        /// <summary>
        /// xlsx
        /// </summary>
        XLSX = 54,
        /// <summary>
        /// ppt
        /// </summary>
        PPT = 55,
        /// <summary>
        /// pptx
        /// </summary>
        PPTX = 56,
        /// <summary>
        /// txt
        /// </summary>
        TXT = 57,
        /// <summary>
        /// pdf
        /// </summary>
        PDF = 58,
        /// <summary>
        /// ofd
        /// </summary>
        OFD = 59,
        /// <summary>
        /// xml
        /// </summary>
        XML = 60,
        /// <summary>
        /// json
        /// </summary>
        JSON = 61,
        /// <summary>
        /// config
        /// </summary>
        CONFIG = 62,
        /// <summary>
        /// sh
        /// </summary>
        SH = 63,
        /// <summary>
        /// bat
        /// </summary>
        BAT = 64,
        /// <summary>
        /// wmf
        /// </summary>
        WMF = 65,
        /// <summary>
        /// csv
        /// </summary>
        CSV = 66,
        /// <summary>
        /// mdb
        /// </summary>
        MDB = 67,
        /// <summary>
        /// vml
        /// </summary>
        VML = 68,
        /// <summary>
        /// yml
        /// </summary>
        YML = 69,
        /// <summary>
        /// js
        /// </summary>
        JS = 70,
        /// <summary>
        /// css
        /// </summary>
        CSS = 71,
        /// <summary>
        /// log
        /// </summary>
        LOG = 72,
        /// <summary>
        /// asp
        /// </summary>
        ASP = 73,
        /// <summary>
        /// aspx
        /// </summary>
        ASPX = 74,
        /// <summary>
        /// jsp
        /// </summary>
        JSP = 75,
        /// <summary>
        /// asa
        /// </summary>
        ASA = 76,
        /// <summary>
        /// html
        /// </summary>
        HTML = 77,
        /// <summary>
        /// htm
        /// </summary>
        HTM = 78,
        /// <summary>
        /// htmlx
        /// </summary>
        HTMLX = 79,
        /// <summary>
        /// m3u8
        /// </summary>
        M3U8 = 80,
        /// <summary>
        /// rar
        /// </summary>
        RAR = 81,
        /// <summary>
        /// zip
        /// </summary>
        ZIP = 82,
        /// <summary>
        /// iso
        /// </summary>
        ISO = 83,
        /// <summary>
        /// apk
        /// </summary>
        APK = 84,
        /// <summary>
        /// ipa
        /// </summary>
        IPA = 85,
        /// <summary>
        /// gzip
        /// </summary>
        GZIP = 86,
        /// <summary>
        /// dll
        /// </summary>
        DLL = 87,
        /// <summary>
        /// gz
        /// </summary>
        GZ = 88,
        /// <summary>
        /// tar
        /// </summary>
        TAR = 89,
        /// <summary>
        /// wgt
        /// </summary>
        WGT = 90,
        /// <summary>
        /// php
        /// </summary>
        PHP = 91,
        /// <summary>
        /// asmx
        /// </summary>
        ASMX = 92,
        /// <summary>
        /// jspx
        /// </summary>
        JSPX = 93,
        /// <summary>
        /// cs
        /// </summary>
        CS = 94,
        /// <summary>
        /// pcshtmlng
        /// </summary>
        CSHTML = 95,
        /// <summary>
        /// nuget
        /// </summary>
        NUGET = 96,
        /// <summary>
        /// sln
        /// </summary>
        SLN = 97,
        /// <summary>
        /// exe
        /// </summary>
        EXE = 98,
        /// <summary>
        /// pdb
        /// </summary>
        PDB = 99,
        /// <summary>
        /// so
        /// </summary>
        SO = 100,
        #endregion

        #region 音视频
        /// <summary>
        /// silk
        /// </summary>
        SILK = 151,
        /// <summary>
        /// m3u
        /// </summary>
        M3U = 152,
        /// <summary>
        /// aac
        /// </summary>
        AAC = 153,
        /// <summary>
        /// midi
        /// </summary>
        MIDI = 154,
        /// <summary>
        /// mid
        /// </summary>
        MID = 155,
        /// <summary>
        /// wav
        /// </summary>
        WAV = 156,
        /// <summary>
        /// avi
        /// </summary>
        AVI = 157,
        /// <summary>
        /// wma
        /// </summary>
        WMA = 158,
        /// <summary>
        /// mp1
        /// </summary>
        MP1 = 159,
        /// <summary>
        /// mp2
        /// </summary>
        MP2 = 160,
        /// <summary>
        /// mp3
        /// </summary>
        MP3 = 161,
        /// <summary>
        /// mpga
        /// </summary>
        MPGA = 162,
        /// <summary>
        /// mp4a
        /// </summary>
        MP4A = 163,
        /// <summary>
        /// mp4
        /// </summary>
        MP4 = 164,
        /// <summary>
        /// mp2v
        /// </summary>
        MP2V = 165,
        /// <summary>
        /// mpeg
        /// </summary>
        MPEG = 166,
        /// <summary>
        /// mpg
        /// </summary>
        MPG = 167,
        /// <summary>
        /// mpa
        /// </summary>
        MPA = 168,
        /// <summary>
        /// wmv
        /// </summary>
        WMV = 169,
        /// <summary>
        /// rmvb
        /// </summary>
        RMVB = 170,
        /// <summary>
        /// swf
        /// </summary>
        SWF = 171,
        /// <summary>
        /// wmx
        /// </summary>
        WMX = 172,
        /// <summary>
        /// mkv
        /// </summary>
        MKV = 173,
        /// <summary>
        /// flv
        /// </summary>
        FLV = 174
        #endregion
    }
}