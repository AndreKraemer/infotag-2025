using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DontLetMeExpireAvalonia.Models;
using DontLetMeExpireAvalonia.OpenFoodFacts;
using DontLetMeExpireAvalonia.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.ViewModels
{
    public partial class ItemViewModel: ViewModelBase
    {
        private readonly IStorageLocationService _storageLocationService;
        private readonly IItemService _itemService;
        private readonly INavigationService _navigationService;
        private readonly IOpenFoodFactsApiClient _openFoodFactsApiClient;



        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyDataErrorInfo]
        [Required]
        [Length(5, 50)]
        private string _name;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private DateTime _expirationDate = DateTime.Today;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private StorageLocation _selectedLocation;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyDataErrorInfo]
        [Range(1, 10000)]
        private decimal _amount = 0;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _image;

        [ObservableProperty]
        private string _id;

        [ObservableProperty]
        private string _searchText;

        public ItemViewModel(INavigationService navigationService,
                             IStorageLocationService storageLocationService,
                             IItemService itemService,
                             IOpenFoodFactsApiClient openFoodFactsApiClient)
        {
            _navigationService = navigationService;
            _storageLocationService = storageLocationService;
            _itemService = itemService;
            _itemService = itemService;
            _openFoodFactsApiClient = openFoodFactsApiClient;
        }



        public ObservableCollection<StorageLocation> StorageLocations { get; set; } = [];


        override public async Task OnNavigatedToAsync(Dictionary<string, object> parameters)
        {
            var itemId = parameters?.GetValueOrDefault("ItemId", null) as string;
            await InitializeAsync(itemId);
        }

        /// <summary>
        /// Initialisiert das ViewModel asynchron.
        /// </summary>
        public async Task InitializeAsync(string? itemId = null)
        {
            // Speicherorte laden
            var locations = await _storageLocationService.GetAsync();

            // Die Liste der Speicherorte aktualisieren
            StorageLocations.Clear();
            foreach (var location in locations)
            {
                StorageLocations.Add(location);
            }
            if (!string.IsNullOrEmpty(itemId))
            {
                var item = await _itemService.GetByIdAsync(itemId);
                if (item != null)
                {
                    Id = item.Id;
                    Name = item.Name;
                    ExpirationDate = item.ExpirationDate;
                    SelectedLocation = StorageLocations.FirstOrDefault(x => x.Id == item.StorageLocationId);
                    Image = item.Image;
                    Amount = item.Amount;
                    Title = item.Name;
                }

            }
            else
            {
                Id = string.Empty;
                Name = string.Empty;
                ExpirationDate = DateTime.Today;
                SelectedLocation = StorageLocations.First();
                Image = string.Empty;
                Amount = 1;
                Title = "Neuer Eintrag";
            }
        }


        /// <summary>
        /// Speichert das Element asynchron.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            

            // Neues Element mit den
            // Daten des ViewModels erstellen
            var item = new Item
            {
                Id = Id,
                Name = Name,
                ExpirationDate = ExpirationDate,
                StorageLocationId = SelectedLocation.Id,
                Amount = Amount,
                Image = Image
            };

            // Element speichern
            await _itemService.SaveAsync(item);

            // Daten für die Anzeige zurücksetzen
            Name = string.Empty;
            ExpirationDate = DateTime.Today;
            Amount = 0;
            SelectedLocation = StorageLocations.First();

            _navigationService.NavigateBack();
        }

        private bool CanSave()
        {
            return !string.IsNullOrEmpty(Name)
              && Amount > 0;
        }

        [RelayCommand]
        private async Task SearchBarcode()
        {
            var response = await _openFoodFactsApiClient.GetProductByCodeAsync(SearchText);
            if (response is { Status: 1, Product: not null })
            {
                Name = response.Product.ProductName!;

                // Zu einem späteren Zeitpunkt das Bild herunterladen

            }
            else
            {
                // Zu einem späteren Zeitpunkt Fehlermeldung anzeigen
            }
        }


    }

    public class DesignTime_ItemViewModel : ItemViewModel
    {
        public DesignTime_ItemViewModel() : base(new NavigationService(), 
                                                 new DummyStorageLocationService(new DummyItemService()), 
                                                 new DummyItemService(), new OpenFoodFactsApiClient())
        {
            InitializeAsync();
        }
    }
}
