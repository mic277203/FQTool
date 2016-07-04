using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FOToolHelper
{
    public static class ImageHelper
    {
        /// <summary>
        /// 判断图像是否存在
        /// </summary>
        /// <param name="template"></param>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static bool ContainsImg(this Bitmap template, Bitmap bmp)
        {
            // create template matching algorithm's instance // (set similarity threshold to 92.1%) 
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.921f); // find all matchings with specified above similarity 
            TemplateMatch[] matchings = tm.ProcessImage(template, bmp); // highlight found matchings

            return matchings.Length > 0;
        }
        /// <summary>
        /// 判断图像是否存在另外的图像中，并返回坐标
        /// </summary>
        /// <param name="template"></param>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Point ContainsGetPoint(this Bitmap template, Bitmap bmp)
        {
            // create template matching algorithm's instance // (set similarity threshold to 92.1%) 
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.921f); // find all matchings with specified above similarity 
            TemplateMatch[] matchings = tm.ProcessImage(template, bmp); // highlight found matchings
            BitmapData data = template.LockBits(new Rectangle(0, 0, template.Width, template.Height), ImageLockMode.ReadWrite, template.PixelFormat);
            Point p = new Point();

            if (matchings.Length > 0)
            {
                Drawing.Rectangle(data, matchings[0].Rectangle, Color.White);
                p = matchings[0].Rectangle.Location;
                template.UnlockBits(data);
            }

            return p;
        }

        /// <summary>
        /// 图像赚Base字符串
        /// </summary>
        /// <returns></returns>
        public static string ToBaseMd5(this Bitmap img)
        {
            if (img == null)
                return string.Empty;
            else
                return Convert.ToBase64String(ToByte(img));
        }

        /// <summary>
        /// 图片转数组
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private static byte[] ToByte(Bitmap img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        public static Bitmap DealBlackAndWhite(Bitmap image)
        {
            //第一步 变灰度图
            image = ImageHelper.ToGray(image);
            //第二步 增加亮度100
            image = ImageHelper.KiLighten(image, 100);
            //第三步增加对比度100
            image = ImageHelper.KiContrast(image, 100);
            //第四步 变黑白
            image = ImageHelper.ToBlackWhite(image);
            //反向
            image = ImageHelper.ApplyInvert(image);

            return image;
        }

        /// <summary>
        /// 反像
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap ApplyInvert(Bitmap source)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(source.Width, source.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            // create the negative color matrix
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
        new float[] {-1, 0, 0, 0, 0},
        new float[] {0, -1, 0, 0, 0},
        new float[] {0, 0, -1, 0, 0},
        new float[] {0, 0, 0, 1, 0},
        new float[] {1, 1, 1, 0, 1}
            });

            // create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                        0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();

            return newBitmap;
        }
        /// <summary>
        /// 图片变成灰度
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap b)
        {
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    Color c = b.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);//转换灰度的算法
                    b.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }
            return b;
        }

        /// <summary>
        /// 图像变成黑白
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bitmap ToBlackWhite(Bitmap b)
        {
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    Color c = b.GetPixel(x, y);
                    if (c.R < (byte)255)
                    {
                        b.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            return b;
        }

        /// <summary>
        /// 图像亮度调整
        /// </summary>
        /// <param name="b"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Bitmap KiLighten(Bitmap b, int degree)
        {

            if (b == null)
            {

                return null;

            }

            if (degree < -255) degree = -255;

            if (degree > 255) degree = 255;

            try
            {

                int width = b.Width;

                int height = b.Height;

                int pix = 0;

                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)data.Scan0;

                    int offset = data.Stride - width * 3;

                    for (int y = 0; y < height; y++)
                    {

                        for (int x = 0; x < width; x++)
                        {

                            // 处理指定位置像素的亮度

                            for (int i = 0; i < 3; i++)
                            {

                                pix = p[i] + degree;



                                if (degree < 0) p[i] = (byte)Math.Max(0, pix);

                                if (degree > 0) p[i] = (byte)Math.Min(255, pix);



                            } // i

                            p += 3;

                        } // x

                        p += offset;

                    } // y

                }



                b.UnlockBits(data);



                return b;

            }

            catch
            {

                return null;

            }



        }

        /// <summary>
        /// 图像对比度调整
        /// </summary>
        /// <param name="b">原始图</param>
        /// <param name="degree">对比度[-100, 100]</param>
        /// <returns></returns>

        public static Bitmap KiContrast(Bitmap b, int degree)
        {

            if (b == null)
            {

                return null;

            }



            if (degree < -100) degree = -100;

            if (degree > 100) degree = 100;



            try
            {



                double pixel = 0;

                double contrast = (100.0 + degree) / 100.0;

                contrast *= contrast;

                int width = b.Width;

                int height = b.Height;

                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {

                    byte* p = (byte*)data.Scan0;

                    int offset = data.Stride - width * 3;

                    for (int y = 0; y < height; y++)
                    {

                        for (int x = 0; x < width; x++)
                        {

                            // 处理指定位置像素的对比度

                            for (int i = 0; i < 3; i++)
                            {

                                pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;

                                if (pixel < 0) pixel = 0;

                                if (pixel > 255) pixel = 255;

                                p[i] = (byte)pixel;

                            } // i

                            p += 3;

                        } // x

                        p += offset;

                    } // y
                }
                b.UnlockBits(data);
                return b;
            }
            catch
            {
                return null;
            }
        }
    }
}
