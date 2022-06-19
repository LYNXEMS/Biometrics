using Biometrics;
using Biometrics.Audio;

bool finger = true;

if (finger)
{
    string[] images = { "print", "print2" };
    Dictionary<string, (PrintImage, List<Minutia>)> results = new();

    foreach (var image in images)
    {
        var print = new PrintImage($"C:/png/print/{image}.png");
        var capitalized = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(image);
        print.SaveImage($"C:/png/print/{capitalized}Pre.png");
        MedianFilter.Filter(print.Image);
        print.SaveImage($"C:/png/print/{capitalized}Median.png");
        print.Image = Helpers.Binarization(print.Image);
        print.SaveImage($"C:/png/print/{capitalized}Binarized.png");
        print.Binarized = Thinning.ZhangSuenThinning(print.Binarized);
        print.SaveImage($"C:/png/print/{capitalized}Thinned.png");
        var min = Minutia.findCheckPoint(print.Binarized);
        results[image] = (print, min);
        Minutia.PrintMinutia(print.Image, min).Save($"C:/png/print/{capitalized}Minutia.png");
    }

    (PrintImage firstImage, List<Minutia> firstMinutia) = results[images[0]];
    (PrintImage secondImage, List<Minutia> secondMinutia) = results[images[1]];
    
    Console.WriteLine("Hello, World!");

    var firstResult = Medina2012Matcher.Match(firstImage, secondImage, firstMinutia, secondMinutia, out _);
    var secondResult = Medina2012Matcher.Match(firstImage, firstImage, firstMinutia, firstMinutia, out _);
    var thirdResult = Medina2012Matcher.Match(secondImage, secondImage, secondMinutia, secondMinutia, out _);

    Console.WriteLine("Result Between = " + firstResult);
    Console.WriteLine("Result First Test = " + secondResult);
    Console.WriteLine("Result Second Test = " + thirdResult);
    
    Console.WriteLine("Goodbye, World!");
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
