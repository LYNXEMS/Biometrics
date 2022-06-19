using Newtonsoft.Json;

namespace Biometrics.Audio;

public class MfccList
{
    public List<Mfcc> mfccs;

    public MfccList(List<Mfcc> mfccs) => this.mfccs = mfccs;

    public MfccList(List<VoiceClip> mfccs)
    {
        this.mfccs = new List<Mfcc>();
        foreach (var item in mfccs)
        {
            this.mfccs.Add(item.mfcc);
        }
    }

    public MfccList(string json) => mfccs = JsonConvert.DeserializeObject<List<Mfcc>>(json) ?? throw new Exception("File doesn't contain correct MFCC data");

    public MfccList(FileSystemInfo file) : this(File.ReadAllText(file.FullName))
    { }

    public void Serialize(string path) => File.WriteAllText(path, JsonConvert.SerializeObject(mfccs, Formatting.None));

    public double Distance(Mfcc other)
    {
        if (!mfccs.Any())
            return double.NegativeInfinity;

        double distance = 0;
        foreach (var item in mfccs)
        {
            for (int i = 0; i < 20; i++)
            {
                distance += DynamicTimeWarping.Distance(item.mfccData[i], other.mfccData[i]);
            }
        }
        return distance / (mfccs.Count * 20);
    }
}