using System.Drawing;
using System.Collections;

namespace Biometrics
{
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

        public static Bitmap mixTwoBitmaps(Bitmap bitmap, Bitmap bitmap1)
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
        public static bool[][] Image2Bool(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            bool[][] s = new bool[bmp.Height][];
            for (int y = 0; y < bmp.Height; y++)
            {
                s[y] = new bool[bmp.Width];
                for (int x = 0; x < bmp.Width; x++)
                    s[y][x] = bmp.GetPixel(x, y).GetBrightness() < 0.3;
            }
            return s;

        }

        public static Image Bool2Image(bool[][] s)
        {
            Bitmap bmp = new Bitmap(s[0].Length, s.Length);
            using (Graphics g = Graphics.FromImage(bmp)) g.Clear(Color.White);
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    if (s[y][x]) bmp.SetPixel(x, y, Color.Black);

            return (Bitmap)bmp;
        }
        public static T[][] ArrayClone<T>(T[][] A)
        { return A.Select(a => a.ToArray()).ToArray(); }

        public static bool[][] ZhangSuenThinning(bool[][] s)
        {
            bool[][] temp = ArrayClone(s);
            int count = 0;
            do
            {
                count = step(1, temp, s);
                temp = ArrayClone(s);
                count += step(2, temp, s);
                temp = ArrayClone(s);
            }
            while (count > 0);
            return s;
        }

        static int step(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (SuenThinningAlg(a, b, temp, stepNo == 2))
                    {
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        static bool SuenThinningAlg(int x, int y, bool[][] s, bool even)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];


            int bp1 = NumberOfNonZeroNeighbors(x, y, s);
            if (bp1 >= 2 && bp1 <= 6)
            {
                if (NumberOfZeroToOneTransitionFromP9(x, y, s) == 1)
                {
                    if (even)
                    {
                        if (!((p2 && p4) && p8))
                        {
                            if (!((p2 && p6) && p8))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (!((p2 && p4) && p6))
                        {
                            if (!((p4 && p6) && p8))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        static int NumberOfZeroToOneTransitionFromP9(int x, int y, bool[][] s)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];

            int A = Convert.ToInt32((!p2 && p3)) + Convert.ToInt32((!p3 && p4)) +
                    Convert.ToInt32((!p4 && p5)) + Convert.ToInt32((!p5 && p6)) +
                    Convert.ToInt32((!p6 && p7)) + Convert.ToInt32((!p7 && p8)) +
                    Convert.ToInt32((!p8 && p9)) + Convert.ToInt32((!p9 && p2));
            return A;
        }
        static int NumberOfNonZeroNeighbors(int x, int y, bool[][] s)
        {
            int count = 0;
            if (s[x - 1][y]) count++;
            if (s[x - 1][y + 1]) count++;
            if (s[x - 1][y - 1]) count++;
            if (s[x][y + 1]) count++;
            if (s[x][y - 1]) count++;
            if (s[x + 1][y]) count++;
            if (s[x + 1][y + 1]) count++;
            if (s[x + 1][y - 1]) count++;
            return count;
        }
    }
}
