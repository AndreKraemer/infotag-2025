using CommunityToolkit.Mvvm.Input;
using DontLetMeExpireAvalonia.Models;
using DontLetMeExpireAvalonia.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.ViewModels
{
    public partial class ItemsViewModel : ViewModelBase
    {

        private readonly IItemService _itemService;
        private readonly INavigationService _navigationService;
        private readonly IStorageLocationService _storageLocationService;


        public ObservableCollection<Item> Items { get; } = [];
        public string? DisplayMode { get; private set; }
        public string? LocationId { get; private set; }

        public ItemsViewModel(INavigationService navigationService,
                              IItemService itemService,
                              IStorageLocationService storageLocationService)
        {
            _navigationService = navigationService;
            _itemService = itemService;
            _storageLocationService = storageLocationService;
        }

        override public async Task OnNavigatedToAsync(Dictionary<string, object> parameters)
        {
            DisplayMode = parameters.GetValueOrDefault("DisplayMode" ,DisplayMode) as string;
            LocationId = parameters.GetValueOrDefault("LocationId", LocationId) as string;
            await InitializeAsync(DisplayMode, LocationId);
        }

        public async Task InitializeAsync(string displayMode, string locationId)
        {
            IEnumerable<Item> items;

            switch (displayMode)
            {
                case "Stock":
                    items = await _itemService.GetAsync();
                    Title = "Mein Vorrat";
                    break;
                case "ExpiringSoon":
                    items = await _itemService.GetExpiresSoonAsync();
                    Title = "Bald ablaufend";
                    break;
                case "ExpiresToday":
                    items = await _itemService.GetExpiresTodayAsync();
                    Title = "Heute ablaufend";
                    break;
                case "Expired":
                    items = await _itemService.GetExpiredAsync();
                    Title = "Abgelaufen";
                    break;
                case "Location":
                    items = await _itemService.GetByLocationAsync(locationId);
                    var location = await _storageLocationService.GetByIdAsync(locationId);
                    Title = location?.Name ?? "Ort";
                    break;
                default:
                    items = await _itemService.GetAsync();
                    break;
            }

            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        [RelayCommand]
        private async Task NavigateToDetails(Item? item)
        {
            var navigationParams = new Dictionary<string, object>
              {
                  {"ItemId", item?.Id}
              };
            await _navigationService.NavigateTo<ItemViewModel>(navigationParams);
        }
    }

    public class DesignTime_ItemsViewModel : ItemsViewModel
    {
        public DesignTime_ItemsViewModel() : base(new NavigationService(), new DummyItemService(), new DummyStorageLocationService(new DummyItemService()))
        {
            InitializeAsync("Stock", "");
        }
    }
}
