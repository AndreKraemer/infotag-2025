using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using CommunityToolkit.Mvvm.DependencyInjection;
using DontLetMeExpireAvalonia;
using DontLetMeExpireAvalonia.Services;
using Microsoft.Extensions.DependencyInjection;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddCommonServices();

        // Register Platform Services here
        services.AddSingleton<IStorageLocationService, DummyStorageLocationService>();
        services.AddSingleton<IItemService, DummyItemService>();

        var provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);
        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}