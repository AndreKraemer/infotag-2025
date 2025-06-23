using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using CommunityToolkit.Mvvm.DependencyInjection;
using DontLetMeExpireAvalonia.Services;
using DontLetMeExpireAvalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DontLetMeExpireAvalonia.Android;

[Activity(
    Label = "DontLetMeExpireAvalonia.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var services = new ServiceCollection();
        services.AddCommonServices();

        var provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }

    public override void OnBackPressed()
    {
        var shell = Ioc.Default.GetRequiredService<ShellViewModel>();
        if (shell.NavigateBackCommand.CanExecute(null))
        {
            shell.NavigateBackCommand.Execute(null);
        }
        else
        {
            // This will terminate the app.
            base.OnBackPressed();
        }
    }
}
