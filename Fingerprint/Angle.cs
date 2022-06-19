using System.Drawing;

namespace Biometrics;

public static class Angle
{
    public static double GetMinutiaAngle(bool[,] bitmap, Minutia minutia) => GetMinutiaAngle(bitmap, minutia.X, minutia.Y, minutia.Type);
    
    public static double GetMinutiaAngle(bool[,] bitmap, int x, int y, MinutiaType type)
    {
        return type switch
        {
            MinutiaType.EndPoint => GetEndPointAngle(bitmap, x, y),
            MinutiaType.BranchPoint => GetBranchPointAngle(bitmap, x, y),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static double GetEndPointAngle(bool[,] bitmap, int x, int y)
    {
        List<Point> checkedPoints = new() { new Point(x, y) };

        for (int i = 0; i < 10; i++)
        {
            foreach (var neighbour in Helpers.GetNeighbours(bitmap, checkedPoints[^1].X, checkedPoints[^1].Y))
            {
                if (!checkedPoints.Contains(neighbour))
                    checkedPoints.Add(neighbour);
            }
        }

        if (checkedPoints.Count < 10)
            return double.NaN;

        return ComputeAngle(checkedPoints[^1].X - checkedPoints[0].X, checkedPoints[^1].Y - checkedPoints[0].Y);
    }

    private static double GetBranchPointAngle(bool[,] bitmap, int x, int y)
    {
        List<Point> treeNeighbours = Helpers.GetNeighbours(bitmap, x, y).ToList();

        if (treeNeighbours.Count < 3)
            return double.NaN;

        List<Point> branch1 = new List<Point> { new(x, y), treeNeighbours[0] };
        List<Point> branch2 = new List<Point> { new(x, y), treeNeighbours[1] };
        List<Point> branch3 = new List<Point> { new(x, y), treeNeighbours[2] };

        for (int i = 0; i < 10; i++)
        {
            foreach (var neighbor in Helpers.GetNeighbours(bitmap, branch1[^1].X, branch1[^1].Y))
                if (!branch1.Contains(neighbor) && !treeNeighbours.Contains(neighbor))
                    branch1.Add(neighbor);
            
            foreach (var neighbor in Helpers.GetNeighbours(bitmap, branch2[^1].X, branch2[^1].Y))
                if (!branch2.Contains(neighbor) && !treeNeighbours.Contains(neighbor))
                    branch2.Add(neighbor);
            
            foreach (var neighbor in Helpers.GetNeighbours(bitmap, branch3[^1].X, branch3[^1].Y))
                if (!branch3.Contains(neighbor) && !treeNeighbours.Contains(neighbor))
                    branch3.Add(neighbor);
        }

        if (branch1.Count < 10 || branch2.Count < 10 || branch3.Count < 10)
            return double.NaN;
        
        double angle1 = ComputeAngle(branch1[^1].X - branch1[0].X, branch1[^1].Y - branch1[0].Y);
        double angle2 = ComputeAngle(branch2[^1].X - branch2[0].X, branch2[^1].Y - branch2[0].Y);
        double angle3 = ComputeAngle(branch3[^1].X - branch3[0].X, branch3[^1].Y - branch3[0].Y);

        double angleDifference1 = DifferencePi(angle1, angle2);
        double angleDifference2 = DifferencePi(angle1, angle3);
        double angleDifference3 = DifferencePi(angle2, angle3);

        double angle;
        
        if (angleDifference1 <= angleDifference2 && angleDifference1 <= angleDifference3)
        {
            angle = angle2 + angleDifference1 / 2;
            if (angle > 2 * Math.PI)
                angle -= 2 * Math.PI;
        }
        else if (angleDifference2 <= angleDifference1 && angleDifference2 <= angleDifference3)
        {
            angle = angle1 + angleDifference2 / 2;
            if (angle > 2 * Math.PI)
                angle -= 2 * Math.PI;
        }
        else
        {
            angle = angle3 + angleDifference3 / 2;
            if (angle > 2 * Math.PI)
                angle -= 2 * Math.PI;
        }

        return angle;
    }
    
    public static double ComputeAngle(double dX, double dY) =>
        dX switch
        {
            > 0 when dY >= 0 => Math.Atan(dY / dX),
            > 0 when dY < 0 => Math.Atan(dY / dX) + 2 * Math.PI,
            < 0 => Math.Atan(dY / dX) + Math.PI,
            0 when dY > 0 => Math.PI / 2,
            0 when dY < 0 => 3 * Math.PI / 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    
    public static double DifferencePi(double alpha, double beta)
    {
        double diff = Math.Abs(beta - alpha);
        return Math.Min(diff, 2 * Math.PI - diff);
    }
    
    public static double Difference2Pi(double alpha, double beta)
    {
        if (beta >= alpha)
            return (beta - alpha);
        return beta - alpha + 2 * Math.PI;
    }
}