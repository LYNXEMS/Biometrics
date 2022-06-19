using Biometrics;
using Biometrics.Audio;
using System.Drawing;

bool finger = false;

if (finger)
{
    var print = new PrintImage("C:/png/print/print.png");
    print.SaveImage("C:/png/print/PrintPre.png");
    MedianFilter.Filter(print.Image);
    print.SaveImage("C:/png/print/PrintMedian.png");
    print.Image = Helpers.Binarization(print.Image);
    print.SaveImage("C:/png/print/PrintBinarized.png");
    print.Binarized = Thinning.ZhangSuenThinning(print.Binarized);
    print.SaveImage("C:/png/print/PrintThinned.png");
    var min = Minutia.findCheckPoint(print.Binarized);
    Minutia.PrintMinutia(print.Image, min).Save("C:/png/print/PrintMinutia.png");
    Console.WriteLine("Hello, World!");
}
else
{
    var osa1 = new VoiceClip("C:/png/osa1.wav");
    var osa2 = new VoiceClip("C:/png/osa3.wav");
    List<Mfcc> mfcc1 = new List<Mfcc>();
    mfcc1.Add(osa1.mfcc);
    MfccList mfcclist = new(mfcc1);
    Console.WriteLine(mfcclist.Distance(osa2.mfcc));
    Console.ReadLine();
}
