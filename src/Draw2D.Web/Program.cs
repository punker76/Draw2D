using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Web;
using Draw2D;

[assembly:SupportedOSPlatform("browser")]

internal class Program
{
    private static void Main(string[] args) 
        => BuildAvaloniaApp().SetupBrowserApp("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
