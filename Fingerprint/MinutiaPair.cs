namespace Biometrics;

public record MinutiaPair(Minutia Test, Minutia Original)
{
    public double MatchValue { get; set; }

    public MinutiaPair(Minutia test, Minutia original, double matchValue) : this(test, original)
    {
        MatchValue = matchValue;
    }
}