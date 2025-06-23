using DontLetMeExpireAvalonia.OpenFoodFacts;
using DontLetMeExpireAvalonia.Services;
using DontLetMeExpireAvalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DontLetMeExpireAvalonia
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<IStorageLocationService, SqliteStorageLocationService>();
            services.AddSingleton<IItemService, SqliteItemService>();
            services.AddSingleton<IOpenFoodFactsApiClient, OpenFoodFactsApiClient>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ItemViewModel>();
            services.AddTransient<ItemsViewModel>();
        }
    }
}
