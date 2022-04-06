using Biometrics;
using System.Drawing;

var print = new PrintImage("C:/png/print.tif");
bool[][] stuff = Helpers.ZhangSuenThinning(print.binarized);
print.SaveImage("C:/png/saved.png");
print.SaveImageBinarized("C:/png/savedb.png");
var img2 = new PrintImage(Helpers.Bool2Image(stuff));
img2.SaveImage("C:/png/savedf.png");
Console.WriteLine("Hello, World!");
