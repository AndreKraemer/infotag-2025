using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using DontLetMeExpireAvalonia.ViewModels;
using DontLetMeExpireAvalonia.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using DontLetMeExpireAvalonia.Services;
using System.Globalization;
using System.Threading;
using Avalonia.Svg.Skia;
using System;
using DontLetMeExpireAvalonia.OpenFoodFacts;
using Avalonia.Controls;

namespace DontLetMeExpireAvalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {

        GC.KeepAlive(typeof(SvgImageExtension).Assembly);
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

        // DontLetMeExpireAvalonia.Resources.Strings.AppResources.Culture = new CultureInfo("en-US");
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        // Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        // Configure IoC
        if (Design.IsDesignMode)
        {
            var services = new ServiceCollection();
            services.AddCommonServices();
            var provider = services.BuildServiceProvider();
            Ioc.Default.ConfigureServices(provider);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = Ioc.Default.GetRequiredService<ShellViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new ShellView
            {
                DataContext = Ioc.Default.GetRequiredService<ShellViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}