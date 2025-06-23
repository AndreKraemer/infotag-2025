using DontLetMeExpireAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.Services
{
    public interface INavigationService
    {
        bool CanNavigateBack { get; }
        ViewModelBase CurrentView { get; }

        event Action<ViewModelBase> Navigated;

        void NavigateBack(Dictionary<string, object> parameters = null);
        Task NavigateTo<TViewModel>(Dictionary<string, object> parameters = null, bool reset = false) where TViewModel : ViewModelBase;
        Task NavigateTo(Type viewModelType, Dictionary<string, object> parameters = null, bool reset = false);
        Task NavigateTo(ViewModelBase viewModel, Dictionary<string, object> parameters = null, bool reset = false);
    }
}