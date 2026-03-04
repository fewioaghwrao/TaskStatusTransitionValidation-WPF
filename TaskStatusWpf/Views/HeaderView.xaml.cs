using System.Windows;
using System.Windows.Controls;
using TaskStatusWpf.ViewModels;

namespace TaskStatusWpf.Views;

public partial class HeaderView : UserControl
{
    public HeaderView()
    {
        InitializeComponent();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);

        var dlg = new ConfirmDialog("確認", "ログアウトしますか？")
        {
            Owner = owner
        };

        var ok = dlg.ShowDialog() == true && dlg.Result;

        if (!ok) return;

        if (DataContext is ShellViewModel vm && vm.LogoutCommand.CanExecute(null))
        {
            vm.LogoutCommand.Execute(null);
        }
    }
}