namespace Biometrics;

// Original:
// X. Jiang and W. Y. Yau, "Fingerprint Minutiae Matching Based on the Local and Global Structures," in 15th International Conference on Pattern Recognition, Barcelona, Spain, 2000, pp. 1038-1041.
// Improved:
// Medina-Pérez, M.A., García-Borroto, M., Gutierrez-Rodríguez, A.E., Altamirano-Robles, L. (2012). Improving the Multiple Alignments Strategy for Fingerprint Verification. In: Carrasco-Ochoa, J.A., Martínez-Trinidad, J.F., Olvera López, J.A., Boyer, K.L. (eds) Pattern Recognition. MCPR 2012. Lecture Notes in Computer Science, vol 7329. Springer, Berlin, Heidelberg. https://doi.org/10.1007/978-3-642-31149-9_15

public static class Medina2012Matcher
{
    internal static double Match(PrintImage testImage, PrintImage originalImage, List<Minutia> test, List<Minutia> original, out List<MinutiaPair> matches)
    {
        matches = new List<MinutiaPair>();

        List<MedinaMinutiaDescriptor> testDescriptors = GetDescriptors(testImage, test);
        List<MedinaMinutiaDescriptor> originalDescriptors = GetDescriptors(originalImage, original);

        List<MinutiaPair> localMatchingMinutia = GetLocalMatchingMinutia(testDescriptors, originalDescriptors);
        if (!localMatchingMinutia.Any())
            return 0d;

        int max = 0;
        int unmatchedCount = int.MaxValue;

        for (int i = 0; i < localMatchingMinutia.Count; i++)
        {
            List<MinutiaPair> globalMatchingMinutia = GetGlobalMatchingMinutia(localMatchingMinutia, localMatchingMinutia[i], ref unmatchedCount);

            if (globalMatchingMinutia.Any() && globalMatchingMinutia.Count > max)
            {
                max = globalMatchingMinutia.Count;
                matches = globalMatchingMinutia;
            }
        }

        if (!matches.Any())
            return 0d;

        return (double)max / Math.Max(testDescriptors.Count, originalDescriptors.Count);
    }

    private static List<MedinaMinutiaDescriptor> GetDescriptors(PrintImage image, List<Minutia> minutiae)
    {
        List<MedinaMinutiaDescriptor> result = new();

        if (minutiae.Count < 3)
            return result;

        foreach (var minutia in minutiae)
        {
            (Minutia? nearest, Minutia? farthest) = GetClosest(minutiae, minutia);

            if (nearest != null && farthest != null)
                result.Add(new MedinaMinutiaDescriptor(minutia, nearest, farthest, image));
        }

        return result;
    }

    private static (Minutia? nearest, Minutia? farthest) GetClosest(List<Minutia> minutiae, Minutia query)
    {
        (double nearest, double farthest) distances = new(double.PositiveInfinity, double.PositiveInfinity);
        (Minutia? nearest, Minutia? farthest) result = new();

        foreach (var minutia in minutiae)
        {
            if (minutia == query)
                continue;

            double currentDistance = query.DistanceSquared(minutia);
            bool first = !(distances.farthest > distances.nearest);

            if (currentDistance < distances.farthest)
            {
                if (first)
                {
                    distances.nearest = currentDistance;
                    result.nearest = minutia;
                }
                else
                {
                    distances.farthest = currentDistance;
                    result.farthest = minutia;
                }
            }
        }

        return result;
    }

    private static List<MinutiaPair> GetLocalMatchingMinutia(List<MedinaMinutiaDescriptor> test, List<MedinaMinutiaDescriptor> original)
    {
        var triplets = new List<Triplet>(test.Count * original.Count);
        foreach (MedinaMinutiaDescriptor testMinutia in test)
        {
            foreach (MedinaMinutiaDescriptor originalMinutia in original)
            {
                double current = testMinutia.RotationInvariantMatch(originalMinutia);

                if (current == 0) continue;
                
                MinutiaPair mainMinutia = new(testMinutia.Main, originalMinutia.Main, current);
                MinutiaPair closestMinutia = new(testMinutia.Closest, originalMinutia.Closest, current);
                MinutiaPair farthestMinutia = new(testMinutia.Farthest, originalMinutia.Farthest, current);

                triplets.Add(new Triplet(mainMinutia, closestMinutia, farthestMinutia, current));
            }
        }

        triplets.Sort((x, y) =>
        {
            if (x == y || x.MatchValue == y.MatchValue)
                return 0;
            if (x.MatchValue < y.MatchValue)
                return 1;
            return -1;
        });
        
        var minutiaPairs = new List<MinutiaPair>(3 * triplets.Count);
        foreach (var triplet in triplets)
        {
            minutiaPairs.Add(triplet.MainMinutia);
            minutiaPairs.Add(triplet.NearestMinutia);
            minutiaPairs.Add(triplet.FarthestMinutia);
        }

        var testMatches = new Dictionary<Minutia, Minutia>(60);
        var originalMatches = new Dictionary<Minutia, Minutia>(60);
        var matchingPairs = new List<MinutiaPair>(60);
        
        foreach (var pair in minutiaPairs)
        {
            if (testMatches.ContainsKey(pair.Test) && originalMatches.ContainsKey(pair.Original)) continue;
            
            matchingPairs.Add(pair);
            if (!testMatches.ContainsKey(pair.Test))
                testMatches.Add(pair.Test, pair.Original);
            if (!originalMatches.ContainsKey(pair.Original))
                originalMatches.Add(pair.Original, pair.Test);
        }

        return matchingPairs;
    }

