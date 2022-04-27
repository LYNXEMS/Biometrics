using System.Drawing;

namespace Biometrics
{
    public class Minutia
    {
        public static List<List<int>> findCheckPoint(bool[,] bitmap)
        {
            int x = bitmap.GetLength(0);
            int y = bitmap.GetLength(1);
            List<int> branchPointX = new List<int>();
            List<int> branchPointY = new List<int>();
            List<int> endPointX = new List<int>();
            List<int> endPointY = new List<int>();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    int t;
                    if (bitmap[i, j])
                    {
                        t = checkThisPoint(bitmap, i, j);
                        if (t == 1)
                        {
                            endPointX.Add(i);
                            endPointY.Add(j);
                        }
                        if (t >= 3)
                        {
                            branchPointX.Add(i);
                            branchPointY.Add(j);
                        }
                    }
                }
            }
            ClearTooClose(endPointX, endPointY);
            ClearTooClose(branchPointX, branchPointY);
            List<List<int>> r = new List<List<int>>();
            r.Add(endPointX);
            r.Add(endPointY);
            r.Add(branchPointX);
            r.Add(branchPointY);
            return r;
        }

        private static int checkThisPoint(bool[,] bitmap, int x, int y)
        {
            int count = 0;
            int topy = y + 1;
            int midy = y;
            int boty = y - 1;
            int leftx = x - 1;
            int midx = x;
            int rightx = x + 1;

            if (bitmap[leftx, topy] != bitmap[midx, topy]) count++;
            if (bitmap[midx, topy] != bitmap[rightx, topy]) count++;
            if (bitmap[rightx, topy] != bitmap[rightx, midy]) count++;
            if (bitmap[rightx, midy] != bitmap[rightx, boty]) count++;
            if (bitmap[rightx, boty] != bitmap[midx, boty]) count++;
            if (bitmap[midx, boty] != bitmap[leftx, boty]) count++;
            if (bitmap[leftx, boty] != bitmap[leftx, midy]) count++;
            if (bitmap[leftx, midy] != bitmap[leftx, topy]) count++;

            return count / 2;
        }

        public static Bitmap PrintMinutia(Bitmap image, List<List<int>> r)
        {
            var newImage = new Bitmap(image);
            for (int i = 0; i < r[0].Count; i++)
            {
                newImage.SetPixel(r[1][i], r[0][i], Color.Green);
            }
            for (int i = 0; i < r[2].Count; i++)
            {
                newImage.SetPixel(r[3][i], r[2][i], Color.Red);
            }
            return newImage;
        }

        public static void ClearTooClose(List<int> x, List<int> y)
        {
            List<int> r = new List<int>();
            if (x.Count != y.Count) return;
            for (int i = 0; i < x.Count; i++)
            {
                if (r.Contains(i)) continue;
                for (int j = i + 1; j < x.Count; j++)
                {
                    if (Math.Abs(x[i]-x[j]) < 5 && Math.Abs(y[i] - y[j]) < 5)
                    {
                        if (!r.Contains(i)) r.Add(i);
                        if (!r.Contains(j)) r.Add(j);
                    }
                }
            }
            r.Sort();
            r.Reverse();
            foreach (var item in r)
            {
                x.RemoveAt(item);
                y.RemoveAt(item);
            }
        }
    }
}
