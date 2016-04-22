/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 关于图片处理的一些实用方法
	/// </summary>
	public static class ImageHelper
	{
		/// <summary>
		/// 制作图片的缩略图
		/// </summary>
		/// <param name="originalImage">原图</param>
		/// <param name="width">缩略图的宽（像素）</param>
		/// <param name="height">缩略图的高（像素）</param>
		/// <param name="mode">缩略方式</param>
		/// <returns>缩略图</returns>
		/// <remarks>
		///		<paramref name="mode"/>：
		///			<para>HW：指定的高宽缩放（可能变形）</para>
		///			<para>HWO：指定高宽缩放（可能变形）（过小则不变）</para>
		///			<para>W：指定宽，高按比例</para>
		///			<para>WO：指定宽（过小则不变），高按比例</para>
		///			<para>H：指定高，宽按比例</para>
		///			<para>HO：指定高（过小则不变），宽按比例</para>
		///			<para>CUT：指定高宽裁减（不变形）</para>
		/// </remarks>
		public static Image MakeThumbnail(Image originalImage, int width, int height, string mode) {
			var toWidth = width;
			var toHeight = height;

			var x = 0;
			var y = 0;
			var originalWidth = originalImage.Width;
			var originalHeight = originalImage.Height;

			mode = string.IsNullOrEmpty(mode) ? "HW" : mode.ToUpper();

			switch(mode) {
				case "HW": //指定高宽缩放（可能变形）
					break;
				case "HWO": //指定高宽缩放（可能变形）（过小则不变）
					if(originalImage.Width <= width && originalImage.Height <= height) {
						return originalImage;
					}
					if(originalImage.Width < width) {
						toWidth = originalImage.Width;
					}
					if(originalImage.Height < height) {
						toHeight = originalImage.Height;
					}
					break;
				case "W": //指定宽，高按比例
					toHeight = originalImage.Height*width/originalImage.Width;
					break;
				case "WO": //指定宽（过小则不变），高按比例
					if(originalImage.Width <= width) {
						return originalImage;
					}
					toHeight = originalImage.Height*width/originalImage.Width;
					break;
				case "H": //指定高，宽按比例
					toWidth = originalImage.Width*height/originalImage.Height;
					break;
				case "HO": //指定高（过小则不变），宽按比例
					if(originalImage.Height <= height) {
						return originalImage;
					}
					toWidth = originalImage.Width*height/originalImage.Height;
					break;
				case "CUT": //指定高宽裁减（不变形）
					if((double)originalImage.Width/(double)originalImage.Height > (double)toWidth/(double)toHeight) {
						originalHeight = originalImage.Height;
						originalWidth = originalImage.Height*toWidth/toHeight;
						y = 0;
						x = (originalImage.Width - originalWidth)/2;
					} else {
						originalWidth = originalImage.Width;
						originalHeight = originalImage.Width*height/toWidth;
						x = 0;
						y = (originalImage.Height - originalHeight)/2;
					}
					break;
				default:
					break;
			}

			return MakeThumbnail(originalImage, x, y, originalWidth, originalHeight, toWidth, toHeight);
		}

		/// <summary>
		/// 制作图片的剪切图
		/// </summary>
		/// <param name="originalImage">原图</param>
		/// <param name="x">需剪切的原图X坐标</param>
		/// <param name="y">需剪切的原图Y坐标</param>
		/// <param name="width">需剪切的长度</param>
		/// <param name="height">需剪切的高度</param>
		/// <param name="toWidth">新图的长度</param>
		/// <param name="toHeight">新图的高度</param>
		/// <returns>剪切图</returns>
		public static Image MakeThumbnail(Image originalImage, int x, int y, int width, int height, int toWidth, int toHeight) {
			//新建一个bmp图片
			var bitmap = new Bitmap(toWidth, toHeight);

			//新建一个画板
			using(var g = Graphics.FromImage(bitmap)) {
				g.CompositingQuality = CompositingQuality.HighQuality;

				//指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;

				//设置高质量,低速度呈现平滑程度
				g.SmoothingMode = SmoothingMode.HighQuality;

				//清空画布并以透明背景色填充
				g.Clear(Color.Transparent);

				//在指定位置并且按指定大小绘制原图片的指定部分
				g.DrawImage(originalImage, new Rectangle(0, 0, toWidth, toHeight),
				            new Rectangle(x, y, width, height),
				            GraphicsUnit.Pixel);
			}
			
			return bitmap;
		}

		/// <summary>
		/// 制作图片的缩略图
		/// </summary>
		/// <param name="originalStream">原图</param>
		/// <param name="thumbnailPath">保存缩略图的路径</param>
		/// <param name="width">缩略图的宽（像素）</param>
		/// <param name="height">缩略图的高（像素）</param>
		/// <param name="mode">缩略方式，参见<seealso cref="MakeThumbnail(Image, int, int, string)"/></param>
		public static void MakeThumbnail(Stream originalStream, string thumbnailPath, int width, int height, string mode) {
			using(var originalImage = Image.FromStream(originalStream)) {
				MakeThumbnail(originalImage, thumbnailPath, width, height, mode);
			}
		}

		/// <summary>
		/// 制作图片的缩略图
		/// </summary>
		/// <param name="originalImage">原图</param>
		/// <param name="thumbnailPath">保存缩略图的路径</param>
		/// <param name="width">缩略图的宽（像素）</param>
		/// <param name="height">缩略图的高（像素）</param>
		/// <param name="mode">缩略方式，参见<seealso cref="MakeThumbnail(Image, int, int, string)"/></param>
		public static void MakeThumbnail(Image originalImage, string thumbnailPath, int width, int height, string mode) {
			using(var bitmap = MakeThumbnail(originalImage, width, height, mode)) {
				//以jpg格式保存缩略图
				bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
			}
		}

		/// <summary>
		/// 制作图片的缩略图
		/// </summary>
		/// <param name="originalImagePath">原图的路径</param>
		/// <param name="thumbnailPath">保存缩略图的路径</param>
		/// <param name="width">缩略图的宽（像素）</param>
		/// <param name="height">缩略图的高（像素）</param>
		/// <param name="mode">缩略方式，参见<seealso cref="MakeThumbnail(Image, int, int, string)"/></param>
		public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode) {
			using(var originalImage = Image.FromFile(originalImagePath)) {
				MakeThumbnail(originalImage, width, height, mode);
			}
		}
	}
}