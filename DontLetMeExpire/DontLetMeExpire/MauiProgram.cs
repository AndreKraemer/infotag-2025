﻿using Azure.AI.OpenAI;
using Camera.MAUI;
using CommunityToolkit.Maui;
using DontLetMeExpire.OpenFoodFacts;
using DontLetMeExpire.Services;
using DontLetMeExpire.ViewModels;
using DontLetMeExpire.Views;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace DontLetMeExpire
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
              .UseMauiApp<App>()
              .ConfigureSyncfusionToolkit()
              .UseMauiCommunityToolkit()
              .UseMauiCameraView() // Camera.Maui
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                  fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                  fonts.AddFont("MaterialSymbols-Rounded.ttf", "MaterialSymbolsRounded");
              });

#if DEBUG
            builder.Logging.AddDebug();
#endif
#if WINDOWS
      // DatePicker unter Windows: Erster Tag der Woche anpassen
      Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("FixFirstDayOfWeek", (handler, view) =>
      {
        handler.PlatformView.FirstDayOfWeek = (Windows.Globalization.DayOfWeek)(int)
            System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
      });
#endif

            //    builder.Services.AddSingleton<IStorageLocationService, DummyStorageLocationService>();
            //    builder.Services.AddSingleton<IItemService, DummyItemService>();
            builder.Services.AddSingleton<IStorageLocationService, SqliteStorageLocationService>();
            builder.Services.AddSingleton<IItemService, SqliteItemService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IPreferences>(Preferences.Default);
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IOpenFoodFactsApiClient, OpenFoodFactsApiClient>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ItemViewModel>();
            builder.Services.AddTransient<ItemPage>();
            builder.Services.AddTransient<ItemsViewModel>();
            builder.Services.AddTransient<ItemsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();

            // create your own agent at: https://ai.azure.com/
            var endpoint = Environment.GetEnvironmentVariable("AI_Foundry_Endpoint") ?? "http://localhost/make-sure-to-add-your-endpoint-here";
            var apiKey = Environment.GetEnvironmentVariable("AI_Foundry_ApiKey") ?? "add-your-api-key-here";
            var foundryClient = new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey));
            var chatClient = foundryClient.GetChatClient("gpt-4o").AsIChatClient();

            builder.Services.AddChatClient(chatClient);

            return builder.Build();
        }
    }
}
