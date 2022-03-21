using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
namespace XiaoFeng.Image
{
    #region 打水印操作类
    /// <summary>
    /// 打水印操作类 V 1.1
    /// Author: jacky
    /// QQ: 7092734
    /// </summary>
    public class ImageWater
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ImageWater() { }

        #region 方法

        #region 添加打文字水印
        /// <summary>
        /// 添加打文字水印
        /// </summary>
        public Boolean AddText()
        {
            if (!File.Exists(this.ImagePath)) { this.ErrMessage = "图片不存在."; return false; }
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(this.ImagePath);
                Graphics g = Graphics.FromImage(image);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
                Font f = new Font(this.FontFamily, this.FontSize);
                SizeF fSize = new SizeF();
                fSize = g.MeasureString(this.WaterText, f);
                switch (this.TextAlign)
                {
                    case 0:
                        this.FontPositionX = (image.Width - fSize.Width) / 2;
                        this.FontPositionY = (image.Height - fSize.Height) / 2;
                        break;
                    case 1:
                        this.FontPositionX = 5;
                        this.FontPositionY = 5;
                        break;
                    case 2:
                        this.FontPositionX = image.Width - fSize.Width - 5;
                        this.FontPositionY = 5;
                        break;
                    case 3:
                        this.FontPositionX = 5;
                        this.FontPositionY = image.Height - fSize.Height - 5;
                        break;
                    case 4:
                        this.FontPositionX = image.Width - fSize.Width - 5;
                        this.FontPositionY = image.Height - fSize.Height - 5;
                        break;
                    default:
                        break;

                }
                Brush b = new SolidBrush(this.FontColor);
                g.DrawString(this.WaterText, f, b, this.FontPositionX, this.FontPositionY);
                g.Dispose();
                image.Save(this.NewImagePath);
                image.Dispose();
                if(this.IsDelTempFile)File.Delete(this.ImagePath);
                return true;
            }
            catch (Exception ex)
            {
                this.ErrMessage = "出错：" + ex.Message.ToString();
                if (this.IsDelTempFile && File.Exists(this.ImagePath)) File.Delete(this.ImagePath);
                return false;
            }
        }
        #endregion

        #region 添加打图片水印
        /// <summary>
        /// 添加打图片水印
        /// </summary>
        public Boolean AddImage()
        {
            if (!File.Exists(this.ImagePath)) { this.ErrMessage = "图片不存在."; return false; }
            if (!File.Exists(this.WaterImagePath))
            {
                this.ErrMessage = "水印图片不存在";
                if (File.Exists(this.ImagePath)) File.Delete(this.ImagePath);
                return false;
            }
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(this.ImagePath);
                System.Drawing.Image copyImage = System.Drawing.Image.FromFile(this.WaterImagePath);
                Graphics g = Graphics.FromImage(image);

                switch (this.TextAlign)
                {
                    case 0:
                        this.FontPositionX = (image.Width - copyImage.Width) / 2;
                        this.FontPositionY = (image.Height - copyImage.Height) / 2;
                        break;
                    case 1:
                        this.FontPositionX = 0;
                        this.FontPositionY = 0;
                        break;
                    case 2:
                        this.FontPositionX = image.Width - copyImage.Width;
                        this.FontPositionY = 0;
                        break;
                    case 3:
                        this.FontPositionX = 0;
                        this.FontPositionY = image.Height - copyImage.Height;
                        break;
                    case 4:
                        this.FontPositionX = image.Width - copyImage.Width;
                        this.FontPositionY = image.Height - copyImage.Height;
                        break;
                    default:
                        break;
                }

                g.DrawImage(copyImage, new Rectangle(int.Parse(this.FontPositionX.ToString()), int.Parse(this.FontPositionY.ToString()), copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                g.Dispose();
                image.Save(this.NewImagePath);
                image.Dispose();
                if (this.IsDelTempFile) File.Delete(this.ImagePath);
                return true;
            }
            catch (Exception ex)
            {
                this.ErrMessage = "出错：" + ex.Message.ToString();
                if (this.IsDelTempFile && File.Exists(this.ImagePath)) File.Delete(this.ImagePath);
                return false;
            }
        }
        #endregion

        #endregion

        #region  属性
        /// <summary>
        /// 错误信息
        /// </summary>
        private string errMessage = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMessage { get { return errMessage; } set { errMessage = value; } }
        /// <summary>
        /// 打水印文字
        /// </summary>
        private string waterText = "www.d369.net";
        /// <summary>
        /// 打水印文字
        /// </summary>
        public string WaterText { get { return waterText; } set { waterText = value; } }
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        private Boolean _isDelTempFile = true;
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        public Boolean IsDelTempFile { get { return _isDelTempFile; } set { _isDelTempFile = value; } }
        /// <summary>
        /// 水印文字字体
        /// </summary>
        private string fontFamily = "Verdana";
        /// <summary>
        /// 水印文字字体
        /// </summary>
        public string FontFamily { get { return fontFamily; } set { fontFamily = value; } }
        /// <summary>
        /// 水印文字大小
        /// </summary>
        private int fontSize = 32;
        /// <summary>
        /// 水印文字大小
        /// </summary>
        public int FontSize { get { return fontSize; } set { fontSize = value; } }
        /// <summary>
        /// 水印文字是否加粗
        /// </summary>
        private Boolean fontWeight = true;
        /// <summary>
        /// 水印文字是否加粗
        /// </summary>
        public Boolean FontWeight { get { return fontWeight; } set { fontWeight = value; } }
        /// <summary>
        /// 水印文字是否加斜
        /// </summary>
        private Boolean fontItalic = false;
        /// <summary>
        /// 水印文字是否加斜
        /// </summary>
        public Boolean FontItalic { get { return fontItalic; } set { fontItalic = value; } }
        /// <summary>
        /// 水印文字颜色
        /// </summary>
        private Color fontColor = Color.Red;
        /// <summary>
        /// 水印文字颜色
        /// </summary>
        public Color FontColor { get { return fontColor; } set { fontColor = value; } }
        /// <summary>
        /// 文字水印在图的横坐标
        /// </summary>
        private float fontPositionX = 10;
        /// <summary>
        /// 文字水印在图的横坐标
        /// </summary>
        public float FontPositionX { get { return fontPositionX; } set { fontPositionX = value; } }
        /// <summary>
        /// 文字水印在图的纵坐标
        /// </summary>
        private float fontPositionY = 10;
        /// <summary>
        /// 文字水印在图的纵坐标
        /// </summary>
        public float FontPositionY { get { return fontPositionY; } set { fontPositionY = value; } }
        /// <summary>
        /// 打水印位置 0:正中间 1:左上角 2:右上角 3:左下角 4:右下角 
        /// </summary>
        private int textAlign = 4;
        /// <summary>
        /// 打水印位置 0:正中间 1:左上角 2:右上角 3:左下角 4:右下角 
        /// </summary>
        public int TextAlign { get { return textAlign; } set { textAlign = value; } }
        /// <summary>
        /// 水印图片地址
        /// </summary>
        private string waterImagePath = "";
        /// <summary>
        /// 水印图片地址
        /// </summary>
        public string WaterImagePath { get { return waterImagePath; } set { waterImagePath = value; } }
        /// <summary>
        /// 打水印的图片地址
        /// </summary>
        private string imagePath = "";
        /// <summary>
        /// 打水印的图片地址
        /// </summary>
        public string ImagePath { get { return imagePath; } set { imagePath = value; } }
        /// <summary>
        /// 保存图片地址
        /// </summary>
        private string newImagePath = "";
        /// <summary>
        /// 保存图片地址
        /// </summary>
        public string NewImagePath { get { return newImagePath; } set { newImagePath = value; } }
        #endregion
    }
    #endregion

    #region 缩略图操作类
    /// <summary>
    /// 缩略图操作类
    /// </summary>
    public class ImageThumbnail
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ImageThumbnail() { }

        #region 方法

        #region MakeMyThumbPhoto切割后生成缩略图
        /// <summary>
        /// 切割后生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="toW">缩略图最终宽度</param>
        /// <param name="toH">缩略图最终高度</param>
        /// <param name="X">X坐标（zoom为1时）</param>
        /// <param name="Y">Y坐标（zoom为1时）</param>
        public static void MakeMyThumbPhoto(string originalImagePath, string thumbnailPath, int toW, int toH, int X, int Y)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
            int towidth = toW;
            int toheight = toH;
            int x = X;
            int y = Y;
            Bitmap BitMap = new Bitmap(towidth, toheight);
            BitMap.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);
            System.Drawing.Image bitmap = BitMap;
            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
            new Rectangle(x, y, towidth, toheight),
            GraphicsUnit.Pixel);
            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
            catch (Exception e)
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
                LogHelper.Error(e);
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        /// <summary>
        /// 切割后生成缩略图
        /// </summary>
        public void MakeMyThumbPhoto()
        {
            MakeMyThumbPhoto(this.SourcePath, this.TargetPath, this.Width, this.Height, this.PosX, this.PosY);
        }
        #endregion

        #region MakeThumbnail生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图模式 指定字符串"WH"指定宽高缩放（可能变形）,"W"指定宽缩放,"H"指定高缩放,"Cut"指定高宽裁减（不变形）,"EQ" 指定宽高等比例缩放(不变形)</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case "WH"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                case "EQ"://指定宽高等比例缩放(不变形)
                    if (width > ow && height > oh)
                    {
                        towidth = ow; toheight = oh;
                    }
                    else if (ow * height > width * oh)
                    {
                        towidth = width; toheight = oh * width / ow;
                    }
                    else
                    {
                        towidth = ow * height / oh;
                        toheight = height;
                    }
                    break;
                default:
                    break;
            }
            //新建一个bmp图片
            Bitmap BitMap = new Bitmap(towidth, toheight);
            BitMap.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);
            System.Drawing.Image bitmap = BitMap;
            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
            catch (Exception e)
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
                LogHelper.Error(e);
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        public void MakeThumbnail()
        {
            MakeThumbnail(this.SourcePath, this.TargetPath, this.Width, this.Height, this.ModeType);
        }
        #endregion

        #endregion

        #region 属性
        /// <summary>
        /// 源图片路径
        /// </summary>
        private string _SourcePath = "";
        /// <summary>
        /// 源图片路径
        /// </summary>
        public string SourcePath { get { return _SourcePath; } set { _SourcePath = value; } }
        /// <summary>
        /// 目标路径
        /// </summary>
        private string _TargetPath = "";
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get { return _TargetPath; } set { _TargetPath = value;} }
        /// <summary>
        /// 宽
        /// </summary>
        private int _Width = 100;
        /// <summary>
        /// 宽
        /// </summary>
        public int Width { get { return _Width; } set { _Width = value; } }
        /// <summary>
        /// 高
        /// </summary>
        private int _Height = 100;
        /// <summary>
        /// 高
        /// </summary>
        public int Height { get { return _Height; } set { _Height = value; } }
        /// <summary>
        /// 模式 指定字符串"WH"指定宽高缩放（可能变形）,"W"指定宽缩放,"H"指定高缩放,"Cut"指定高宽裁减（不变形）,"EQ" 指定宽高等比例缩放(不变形)
        /// </summary>
        private string _ModeType = "";
        /// <summary>
        /// 模式 指定字符串"WH"指定宽高缩放（可能变形）,"W"指定宽缩放,"H"指定高缩放,"Cut"指定高宽裁减（不变形）,"EQ" 指定宽高等比例缩放(不变形)
        /// </summary>
        public string ModeType { get { return _ModeType; } set { _ModeType = value; } }
        /// <summary>
        /// X坐标
        /// </summary>
        private int _PosX = 0;
        /// <summary>
        /// X坐标
        /// </summary>
        public int PosX { get { return _PosX; } set { _PosX = value; } }
        /// <summary>
        /// Y坐标
        /// </summary>
        private int _PosY = 0;
        /// <summary>
        /// Y坐标
        /// </summary>
        public int PosY { get { return _PosY; } set { _PosY = value; } }
        #endregion
    }
#endregion
}
