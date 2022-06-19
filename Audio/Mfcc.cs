namespace Biometrics.Audio;

public class Mfcc
{
    public double[][] mfccData;

    public Mfcc(double[][] data) => mfccData = data;

    public Mfcc(Mfcc other)
    {
        mfccData = other.mfccData.Clone() as double[][];
    }

    public static implicit operator Mfcc(double[][] values) => new(values);
}