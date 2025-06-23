
namespace DontLetMeExpireUno.Views;

public sealed partial class ItemPage : Page
{
    public ItemPage()
    {
        this.InitializeComponent();
        DataContextChanged += ItemPage_DataContextChanged;

    }
    private async void ItemPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // DataContext is of type ShellViewModel instead of MainViewModel
        // the first time somebody navigates to the page.
        // As a workaround, we have to handle the DataContextChangedEvent
        if (DataContext is ItemViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        // DataContext is of type ShellViewModel instead of MainViewModel
        // the first time somebody navigates to the page.
        // As a workaround, we have to handle the DataContextChangedEvent
        if (DataContext is ItemViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }

    }

}

