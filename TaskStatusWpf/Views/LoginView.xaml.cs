using System.Windows.Controls;
using TaskStatusWpf.ViewModels;

namespace TaskStatusWpf.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();

        PasswordBox.PasswordChanged += (_, __) =>
        {
            if (DataContext is LoginViewModel vm)
            {
                if (vm.Password != PasswordBox.Password)
                    vm.Password = PasswordBox.Password;
            }
        };

        DataContextChanged += (_, __) =>
        {
            SyncFromVmToPasswordBox();
            HookVmPropertyChanged();
        };
    }

    private void HookVmPropertyChanged()
    {
        if (DataContext is not LoginViewModel vm) return;

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LoginViewModel.Password))
            {
                // TextBoxはBinding済み。PasswordBoxだけ同期が必要。
                if (PasswordBox.Password != vm.Password)
                    PasswordBox.Password = vm.Password ?? "";
            }
            else if (e.PropertyName == nameof(LoginViewModel.IsPasswordVisible))
            {
                // 切替時にズレないように整える
                SyncFromVmToPasswordBox();

                // 表示側TextBoxにもフォーカス移動したいならここでFocus可
                // if (vm.IsPasswordVisible) PasswordTextBox.Focus();
                // else PasswordBox.Focus();
            }
        };
    }

    private void SyncFromVmToPasswordBox()
    {
        if (DataContext is LoginViewModel vm)
        {
            var p = vm.Password ?? "";
            if (PasswordBox.Password != p)
                PasswordBox.Password = p;
        }
    }
}