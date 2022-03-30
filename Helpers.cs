using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometrics
{
    internal class Helpers
    {
		static Color White = Color.FromArgb(255, 255, 255);
		static Color Black = Color.FromArgb(0, 0, 0);

		public static void Binarize(Bitmap tImage)
		{
			{
				for (int x = 0; x < tImage.Width; x++)
				{
					for (int y = 0; y < tImage.Height; y++)
					{
						Color tCol = tImage.GetPixel(x, y);
						if (tCol.R > 127) tImage.SetPixel(x, y, White);
						else tImage.SetPixel(x, y, Black);
					}
				}
			}
		}

		public static void Greyscale(Bitmap tImage)
		{
			for (int x = 0; x < tImage.Width; x++)
			{
				for (int y = 0; y < tImage.Height; y++)
				{
					Color tCol = tImage.GetPixel(x, y);

					// L = 0.2126·R + 0.7152·G + 0.0722·B 
					double L = 0.2126 * tCol.R + 0.7152 * tCol.G + 0.0722 * tCol.B;
					tImage.SetPixel(x, y, Color.FromArgb(Convert.ToInt32(L), Convert.ToInt32(L), Convert.ToInt32(L)));
				}
			}
		}

		public static Bitmap CreateNonIndexedImage(System.Drawing.Image src)
		{
			Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			using (Graphics gfx = Graphics.FromImage(newBmp))
			{
				gfx.DrawImage(src, 0, 0);
			}

			return newBmp;
		}
	}
}
