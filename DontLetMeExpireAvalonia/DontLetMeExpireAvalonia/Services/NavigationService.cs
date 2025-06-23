using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using DontLetMeExpireAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<ViewModelBase> _navigationStack = new();
        public event Action<ViewModelBase> Navigated;

        public async Task NavigateTo<TViewModel>(Dictionary<string, object> parameters = null, bool reset = false) where TViewModel : ViewModelBase
        {
            await NavigateTo(typeof(TViewModel), parameters, reset);
        }

        public async Task NavigateTo(Type viewModelType, Dictionary<string, object> parameters = null, bool reset = false)
        {
            var viewModel = CreateViewModel(viewModelType);
            if (viewModel == null)
            {
                throw new InvalidOperationException($"ViewModel {viewModelType.Name} not found");
            }

            await NavigateTo(viewModel, parameters, reset);
        }

        private ViewModelBase CreateViewModel(Type viewModelType)
        {
            if (Design.IsDesignMode)
            {
                var designModelType = Type.GetType($"{viewModelType.Namespace}.DesignTime_{viewModelType.Name}");
                return (designModelType != null ? Activator.CreateInstance(designModelType, true) : Activator.CreateInstance(viewModelType)) as ViewModelBase
                       ?? Ioc.Default.GetService(viewModelType) as ViewModelBase;
            }
            return Ioc.Default.GetService(viewModelType) as ViewModelBase ?? Activator.CreateInstance(viewModelType, true) as ViewModelBase;
        }

        public async Task NavigateTo(ViewModelBase viewModel, Dictionary<string, object> parameters = null, bool reset = false)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (reset)
            {
                while (_navigationStack.Count > 0)
                {
                    var vm = _navigationStack.Pop();
                    vm.OnNavigatedFrom();
                }
            }

            _navigationStack.Push(viewModel);
            await viewModel.OnNavigatedToAsync(parameters);
            Navigated?.Invoke(viewModel);
        }

        public void NavigateBack(Dictionary<string, object> parameters = null)
        {
            if (CanNavigateBack)
            {
                var currentViewModel = _navigationStack.Pop();
                currentViewModel.OnNavigatedFrom();
                var previousViewModel = _navigationStack.Peek();
                previousViewModel.OnNavigatedToAsync(parameters);
                Navigated?.Invoke(previousViewModel);
            }
        }

        public ViewModelBase CurrentView => _navigationStack.Peek();

        public bool CanNavigateBack => _navigationStack.Count > 1;
    }
}
