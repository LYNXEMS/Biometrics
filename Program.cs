using System.Drawing;

Console.WriteLine("Hello, World!");

Bitmap tImage = new Bitmap("spectrum.bmp");

Color White = Color.FromArgb(255, 255, 255);
Color Black = Color.FromArgb(0, 0, 0);

void Greyscale(Bitmap tImage)
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

void Binarize(Bitmap tImage)
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

// Save
tImage.Save("spectrum2.bmp");
