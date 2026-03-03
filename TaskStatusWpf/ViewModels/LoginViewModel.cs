using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly AppSession _session;

    public event Action? LoginSucceeded;

    [ObservableProperty] private string email = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? errorMessage;

    public LoginViewModel(ApiClient api, AppSession session)
    {
        _api = api;
        _session = session;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            var token = await _api.LoginAsync(Email, Password);
            _session.SetToken(token);
            _api.SetBearerToken(token);

            var me = await _api.GetMeAsync();
            _session.SetMe(me);

            LoginSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}