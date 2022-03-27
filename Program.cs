using System;
using System.Diagnostics;
namespace Biometrics
{
    public class Program
    {
        public static void runImgPy()
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = @"C:\Python310\python.exe";
            processStartInfo.Arguments = Environment.CurrentDirectory + @"\img.py";
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            process.StartInfo = processStartInfo;
            process.Start();
        }

        public static void test()
        {
            pixel p;
            p.x = 0;
            p.y = 0;
            p.value = (char)0;
            Console.WriteLine(p.x + "\n" + p.y + "\n" + Convert.ToInt16(p.value));
        }

        public static void Main(string[] args)
        {
            //test();
            //runImgPy();
        }
    }
}