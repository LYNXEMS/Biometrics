namespace Biometrics.Audio;

public static class DynamicTimeWarping
{
    public static double Distance(IEnumerable<double[]> first, IEnumerable<double[]> second)
        => Distance(first.SelectMany(x => x).ToArray(), second.SelectMany(x => x).ToArray());

    public static double Distance(IList<double> first, IList<double> second)
    {
        var n = first.Count + 1;
        var m = second.Count + 1;

        var dtw = new double[n, m];

        for (var i = 0; i < n; i++)
            for (var j = 0; j < m; j++)
                dtw[i, j] = double.PositiveInfinity;
        dtw[0, 0] = 0;

        for (var i = 1; i < n; i++)
            for (var j = 1; j < m; j++)
            {
                var cost = Math.Abs(first[i - 1] - second[j - 1]);
                dtw[i, j] = cost + Math.Min(dtw[i - 1, j - 1], Math.Min(dtw[i - 1, j], dtw[i, j - 1]));
            }

        return Math.Sqrt(dtw[n - 1, m - 1]);
    }
}