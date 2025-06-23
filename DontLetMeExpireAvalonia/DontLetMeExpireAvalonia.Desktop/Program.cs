using System;
using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using DontLetMeExpireAvalonia.OpenFoodFacts;
using DontLetMeExpireAvalonia.Services;
using DontLetMeExpireAvalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DontLetMeExpireAvalonia.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddCommonServices();

        // Register Platform Services here

        var provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
