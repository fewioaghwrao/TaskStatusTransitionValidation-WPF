using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskStatusWpf.Models;

namespace TaskStatusWpf.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    [ObservableProperty] private bool isLoggedIn;
    [ObservableProperty] private string? userText;

    public event Action? LogoutRequested;

    public void SetLoggedIn(MeResponse me)
    {
        IsLoggedIn = true;
        UserText = $"{me.DisplayName} ({me.Role})";
    }

    public void SetLoggedOut()
    {
        IsLoggedIn = false;
        UserText = null;
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }
}