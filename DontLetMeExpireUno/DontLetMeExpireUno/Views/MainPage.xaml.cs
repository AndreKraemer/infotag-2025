namespace DontLetMeExpireUno.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += MainPage_DataContextChanged;
    }

    private async void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // DataContext is of type ShellViewModel instead of MainViewModel
        // the first time somebody navigates to the page.
        // As a workaround, we have to handle the DataContextChangedEvent
        if (DataContext is MainViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        // DataContext is of type ShellViewModel instead of MainViewModel
        // the first time somebody navigates to the page.
        // As a workaround, we have to handle the DataContextChangedEvent
        if (DataContext is MainViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
        base.OnNavigatedTo(e);
    }
}
