using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.ViewModels;

public abstract partial class ViewModelBase : ObservableValidator
{
    [ObservableProperty]
    private string _title;

    public virtual Task OnNavigatedToAsync(Dictionary<string, object> parameters)
    {
        // Standardmäßig nichts tun, kann in abgeleiteten Klassen überschrieben werden
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedFrom()
    {
        // Aufräumarbeiten oder Speichern von Zuständen
    }
}