    private static List<MinutiaPair> GetGlobalMatchingMinutia(List<MinutiaPair> localMatchingPairs, MinutiaPair referencePair, ref int unmatchedCount)
    {
        List<MinutiaPair> globalMatchingMinutia = new(localMatchingPairs.Count);
        Dictionary<Minutia, Minutia> testMatches = new(localMatchingPairs.Count);
        Dictionary<Minutia, Minutia> originalMatches = new(localMatchingPairs.Count);
        testMatches.Add(referencePair.Test, referencePair.Original);
        originalMatches.Add(referencePair.Original, referencePair.Test);

        MinutiaMapper mm = new(referencePair.Test, referencePair.Original);
        Minutia referenceTest = mm.Map(referencePair.Test);
        Minutia referenceOriginal = referencePair.Original;
        int currentUnmatchedCount = 0;
        int i;
        
        for (i = 0; i < localMatchingPairs.Count; i++)
        {
            MinutiaPair pair = localMatchingPairs[i];
            if (!testMatches.ContainsKey(pair.Test) && !originalMatches.ContainsKey(pair.Original))
            {
                Minutia test = mm.Map(pair.Test);
                Minutia original = pair.Original;
                
                if (MatchDistance(referenceTest, referenceOriginal, test, original) && MatchDirections(test, original) && MatchPosition(referenceTest, referenceOriginal, test, original))
                {
                    globalMatchingMinutia.Add(pair);
                    testMatches.Add(pair.Test, pair.Original);
                    originalMatches.Add(pair.Original, pair.Test);
                }
                else
                    currentUnmatchedCount++;
            }

            if (currentUnmatchedCount >= unmatchedCount)
                break;
        }

        if (i == localMatchingPairs.Count)
        {
            unmatchedCount = currentUnmatchedCount;
            globalMatchingMinutia.Add(referencePair);
            return globalMatchingMinutia;
        }

        return globalMatchingMinutia;
    }

    private static bool MatchDistance(Minutia refTest, Minutia refOriginal, Minutia test, Minutia original)
    {
        double d0 = refTest.Distance(test);
        double d1 = refOriginal.Distance(original);
        return Math.Abs(d0 - d1) <= 8;
    }

    private static bool MatchPosition(Minutia refTest, Minutia refOriginal, Minutia test, Minutia original)
    {
        double x = refTest.X - test.X;
        double y = refTest.Y - test.Y;
        double testAngle = Angle.ComputeAngle(x, y);

        x = refOriginal.X - original.X;
        y = refOriginal.Y - original.Y;
        double originalAngle = Angle.ComputeAngle(x, y);

        return Angle.DifferencePi(testAngle, originalAngle) <= 0.523598775598299D; // Math.PI / 6
    }

    private static bool MatchDirections(Minutia test, Minutia original)
    {
        return Angle.DifferencePi(test.Angle, original.Angle) <= 0.523598775598299D; // Math.PI / 6
    }

    private record Triplet(MinutiaPair MainMinutia, MinutiaPair NearestMinutia, MinutiaPair FarthestMinutia)
    {
        public double MatchValue { set; get; }

        public Triplet(MinutiaPair mainMinutia, MinutiaPair nearestMinutia, MinutiaPair farthestMinutia, double matchValue) : this(mainMinutia, nearestMinutia, farthestMinutia)
        {
            MatchValue = matchValue;
        }
    }

    private class MedinaMinutiaDescriptor
    {
        public Minutia Main { get; }
        public Minutia Closest { get; }
        public Minutia Farthest { get; }
        
        private int RidgeCountClosest { get; }
        private int RidgeCountFarthest { get; }

        private double DistanceClosest { get; }
        private double DistanceFarthest { get; }
        
        private double AngleAlphaClosest { get; }
        private double AngleAlphaFarthest { get; }
        
        private double AngleBetaClosest { get; }
        private double AngleBetaFarthest { get; }

