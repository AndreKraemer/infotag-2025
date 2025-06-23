using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DontLetMeExpireUno.Services;

namespace DontLetMeExpireUno.ViewModels;

public partial class ItemViewModel: ObservableValidator
{
    private readonly IStorageLocationService _storageLocationService;
    private readonly IItemService _itemService;
    private readonly INavigator _navigator;
    private readonly Entity _entity;

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
    private decimal _amount;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _image;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PropertyErrorMessages))]
    [NotifyPropertyChangedFor(nameof(isValid))]
    IEnumerable<ValidationResult> _errors = [];

    public Dictionary<string, string> PropertyErrorMessages
    {
        get
        {
            return _errors.ToDictionary(
                pair => pair.MemberNames.FirstOrDefault(),
                pair => pair.ErrorMessage
            );
        }
    }

    public bool isValid => !Errors.Any();

    public ItemViewModel(INavigator navigator, Entity entity, 
                         IItemService itemService, IStorageLocationService storageLocationService)
    { 
        _navigator = navigator;
        _entity = entity;
        _itemService = itemService;
        _storageLocationService = storageLocationService;
    }


    public ObservableCollection<StorageLocation> StorageLocations { get; set; } = [];


    /// <summary>
    /// Initialisiert das ViewModel asynchron.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Speicherorte laden
        var locations = await _storageLocationService.GetAsync();

        // Die Liste der Speicherorte aktualisieren
        StorageLocations.Clear();
        foreach (var location in locations)
        {
            StorageLocations.Add(location);
        }
        SelectedLocation = StorageLocations.FirstOrDefault();
    }


    /// <summary>
    /// Speichert das Element asynchron.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        ClearErrors();
        ValidateAllProperties();
        Errors = GetErrors();
        if(!isValid)
        {
            return;
        }

        // Neues Element mit den
        // Daten des ViewModels erstellen
        var item = new Item
        {
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

        _navigator.GoBack(this);
    }

    private bool CanSave()
    {
        return true;
            // !string.IsNullOrEmpty(Name)
          //&& Amount > 0;
    }
}
