using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class ImageHelper
    {
		public static Image ShrinkImage(Image sourceImage, float scaleFactor)
		{
			int newWidth = Convert.ToInt32(sourceImage.Width * scaleFactor);
			int newHeight = Convert.ToInt32(sourceImage.Height * scaleFactor);

			newWidth = newWidth == 0 ? 1 : newWidth;
			newHeight = newHeight == 0 ? 1 : newHeight;

			var thumbnailBitmap = new Bitmap(newWidth, newHeight);
			using (Graphics g = Graphics.FromImage(thumbnailBitmap))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
				g.DrawImage(sourceImage, imageRectangle);
			}
			return thumbnailBitmap;
		}
	}
}
