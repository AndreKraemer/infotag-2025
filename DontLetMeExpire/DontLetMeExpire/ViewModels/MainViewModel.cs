using DontLetMeExpire.Models;
using DontLetMeExpire.Resources.Strings;
using DontLetMeExpire.Services;
using DontLetMeExpire.Views;
using Microsoft.Extensions.AI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DontLetMeExpire.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IItemService _itemService;
    private readonly IStorageLocationService _storageLocationService;
    private readonly IChatClient _chatClient;
    private int _stockCount;
    private int _expiringSoonCount;
    private int _expiresTodayCount;
    private int _expiredCount;
    private string _recipeResult = string.Empty;
    private bool _isBottomSheetOpen;
    private bool _isGeneratingRecipe;

    public MainViewModel(INavigationService navigationService, IItemService itemService, IStorageLocationService storageLocationService, IChatClient chatClient)
    {
        _itemService = itemService;
        _storageLocationService = storageLocationService;
        _chatClient = chatClient;
        _navigationService = navigationService;
        CreateSuggestedLocationsCommand = new Command(async () => await CreateSuggestedLocations());
        CreateSuggestedLocationsAndStockCommand = new Command(async () => await CreateSuggestedLocationsAndStock());
        NavigateToAddItemCommand = new Command(async () => await NavigateToAddItem(), CanAddNewItem);
        NavigateToStockCommand = new Command(async () => await NavigateToStock());
        NavigateToExpiringSoonCommand = new Command(async () => await NavigateToExpiringSoon());
        NavigateToExpiresTodayCommand = new Command(async () => await NavigateToExpiresToday());
        NavigateToExpiredCommand = new Command(async () => await NavigateToExpired());
        NavigateToLocationCommand = new Command<string>(async (locationId) => await NavigateToLocation(locationId));
        GenerateRecipeCommand = new Command(async () => await GenerateRecipe(), CanGenerateRecipe);
        CloseBottomSheetCommand = new Command(() => IsBottomSheetOpen = false);
    }

    public ICommand NavigateToAddItemCommand { get; }
    public ICommand GenerateRecipeCommand { get; }
    public ICommand CloseBottomSheetCommand { get; }

    /// <summary>
    /// Die Anzahl aller gelagerten Artikel.
    /// </summary>
    public int StockCount
    {
        get => _stockCount;
        set => SetProperty(ref _stockCount, value);
    }

    /// <summary>
    /// Die Anzahl der Artikel, die bald ablaufen.
    /// </summary>
    public int ExpiringSoonCount
    {
        get => _expiringSoonCount;
        set => SetProperty(ref _expiringSoonCount, value);
    }

    /// <summary>
    /// Die Anzahl der Artikel, die heute ablaufen.
    /// </summary>
    public int ExpiresTodayCount
    {
        get => _expiresTodayCount;
        set => SetProperty(ref _expiresTodayCount, value);
    }

    /// <summary>
    /// Die Anzahl der abgelaufenen Artikel.
    /// </summary>
    public int ExpiredCount
    {
        get => _expiredCount;
        set => SetProperty(ref _expiredCount, value);
    }

    /// <summary>
    /// Das Ergebnis der Rezeptgenerierung.
    /// </summary>
    public string RecipeResult
    {
        get => _recipeResult;
        set => SetProperty(ref _recipeResult, value);
    }

    /// <summary>
    /// Gibt an, ob das Bottom Sheet geöffnet ist.
    /// </summary>
    public bool IsBottomSheetOpen
    {
        get => _isBottomSheetOpen;
        set => SetProperty(ref _isBottomSheetOpen, value);
    }

    /// <summary>
    /// Gibt an, ob gerade ein Rezept generiert wird.
    /// </summary>
    public bool IsGeneratingRecipe
    {
        get => _isGeneratingRecipe;
        set => SetProperty(ref _isGeneratingRecipe, value);
    }

    public ICommand NavigateToStockCommand { get; }
    public ICommand NavigateToExpiringSoonCommand { get; }
    public ICommand NavigateToExpiresTodayCommand { get; }
    public ICommand NavigateToExpiredCommand { get; }

    public ICommand CreateSuggestedLocationsCommand { get; }
    public ICommand CreateSuggestedLocationsAndStockCommand { get; }
    public Command<string> NavigateToLocationCommand { get; }

    public ObservableCollection<StorageLocationWithItemCount> StorageLocations { get; } = [];

    public ObservableCollection<ConditionWithCount> ConditionsWithCount { get; } = [];

    /// <summary>
    /// Initialisiert das ViewModel asynchron.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;
            var locations = await _storageLocationService.GetWithItemCountAsync();
            StorageLocations.Clear();

            // //HACK: Add Dummy Locations for Shimmer Effect in UI
            StorageLocations.Add(new StorageLocationWithItemCount());
            StorageLocations.Add(new StorageLocationWithItemCount());
            StorageLocations.Add(new StorageLocationWithItemCount());

            // Ladezeit simulieren
            await Task.Delay(3000);

            StorageLocations.Clear();
            foreach (var location in locations)
            {
                StorageLocations.Add(location);
            }

            StockCount = (await _itemService.GetAsync()).Count();
            ExpiringSoonCount = (await _itemService.GetExpiresSoonAsync()).Count();
            ExpiresTodayCount = (await _itemService.GetExpiresTodayAsync()).Count();
            ExpiredCount = (await _itemService.GetExpiredAsync()).Count();

            // calculate the stock that is not expiring soon , expires today or is already expired
            var myStockCount = StockCount - ExpiringSoonCount - ExpiresTodayCount - ExpiredCount;

            ConditionsWithCount.Clear();
            ConditionsWithCount.Add(new ConditionWithCount(AppResources.ExpiringSoon, ExpiringSoonCount));
            ConditionsWithCount.Add(new ConditionWithCount(AppResources.ExpiresToday, ExpiresTodayCount));
            ConditionsWithCount.Add(new ConditionWithCount(AppResources.Expired, ExpiredCount));
            ConditionsWithCount.Add(new ConditionWithCount(AppResources.MyStock, myStockCount));

            ((Command)NavigateToAddItemCommand).ChangeCanExecute();
            ((Command)GenerateRecipeCommand).ChangeCanExecute();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task NavigateToStock()
    {
        await NavigateToItemsPage("Stock");
    }

    private async Task NavigateToExpiringSoon()
    {
        await NavigateToItemsPage("ExpiringSoon");
    }

    private async Task NavigateToExpiresToday()
    {
        await NavigateToItemsPage("ExpiresToday");
    }

    private async Task NavigateToExpired()
    {
        await NavigateToItemsPage("Expired");
    }

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
        await _navigationService.GoToAsync(nameof(ItemsPage), navigationParams);
    }

    private async Task NavigateToAddItem()
    {
        await _navigationService.GoToAsync(nameof(ItemPage));
    }

    private bool CanAddNewItem()
    {
        return StorageLocations.Any();
    }

    private bool CanGenerateRecipe()
    {
        return ExpiringSoonCount > 0 && !IsGeneratingRecipe;
    }

    private async Task GenerateRecipe()
    {
        try
        {
            IsGeneratingRecipe = true;
            RecipeResult = "Rezept wird generiert...";
            IsBottomSheetOpen = true;

            // Holen der bald ablaufenden Artikel
            var expiringItems = await _itemService.GetExpiresSoonAsync();
            
            if (!expiringItems.Any())
            {
                RecipeResult = "Es gibt keine bald ablaufenden Lebensmittel, für die ein Rezept erstellt werden könnte.";
                return;
            }

            // Prompt für OpenAI erstellen
            var prompt = "Ich habe diese Lebensmittel:";
            foreach (var item in expiringItems)
            {
                prompt += $"\n- {item.Name} (läuft ab am {item.ExpirationDate:dd.MM.yyyy})";
            }
            prompt += "\nIch möchte möglichst wenig davon verschwenden. Erstelle mir ein Rezept und achte dabei darauf, dass die Lebensmittel, die am schnellsten ablaufen werden, dabei verbraucht werden. Wenn mir etwas fehlt, dann weise mich darauf hin.";

            // Rezept generieren
            var chatMessage = new ChatMessage(ChatRole.User, prompt);
            var response = await _chatClient.GetResponseAsync(chatMessage);
            
            // Antwort im ViewModel speichern
            RecipeResult = response.Text;
        }
        catch (Exception ex)
        {
            RecipeResult = $"Bei der Rezeptgenerierung ist ein Fehler aufgetreten: {ex.Message}";
        }
        finally
        {
            IsGeneratingRecipe = false;
        }
    }

    private async Task CreateSuggestedLocations()
    {
        foreach (var location in DummyData.Locations)
        {
            await _storageLocationService.SaveAsync(location);
        }
        await InitializeAsync();
    }

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

public record ConditionWithCount(string Condition, int Count);