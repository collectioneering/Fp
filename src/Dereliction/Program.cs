using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;
using Fp;

namespace Dereliction;

internal static class Program
{
    public const string PROGRAM_NAME = nameof(Dereliction);
    public const string SCRIPT_DIRECTORY = "scripts";
    private static string[]? s_args;
    private static string? s_workingDirectory;
    private static string? s_mainScript;

    private static void CategorizeArgs()
    {
        if (s_args != null) return;
        s_args = Environment.GetCommandLineArgs();
        if (s_args.Length == 1) return;
        string main = s_args[1];
        if (Directory.Exists(main)) s_workingDirectory = Path.GetFullPath(main);
        else if (Processor.PathHasExtension(main, ".csx")) s_mainScript = main;
    }

    public static string WorkingDirectory
    {
        get
        {
            if (s_args == null) CategorizeArgs();
            if (s_workingDirectory == null)
            {
                s_workingDirectory = Path.GetFullPath(SCRIPT_DIRECTORY);
                if (!Directory.Exists(s_workingDirectory)) Directory.CreateDirectory(s_workingDirectory);
            }

            return s_workingDirectory;
        }
    }

    public static string? MainScript
    {
        get
        {
            if (s_args == null) CategorizeArgs();
            return s_mainScript;
        }
    }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        if (MainScript != null) fpx.Program.ConsoleAsync(args).Wait();
        else BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
#if DEBUG
            .WithDeveloperTools()
#endif
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
