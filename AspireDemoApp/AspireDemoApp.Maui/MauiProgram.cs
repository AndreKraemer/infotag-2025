using AspireDemoApp.Maui.ViewModels;
using AspireDemoApp.Maui.Views;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspireDemoApp.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddTransient<WeatherPage>();
            builder.Services.AddHttpClient<WeatherApiClient>(client =>
            {
                // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
                // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
                client.BaseAddress = new("https+http://apiService");
            });
            builder.AddServiceDefaults();
            return builder.Build();
        }
    }
}
