using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DontLetMeExpireAvalonia.Models;
using DontLetMeExpireAvalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace DontLetMeExpireAvalonia.ViewModels
{
    public partial class ShellViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _flyoutIsPresented;

        [ObservableProperty]
        private ViewModelBase _content;

        [ObservableProperty]
        private FlyoutItem _activeFlyoutItem;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NavigateBackCommand))]
        private bool _canNavigateBack;

        public ObservableCollection<FlyoutItem> FlyoutItems { get; }

        private readonly INavigationService _navigationService;

        public ShellViewModel(): this(new NavigationService())
        {
            
        }

        public ShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.Navigated += OnNavigated;


            FlyoutItems = new ObservableCollection<FlyoutItem>
            {
                new FlyoutItem(typeof(MainViewModel),"Home", "Home", []),
                new FlyoutItem(typeof(ItemViewModel),"dummy", "Item", []),
            };

            ActiveFlyoutItem = FlyoutItems.First();            
        }

        private void OnNavigated(ViewModelBase viewModel)
        {
            Content = viewModel;
            FlyoutIsPresented = false;
            CanNavigateBack = _navigationService.CanNavigateBack;
        }

        async partial void OnActiveFlyoutItemChanged(FlyoutItem value)
        {
            if (value is null) return;
            await _navigationService.NavigateTo(value.ModelType, value.NavigationParameters, true);
        }

        [RelayCommand]
        private void ToggleFlyout() => FlyoutIsPresented = !FlyoutIsPresented;

        [RelayCommand(CanExecute = nameof(CanNavigateBack))]
        private void NavigateBack()
        {
             _navigationService.NavigateBack();
        }


    }
}
