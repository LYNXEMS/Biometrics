using System.Numerics;

namespace Biometrics.Audio;

public class MfccCalculator
{
    public double[]? frame;
    public double[,]? frame_mass;
    public Complex[,]? frame_mass_FFT;

    int[] filter_points =
    {
        6, 18, 31, 46, 63, 82, 103, 127, 154, 184, 218,
        257, 299, 348, 402, 463, 531, 608, 695, 792, 901, 1023
    };

    double[,] H = new double[20, 1024];

    public double[][] MFCC_20_calculation(double[] wav_PCM)
    {
        var count_frames = wav_PCM.Length * 2 / 2048 + 1;
        RMS_gate(wav_PCM);
        Normalize(wav_PCM);
        frame_mass = Set_Frames(wav_PCM);
        Hamming_window(frame_mass, count_frames);
        frame_mass_FFT = FFT_frames(frame_mass, count_frames);
        var MFCC_mass = new double[20][];
        for (var i = 0; i < 20; i++)
            for (var j = 0; j < 1024; j++)
            {
                if (j < filter_points[i]) H[i, j] = 0;
                if ((filter_points[i] <= j) & (j <= filter_points[i + 1]))
                    H[i, j] = (double)(j - filter_points[i]) / (filter_points[i + 1] - filter_points[i]);
                if ((filter_points[i + 1] <= j) & (j <= filter_points[i + 2]))
                    H[i, j] = (double)(filter_points[i + 2] - j) / (filter_points[i + 2] - filter_points[i + 1]);
                if (j > filter_points[i + 2]) H[i, j] = 0;
            }

        for (var k = 0; k < count_frames; k++)
        {
            var S = new double[20];
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 1024; j++)
                {
                    S[i] += Math.Pow(frame_mass_FFT[k, j].Magnitude, 2) * H[i, j];
                }

                if (S[i] != 0) S[i] = Math.Log(S[i], Math.E);
            }

            for (var l = 0; l < 20; l++)
            {
                MFCC_mass[l] = new double[count_frames];
                for (var i = 0; i < 20; i++)
                    MFCC_mass[l][k] += S[i] * Math.Cos(Math.PI * l * (i * 0.5 / 20));
            }
        }

        return MFCC_mass;
    }


    private void RMS_gate(double[] wav_PCM)
    {
        var k = 0;
        var buf_rms = new double[50];
        double RMS = 0;

        for (var j = 0; j < wav_PCM.Length; j++)
        {
            if (k < 100)
            {
                RMS += Math.Pow(wav_PCM[j], 2);
                k++;
            }
            else
            {
                if (Math.Sqrt(RMS / 100) < 0.005)
                    for (var i = j - 100; i <= j; i++)
                        wav_PCM[i] = 0;
                k = 0;
                RMS = 0;
            }
        }
    }

    private void Normalize(double[] wav_PCM)
    {
        var abs_wav_buf = new double[wav_PCM.Length];
        for (var i = 0; i < wav_PCM.Length; i++)
            if (wav_PCM[i] < 0) abs_wav_buf[i] = -wav_PCM[i];
            else abs_wav_buf[i] = wav_PCM[i];
        var max = abs_wav_buf.Max();
        var k = 1f / max;

        for (var i = 0; i < wav_PCM.Length; i++)
        {
            wav_PCM[i] *= k;
        }
    }

    private double[,] Set_Frames(double[] wav_PCM)
    {
        double[,] frame_mass_1;
        var count_frames = 0;
        var count_samp = 0;

        frame_mass_1 = new double[wav_PCM.Length * 2 / 2048 + 1, 2048];
        for (var j = 0; j < wav_PCM.Length; j++)
        {
            if (j >= 1024)
            {
                count_samp++;
                if (count_samp >= 2049)
                {
                    count_frames += 2;
                    count_samp = 1;
                }

                frame_mass_1[count_frames, count_samp - 1] = wav_PCM[j - 1024];
                frame_mass_1[count_frames + 1, count_samp - 1] = wav_PCM[j];
            }
        }

        return frame_mass_1;
    }

    private void Hamming_window(double[,] frames, int count_frames)
    {
        var omega = 2.0 * Math.PI / 2048f;
        for (var i = 0; i < count_frames; i++)
            for (var j = 0; j < 2048; j++)
                frames[i, j] = (0.54 - 0.46 * Math.Cos(omega * j)) * frames[i, j]; // Replace with NAudio.Dsp.FastFourierTransform.HammingWindow?
    }


    private Complex[,] FFT_frames(double[,] frames, int count_frames)
    {
        var frame_mass_complex = new Complex[count_frames, 2048];
        var FFT_frame = new NAudio.Dsp.Complex[2048];
        for (var k = 0; k < count_frames; k++)
        {
            for (var i = 0; i < 2048; i++) FFT_frame[i] = new NAudio.Dsp.Complex { X = (float)frames[k, i] };

            NAudio.Dsp.FastFourierTransform.FFT(true, 0, FFT_frame);
            for (var i = 0; i < 2048; i++)
            {
                frame_mass_complex[k, i] = new Complex(FFT_frame[i].X, FFT_frame[i].Y);
            }
        }

        return frame_mass_complex;
    }
}