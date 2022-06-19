using Biometrics.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Biometrics;

public class VoiceClip
{
    private AudioFileReader audioFile;
    public ISampleProvider provider;
    public Mfcc mfcc;

    public VoiceClip(string file)
    {
        audioFile = new AudioFileReader(file);
        provider = audioFile.WaveFormat.Channels switch
        {
            1 => audioFile,
            2 => new StereoToMonoSampleProvider(audioFile),
            _ => throw new ArgumentException($"The audio file must have 1 or 2 channels, but it has {audioFile.WaveFormat.Channels}", nameof(file))
        };

        var all = new List<double>();
        var buffer = new float[1024];
        var count = buffer.Length;

        while (count == buffer.Length)
        {
            count = provider.Read(buffer, 0, buffer.Length);
            all.AddRange(buffer.Take(count).Select(x => (double)x));
        }

        mfcc = new MfccCalculator().MFCC_20_calculation(all.ToArray());
    }
}