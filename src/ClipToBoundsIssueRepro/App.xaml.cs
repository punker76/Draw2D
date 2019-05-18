using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using ClipToBoundsIssueRepro.Views;

namespace ClipToBoundsIssueRepro
{
    public class App : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            app.Run(new MainWindow());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions { AllowEglInitialization = true })
                         .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                         .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                         .UseSkia()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
