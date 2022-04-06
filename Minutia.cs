namespace Biometrics
{
    public enum MinutiaType
    {
        Initium, Terminatio,
        BifurcatioSimplex, BifurcatioDuplex, BifurcatioTriplex,
        LunctioSimplex, LunctioDuplex, LunctioTriplex,
        Unculus,
        OcellusSimplex, OcellusDuplex,
        PonticulusSimplex, PonticulusGemellus,
        Punctum,
        Segmentum,
        LuncturaLateralis,
        LineaIntermittens,
        Decussatio,
        Tripus,
        LineaRudimentalis,
        M
    }
    public enum MinutiaTypeSymbol
    {
        I, T,
        B1, B2, B3,
        JN1, JN2, JN3,
        U,
        O1, O2,
        P1, P2,
        PN,
        S,
        J,
        LI,
        D,
        TR,
        LR,
        M
    }
    public class Minutia
    {
        public Minutia(MinutiaType name, MinutiaTypeSymbol symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        public MinutiaType Name { set; get; }
        public MinutiaTypeSymbol Symbol { set; get; }
        public string getName() => Enum.GetName(Name);
        public string getSymbol() => Enum.GetName(Name);
    }
}
