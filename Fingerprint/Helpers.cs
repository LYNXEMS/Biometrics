using System.Drawing;

namespace Biometrics;

public static class Helpers
{

    public static Bitmap CreateNonIndexedImage(Image src)
    {
        Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        using (Graphics gfx = Graphics.FromImage(newBmp))
        {
            gfx.DrawImage(src, 0, 0);
        }

        return newBmp;
    }

    public static Bitmap Binarization(Bitmap sourceBitmap)
    {
        Bitmap targetBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
        for (int y = 0; y < sourceBitmap.Height; ++y)
            for (int x = 0; x < sourceBitmap.Width; ++x)
            {
                Color c = sourceBitmap.GetPixel(x, y);
                byte rgb = (byte)(0.33 * c.R + 0.33 * c.G + 0.33 * c.B);
                if (rgb > 220) targetBitmap.SetPixel(x, y, Color.White);
                else targetBitmap.SetPixel(x, y, Color.Black);

            }
        return targetBitmap;
    }

    public static Bitmap? mixTwoBitmaps(Bitmap bitmap, Bitmap bitmap1)
    {
        if (bitmap.Width != bitmap1.Width || bitmap.Height != bitmap1.Height)
            return null;
        else
        {
            int theWidth = bitmap.Width;
            int theHeight = bitmap.Height;
            Bitmap bitmap2 = new Bitmap(theWidth, theHeight);

            for (int x = 0; x < theWidth; x++)
            {
                for (int y = 0; y < theHeight; y++)
                {
                    int R = (int)(0.5f * bitmap.GetPixel(x, y).R + 0.5f * bitmap1.GetPixel(x, y).R);
                    int G = (int)(0.5f * bitmap.GetPixel(x, y).G + 0.5f * bitmap1.GetPixel(x, y).G);
                    int B = (int)(0.5f * bitmap.GetPixel(x, y).B + 0.5f * bitmap1.GetPixel(x, y).B);
                    Color color = Color.FromArgb(0, R, G, B);
                    bitmap2.SetPixel(x, y, color);
                }
            }
            return bitmap2;
        }
    }

    public static bool[,] Image2Bool(Image img)
    {
        Bitmap bmp = new Bitmap(img);
        bool[,] s = new bool[bmp.Height, bmp.Width];
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
                s[y, x] = bmp.GetPixel(x, y).GetBrightness() < 0.3;
        }
        return s;

    }

    public static Image Bool2Image(bool[,] s)
    {
        Bitmap bmp = new Bitmap(s.GetLength(1), s.GetLength(0));
        using (Graphics g = Graphics.FromImage(bmp)) g.Clear(Color.White);
        for (int y = 0; y < bmp.Height; y++)
            for (int x = 0; x < bmp.Width; x++)
                if (s[y, x]) bmp.SetPixel(x, y, Color.Black);

        return (Bitmap)bmp;
    }

    public static T[][] ArrayClone<T>(T[][] A)
    { return A.Select(a => a.ToArray()).ToArray(); }

    public static IEnumerable<Point> GetNeighbours(bool[,] bitmap, int x, int y)
    {
        if (bitmap[x + 1, y]) yield return new Point(x + 1, y);
        if (bitmap[x + 1, y - 1]) yield return new Point(x + 1, y - 1);
        if (bitmap[x, y - 1]) yield return new Point(x, y - 1);
        if (bitmap[x - 1, y - 1]) yield return new Point(x - 1, y - 1);
        if (bitmap[x - 1, y]) yield return new Point(x - 1, y);
        if (bitmap[x - 1, y + 1]) yield return new Point(x - 1, y + 1);
        if (bitmap[x, y + 1]) yield return new Point(x, y + 1);
        if (bitmap[x + 1, y + 1]) yield return new Point(x + 1, y + 1);
    }
}