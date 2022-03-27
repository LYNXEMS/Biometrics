using System;
using System.Diagnostics;

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

    public static void Main(string[] args) {
        runImgPy();
    }
}