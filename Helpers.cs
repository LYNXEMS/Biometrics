using System.Drawing;
using System.Collections;

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

        public static bool[][] Bitmap2Boolean(Image image)
        {
            Bitmap b = image.image;
            bool[][] array2D = new bool[b.Width][];
            for (int x = 0; x < b.Width; x++)
            {
                array2D[x] = new bool[b.Height];
                for (int y = 0; y < b.Height; y++)
                    array2D[x][y] = b.GetPixel(x, y).GetBrightness() < 0.3;
            }
            return array2D;
        }

        public static Bitmap Boolean2Bitmap(bool[][] array2D)
        {
            Bitmap b = new Bitmap(array2D[0].Length, array2D.Length);
            using (Graphics graphics = Graphics.FromImage(b)) graphics.Clear(White);
            for (int x = 0; x < b.Width; x++)
            {
                array2D[x] = new bool[b.Height];
                for (int y = 0; y < b.Height; y++)
                    if (array2D[x][y]) b.SetPixel(x, y, Black);
            }
            return (Bitmap)b;
        }

        public static Bitmap mixTwoBitmaps(Bitmap bitmap, Bitmap bitmap1)
        {
            if (bitmap.Width != bitmap1.Width || bitmap.Height != bitmap1.Height)
                return bitmap1;
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
                        Color color = Color.FromArgb(255, R, G, B);
                        bitmap2.SetPixel(x, y, color);
                    }
                }
                return bitmap2;
            }
        }
        public static int numberOfNeighbours(int pixelX, int pixelY, Image image)
        {
            Bitmap b = image.image;
            if ((pixelX == 0 && pixelY == 0) || (pixelX == b.Width - 1 && pixelY == 0) || (pixelX == 0 && pixelY == b.Height - 1) || (pixelX == b.Width - 1 && pixelY == b.Height - 1))
                return 3;
            else if ((pixelX > 0 && pixelX < b.Width && pixelY == 0) || (pixelX == 0 && pixelY > 0 && pixelY < b.Height) || (pixelX > 0 && pixelX < b.Width && pixelY == b.Height - 1) || (pixelX == b.Width - 1 && pixelY > 0 && pixelY < b.Height))
                return 5;
            else return 8;
        }

        private static int numberOfWhiteNeigbours(int x, int y, bool[][] array2D)
        {
            int count = 0;
            if (array2D[x - 1][y]) count++;
            if (array2D[x - 1][y + 1]) count++;
            if (array2D[x - 1][y - 1]) count++;
            if (array2D[x][y - 1]) count++;
            if (array2D[x][y + 1]) count++;
            if (array2D[x + 1][y + 1]) count++;
            if (array2D[x + 1][y]) count++;
            if (array2D[x + 1][y - 1]) count++;
            return count;
        }
        private static int numberOfZeroToOneTransitionFromPixel9(int x, int y, bool[][] array2D)
        {
            bool p2 = array2D[x][y - 1];
            bool p3 = array2D[x + 1][y - 1];
            bool p4 = array2D[x + 1][y];
            bool p5 = array2D[x + 1][y + 1];
            bool p6 = array2D[x][y + 1];
            bool p7 = array2D[x - 1][y + 1];
            bool p8 = array2D[x - 1][y];
            bool p9 = array2D[x - 1][y - 1];

            int finale = Convert.ToInt32((!p2 && p3)) + Convert.ToInt32((!p3 && p4)) +
                Convert.ToInt32((!p4 && p6)) + Convert.ToInt32((!p5 && p6)) +
                Convert.ToInt32((!p6 && p7)) + Convert.ToInt32((!p7 && p8)) +
                Convert.ToInt32((!p8 && p9)) + Convert.ToInt32((!p9 && p2));
            return finale;
        }
        private static bool ZhangSuenThinningAlgorithm(int x, int y, bool[][] array2D, bool even)
        {
            bool p2 = array2D[x][y - 1];
            bool p4 = array2D[x + 1][y];
            bool p6 = array2D[x][y + 1];
            bool p8 = array2D[x - 1][y];

            int bp1 = numberOfWhiteNeigbours(x, y, array2D);
            if (bp1 >= 2 && bp1 <= 6)
            {
                if (numberOfZeroToOneTransitionFromPixel9(x, y, array2D) == 1)
                    if (even)
                        if (!((p2 && p4) && p8))
                            if (!((p2 && p6) && p8))
                                return true;
                            else
                            {
                                if (!((p2 && p4) && p6))
                                    if (!((p4 && p6) && p8))
                                        return true;
                            }
            }
            return false;
        }

        private static int step(int stepNumber, bool[][] tmp, bool[][] array2D)
        {
            int count = 0;
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                for (int j = 0; j < tmp[0].Length - 1; j++)
                    if (ZhangSuenThinningAlgorithm(i, j, tmp, stepNumber == 2))
                        if (array2D[i][j]) count++;
                        else array2D[i][j] = false;
            }
            return count;
        }

        public static T[][] ArrayClone<T>(T[][] array) => array.Select(i => i.ToArray()).ToArray();

        public static bool[][] ZhangSuenThinning(bool[][] array2D)
        {
            bool[][] tmp = ArrayClone(array2D);
            int count = 0;
            do
            {
                count = step(1, tmp, array2D);
                tmp = ArrayClone(array2D);
                count += step(2, tmp, array2D);
                tmp = ArrayClone(array2D);
            }
            while (count > 0);
            return array2D;
        }

        public static Color substract(Color c, Color c1) => Color.FromArgb(Math.Abs(c.R - c1.R), Math.Abs(c.G - c1.G), Math.Abs(c.B - c1.B));
        public static Color add(Color c, Color c1) => Color.FromArgb(c.R + c1.R, c.G + c1.G, c.B + c1.B);
        public static Color add(Color c, Color c1, Color c2, Color c3) => add(add(c, c1), add(c2, c3));
        public static Color add(Color c, Color c1, Color c2, Color c3, Color c4, Color c5, Color c6, Color c7)
        {
            return add(add(c, c1, c2, c3), add(c4, c5, c6, c7));
        }
        public static Color add(Color c, Color c1, Color c2, Color c3, Color c4, Color c5, Color c6, Color c7, Color c8, Color c9, Color c10, Color c11)
        {
            return add(add(c, c1, c2, c3, c4, c5, c6, c7), add(c8, c9, c10, c11));
        }

        /*not completed
        public static double crossingNumberOfPixelAt(int x, int y, Image image)
        {
            Bitmap b = image.image;
            Color main = b.GetPixel(x, y);
            Color leftPixel = b.GetPixel(x - 1, y);
            Color rightPixel = b.GetPixel(x + 1, y);
            Color lowerPixel = b.GetPixel(x, y + 1);
            Color upperPixel = b.GetPixel(x, y - 1);
            Color lowerLeftCornerPixel = b.GetPixel(x - 1, y + 1);
            Color lowerRightCornerPixel = b.GetPixel(x + 1, y + 1);
            Color upperLeftCornerPixel = b.GetPixel(x - 1, y - 1);
            Color upperRightCornerPixel = b.GetPixel(x + 1, y - 1);

            Color d = substract(upperLeftCornerPixel,upperPixel);
            Color d1 = substract(upperPixel, upperRightCornerPixel);
            Color d2 = substract(leftPixel, upperLeftCornerPixel);
            Color d3 = substract(rightPixel, upperRightCornerPixel);
            Color d4 = substract(leftPixel, lowerLeftCornerPixel);
            Color d5 = substract(rightPixel, lowerRightCornerPixel);
            Color d6 = substract(lowerPixel, lowerLeftCornerPixel);
            Color d7 = substract(lowerPixel, lowerRightCornerPixel);
            Color d8 = substract(main, leftPixel);
            Color d9 = substract(main, rightPixel);
            Color d10 = substract(main, upperPixel);
            Color d11 = substract(main, lowerPixel);
            Color finale = add(d,d1,d2,d3,d4,d5,d6,d7,d8,d9,d10,d11);
            return -12;
        }
        */
    }
}
