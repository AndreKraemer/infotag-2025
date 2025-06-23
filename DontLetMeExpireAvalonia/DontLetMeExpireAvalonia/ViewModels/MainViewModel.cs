using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DontLetMeExpireAvalonia.Models;
using DontLetMeExpireAvalonia.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IItemService _itemService;
    private readonly IStorageLocationService _storageLocationService;
    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService, IItemService itemService, IStorageLocationService storageLocationService)
    {
        _itemService = itemService;
        _navigationService = navigationService;
        _storageLocationService = storageLocationService;
    }

    [ObservableProperty]
    private int _stockCount;

    [ObservableProperty]
    private int _expiringSoonCount;

    [ObservableProperty]
    private int _expiresTodayCount;

    [ObservableProperty]
    private int _expiredCount;

    public ObservableCollection<StorageLocationWithItemCount> StorageLocations { get; } = [];

    public override async Task OnNavigatedToAsync(Dictionary<string, object> parameters)
    {
        await InitializeAsync();
    }

    /// <summary>
    /// Initialisiert das ViewModel asynchron.
    /// </summary>
    public async Task InitializeAsync()
    {
        var locations = await _storageLocationService.GetWithItemCountAsync();

        StorageLocations.Clear();
        foreach (var location in locations)
        {
            StorageLocations.Add(location);
        }

        StockCount = (await _itemService.GetAsync()).Count();
        ExpiringSoonCount = (await _itemService.GetExpiresSoonAsync()).Count();
        ExpiresTodayCount = (await _itemService.GetExpiresTodayAsync()).Count();
        ExpiredCount = (await _itemService.GetExpiredAsync()).Count();
        NavigateToAddItemCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task NavigateToStock()
    {
        await NavigateToItemsPage("Stock");
    }

    [RelayCommand]
    private async Task NavigateToExpiringSoon()
    {
        await NavigateToItemsPage("ExpiringSoon");
    }

    [RelayCommand]
    private async Task NavigateToExpiresToday()
    {
        await NavigateToItemsPage("ExpiresToday");
    }

    [RelayCommand]
    private async Task NavigateToExpired()
    {
        await NavigateToItemsPage("Expired");
    }

    [RelayCommand]
    private async Task NavigateToLocation(string locationId)
    {
        await NavigateToItemsPage("Location", locationId);
    }

    private async Task NavigateToItemsPage(string displayMode, string? locationId = null)
    {
        var navigationParams = new Dictionary<string, object>
        {
          { "DisplayMode", displayMode },
          { "LocationId", locationId }
        };
        await _navigationService.NavigateTo<ItemsViewModel>(navigationParams);
    }

    [RelayCommand(CanExecute = nameof(CanNavigateToAddItem))]
    private async Task NavigateToAddItem()
    {
        await _navigationService.NavigateTo<ItemViewModel>();
    }

    private bool CanNavigateToAddItem()
    {
        return StorageLocations.Any();
    }

    [RelayCommand]
    private async Task CreateSuggestedLocations()
    {
        foreach (var location in DummyData.Locations)
        {
            await _storageLocationService.SaveAsync(location);
        }
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task CreateSuggestedLocationsAndStock()
    {

        foreach (var location in DummyData.Locations)
        {
            await _storageLocationService.SaveAsync(location);
        }

        foreach (var item in DummyData.Items)
        {
            await _itemService.SaveAsync(item);
        }

        await InitializeAsync();
    }


}

public class DesignTime_MainViewModel : MainViewModel
{
    public DesignTime_MainViewModel() : base(new NavigationService(), new DummyItemService(), new DummyStorageLocationService(new DummyItemService()))
    {
        InitializeAsync();
    }
}
