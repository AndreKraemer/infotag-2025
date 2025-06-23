using DontLetMeExpireAvalonia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }

    public class DesignTime_SettingsViewModel : SettingsViewModel
    {
        public DesignTime_SettingsViewModel(): base(new NavigationService())
        {
            
        }
    }
}
