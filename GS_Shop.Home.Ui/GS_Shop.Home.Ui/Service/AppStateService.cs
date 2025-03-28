using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GS_Shop.Home.Ui.Service;

public class AppStateService
{
    public event Action? OnChange;

    private bool _isUserLogin;
    public bool IsUserLogin
    {
        get => _isUserLogin;
        set
        {
            if (_isUserLogin != value)
            {
                _isUserLogin = value;
                NotifyStateChanged();
            }
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
