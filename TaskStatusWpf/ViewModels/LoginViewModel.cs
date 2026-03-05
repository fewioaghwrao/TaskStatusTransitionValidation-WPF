using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IApiClient _api;   // ✅ 変更
    private readonly AppSession _session;

    public event Action? LoginSucceeded;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string email = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool isBusy;

    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private bool isPasswordVisible;

    public string PasswordToggleText => IsPasswordVisible ? "非表示" : "表示";
    partial void OnIsPasswordVisibleChanged(bool value)
        => OnPropertyChanged(nameof(PasswordToggleText));

    public bool CanLogin =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password);

    public string? EmailValidationMessage =>
        string.IsNullOrWhiteSpace(Email) ? "メールアドレスは必須です" : null;

    public string? PasswordValidationMessage =>
        string.IsNullOrWhiteSpace(Password) ? "パスワードは必須です" : null;

    public bool HasEmailError => EmailValidationMessage != null;
    public bool HasPasswordError => PasswordValidationMessage != null;

    partial void OnEmailChanged(string value)
    {
        OnPropertyChanged(nameof(EmailValidationMessage));
        OnPropertyChanged(nameof(HasEmailError));
    }

    partial void OnPasswordChanged(string value)
    {
        OnPropertyChanged(nameof(PasswordValidationMessage));
        OnPropertyChanged(nameof(HasPasswordError));
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
        => IsPasswordVisible = !IsPasswordVisible;

    public LoginViewModel(IApiClient api, AppSession session) // ✅ 変更
    {
        _api = api;
        _session = session;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
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
        catch
        {
            ErrorMessage = "ログインに失敗しました。メールアドレスまたはパスワードを確認してください。";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
        => OnPropertyChanged(nameof(IsNotBusy));

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    partial void OnErrorMessageChanged(string? value)
        => OnPropertyChanged(nameof(HasError));
}