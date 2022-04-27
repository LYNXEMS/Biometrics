using Biometrics;

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