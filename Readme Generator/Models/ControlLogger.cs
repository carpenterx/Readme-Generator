using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System;

namespace Readme_Generator.Models;

public static class ControlLogger
{
    private const string HELP_SUFFIX = "_HELP";
    private static string logRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static string logName = "log.txt";

    public static void SetOutputFileName(string fileName)
    {
        logName = fileName;
    }

    public static void SetOutputRoot(string root)
    {
        logRoot = root;
    }

    public static void SetOutputPath(string root, string fileName)
    {
        logRoot = root;
        logName = fileName;
    }

    public static void LogPositions()
    {
        string logPath = Path.Combine(logRoot, logName);
        if (Directory.Exists(logRoot))
        {
            using StreamWriter streamWriter = File.AppendText(logPath);
            foreach (FrameworkElement frameworkElement in FindHelpControls(Application.Current.MainWindow))
            {
                Point relativePoint = frameworkElement.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

                streamWriter.WriteLine($"Name: {frameworkElement.Name}");
                streamWriter.WriteLine($"X: {relativePoint.X}");
                streamWriter.WriteLine($"Y: {relativePoint.Y}");
                streamWriter.WriteLine($"Width: {frameworkElement.ActualWidth}");
                streamWriter.WriteLine($"Height: {frameworkElement.ActualHeight}");
                streamWriter.WriteLine();
            }
        }
    }

    private static IEnumerable<FrameworkElement> FindHelpControls(DependencyObject depObj)
    {
        if (depObj == null) yield return (FrameworkElement)Enumerable.Empty<FrameworkElement>();
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
            if (ithChild == null) continue;
            if (ithChild is FrameworkElement frameworkElement)
            {
                if (frameworkElement.Name.EndsWith(HELP_SUFFIX)) yield return frameworkElement;
            }

            foreach (DependencyObject dependencyObjectChild in FindHelpControls(ithChild))
            {
                if (dependencyObjectChild is FrameworkElement childFrameworkElement)
                {
                    if (childFrameworkElement.Name.EndsWith(HELP_SUFFIX)) yield return childFrameworkElement;
                }
            }
        }
    }
}
