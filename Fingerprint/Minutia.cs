using System.Drawing;
using System.Dynamic;

namespace Biometrics;

public enum MinutiaType
{
    Unknown,
    EndPoint,
    BranchPoint
}

public class Minutia
{
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public double Angle { get; protected set; }
    public MinutiaType Type { get; protected set; }

    public Minutia(int x, int y, MinutiaType type = MinutiaType.Unknown, double angle = double.NaN)
    {
        X = x;
        Y = y;
        Type = type;
        Angle = angle;
    }

    public double Distance(Minutia other) => Math.Sqrt(DistanceSquared(other));

    public double DistanceSquared(Minutia other)
    {
        var distX = X - other.X;
        var distY = Y - other.Y;
        
        return (distX * distX) + (distY * distY);
    }
    
    public static List<Minutia> findCheckPoint(bool[,] bitmap)
    {
        List<Minutia> result = new();
        int x = bitmap.GetLength(0);
        int y = bitmap.GetLength(1);
        
        for (int i = 1; i < x - 1; i++)
        {
            for (int j = 1; j < y - 1; j++)
            {
                if (bitmap[i, j])
                {
                    MinutiaType type = checkThisPoint(bitmap, i, j) switch
                    {
                        1 => MinutiaType.EndPoint,
                        3 => MinutiaType.BranchPoint,
                        _ => MinutiaType.Unknown,
                    };

                    if (type != MinutiaType.Unknown) 
                        result.Add(new Minutia(i, j, type));
                }
            }
        }
        ClearTooClose(result, MinutiaType.BranchPoint);
        ClearTooClose(result, MinutiaType.EndPoint);

        foreach (var minutia in result) 
            minutia.Angle = Biometrics.Angle.GetMinutiaAngle(bitmap, minutia);
        result.RemoveAll(m => double.IsNaN(m.Angle));

        return result;
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

    public static Bitmap PrintMinutia(Bitmap image, List<Minutia> r)
    {
        var newImage = new Bitmap(image);

        foreach (var minutia in r)
        {
            Color color = minutia.Type switch
            {
                MinutiaType.BranchPoint => Color.Red,
                MinutiaType.EndPoint => Color.Green,
                _ => throw new Exception($"{nameof(Minutia)}.{nameof(minutia.Type)} with value {MinutiaType.Unknown} is not supported")
            };
            
            newImage.SetPixel(minutia.Y, minutia.X, color);
        }

        return newImage;
    }

    public static void ClearTooClose(List<Minutia> list, MinutiaType typeToClear = MinutiaType.Unknown)
    {
        List<int> r = new List<int>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Type == MinutiaType.Unknown) continue;
            if (typeToClear != MinutiaType.Unknown && list[i].Type != typeToClear) continue;
            if (r.Contains(i)) continue;
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[j].Type == MinutiaType.Unknown) continue;
                if (typeToClear != MinutiaType.Unknown && list[i].Type != typeToClear) continue;
                if (list[i].DistanceSquared(list[j]) < 25)
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
            list.RemoveAt(item);
        }
    }
}