        internal MedinaMinutiaDescriptor(Minutia main, Minutia closest, Minutia farthest, PrintImage image)
        {
            Main = main;
            Closest = closest;
            Farthest = farthest;

            DistanceClosest = Main.Distance(closest);
            DistanceFarthest = Main.Distance(farthest);

            if (DistanceFarthest < DistanceClosest)
            {
                (Closest, Farthest) = (Farthest, Closest);
                (DistanceClosest, DistanceFarthest) = (DistanceFarthest, DistanceClosest);
            }

            AngleAlphaClosest = ComputeAlphaAngle(Main, Closest);
            AngleAlphaFarthest = ComputeAlphaAngle(Main, Farthest);

            AngleBetaClosest = ComputeBetaAngle(Main, Closest);
            AngleBetaFarthest = ComputeBetaAngle(Main, Farthest);

            RidgeCountClosest = image.RidgeCount(main, closest);
            RidgeCountFarthest = image.RidgeCount(main, farthest);
        }

        public void Deconstruct(out Minutia main, out Minutia closest, out Minutia farthest)
        {
            main = Main;
            closest = Closest;
            farthest = Farthest;
        }

        private double ComputeAlphaAngle(Minutia mtia0, Minutia mtia1)
        {
            double x = mtia0.X - mtia1.X;
            double y = mtia0.Y - mtia1.Y;
            return Angle.Difference2Pi(mtia0.Angle, Angle.ComputeAngle(x, y));
        }

        private double ComputeBetaAngle(Minutia mtia0, Minutia mtia1)
        {
            return Angle.Difference2Pi(mtia0.Angle, mtia1.Angle);
        }

        internal double RotationInvariantMatch(MedinaMinutiaDescriptor target)
        {
            double distDiff = MatchDistances(target);
            double alphaDiff = MatchAlphaAngles(target);
            double betaDiff = MatchBetaAngles(target);
            double ridgeCountDiff = MatchRidgeCounts(target);
            double mtiaTypeDiff = MatchByType(target);

            double dist = Math.Sqrt(distDiff + alphaDiff + betaDiff + ridgeCountDiff + mtiaTypeDiff);

            return dist < 66 ? (66 - dist)/66 : 0;
        }

        private double MatchDistances(MedinaMinutiaDescriptor target)
        {
            double diff0 = Math.Abs(target.DistanceClosest - DistanceClosest);
            double diff1 = Math.Abs(target.DistanceFarthest - DistanceFarthest);

            return diff0 + diff1;
        }

        private double MatchAlphaAngles(MedinaMinutiaDescriptor target)
        {
            double diff0 = Angle.DifferencePi(target.AngleAlphaClosest, AngleAlphaClosest);
            double diff1 = Angle.DifferencePi(target.AngleAlphaFarthest, AngleAlphaFarthest);

            return 54 * (Math.Pow(diff0, 2) + Math.Pow(diff1, 2)) / Math.PI;
        }

        private double MatchBetaAngles(MedinaMinutiaDescriptor target)
        {
            double diff0 = Angle.DifferencePi(target.AngleBetaClosest, AngleBetaClosest);
            double diff1 = Angle.DifferencePi(target.AngleBetaFarthest, AngleBetaFarthest);

            return 54 * (Math.Pow(diff0, 2) + Math.Pow(diff1, 2)) / Math.PI;
        }

        private double MatchRidgeCounts(MedinaMinutiaDescriptor target)
        {
            double diff0 = Math.Abs(target.RidgeCountClosest - RidgeCountClosest);
            double diff1 = Math.Abs(target.RidgeCountFarthest - RidgeCountFarthest);

            return 3 * (Math.Pow(diff0, 2) + Math.Pow(diff1, 2));
        }

        private double MatchByType(MedinaMinutiaDescriptor target)
        {
            int diff0 = target.Main.Type == Main.Type ? 0 : 1;
            int diff1 = target.Closest.Type == Closest.Type ? 0 : 1;
            int diff2 = target.Farthest.Type == Farthest.Type ? 0 : 1;
            return 3 * (diff0 + diff1 + diff2);
        }
    }

    private class MinutiaMapper
    {
        public MinutiaMapper(Minutia test, Minutia original)
        {
            angle = original.Angle - test.Angle;
            this.original = original;
            this.test = test;
        }

        public Minutia Map(Minutia m)
        {
            return new Minutia(
                (int)Math.Round((m.X - test.X) * Math.Cos(angle) - (m.Y - test.Y) * Math.Sin(angle) + original.X),
                (int)Math.Round((m.X - test.X) * Math.Sin(angle) + (m.Y - test.Y) * Math.Cos(angle) + original.Y),
                angle: m.Angle + angle);
        }

        private readonly double angle;
        private readonly Minutia original;
        private readonly Minutia test;
    }
}