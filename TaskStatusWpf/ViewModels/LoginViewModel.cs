using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly AppSession _session;

    public event Action? LoginSucceeded;

    // 入力が変わったら LoginCommand の活性/非活性を更新
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string email = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password = "";

    // Busy が変わったら LoginCommand の活性/非活性を更新
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool isBusy;

    [ObservableProperty] private string? errorMessage;

    [ObservableProperty] private bool isPasswordVisible;

    public string PasswordToggleText => IsPasswordVisible ? "非表示" : "表示";
    partial void OnIsPasswordVisibleChanged(bool value)
        => OnPropertyChanged(nameof(PasswordToggleText));

    // ✅ 必須入力チェック（超最低限）
    public bool CanLogin =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password);

    // ✅ 必須メッセージ（簡易）
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

    public LoginViewModel(ApiClient api, AppSession session)
    {
        _api = api;
        _session = session;
    }

    // ✅ CanExecute を使う：ボタンが自動でDisableになる
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
            // ユーザー向け（詳細は後でログへ）
            ErrorMessage = "ログインに失敗しました。メールアドレスまたはパスワードを確認してください。";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    partial void OnErrorMessageChanged(string? value)
        => OnPropertyChanged(nameof(HasError));
}