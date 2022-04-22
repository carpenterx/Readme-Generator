using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Readme_Generator.Models;

public static class ControlScreenshotter
{
    private static string screenshotRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static string screenshotName = "screenshot";

    public static void SetOutputFileName(string fileName)
    {
        screenshotName = fileName;
    }

    public static void SetOutputRoot(string root)
    {
        screenshotRoot = root;
    }

    public static void SetOutputPath(string root, string fileName)
    {
        screenshotRoot = root;
        screenshotName = fileName;
    }

    public static void TakeScreenshot(FrameworkElement element)
    {
        if (Directory.Exists(screenshotRoot))
        {
            string screenshotFullName = $"{screenshotName} {DateTime.Now:ddMMyyyy-hhmmss}.png";
            //string filename = @"C:\Users\jorda\Desktop\screenshot " + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
            string screenshotPath = Path.Combine(screenshotRoot, screenshotFullName);

            RenderTargetBitmap bmp = new((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(element);

            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            FileStream fs = new(screenshotPath, FileMode.Create);
            encoder.Save(fs);

            fs.Close();
        }
    }

    public static void TakeScreenshot()
    {
        TakeScreenshot(Application.Current.MainWindow);
    }
}
