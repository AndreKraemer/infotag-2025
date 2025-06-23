using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AspireDemoApp.Maui.ViewModels
{
    public partial class WeatherViewModel : ObservableObject
    {
        private readonly WeatherApiClient _weatherApiClient;

        [ObservableProperty]
        private ObservableCollection<WeatherForecast> _forecasts = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        public WeatherViewModel(WeatherApiClient weatherApiClient)
        {
            _weatherApiClient = weatherApiClient;
        }

        [RelayCommand]
        public async Task LoadWeatherAsync()
        {
            if (IsLoading)
                return;

            try
            {
                HasError = false;
                ErrorMessage = string.Empty;
                IsLoading = true;

                var forecasts = await _weatherApiClient.GetWeatherAsync();
                
                Forecasts.Clear();
                foreach (var forecast in forecasts)
                {
                    Forecasts.Add(forecast);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error loading weather data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}