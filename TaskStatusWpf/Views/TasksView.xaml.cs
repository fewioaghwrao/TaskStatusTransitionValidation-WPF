using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using TaskStatusWpf.ViewModels;

namespace TaskStatusWpf.Views;

public partial class TasksView
{
    public TasksView()
    {
        InitializeComponent();

        TasksGrid.MouseDoubleClick += (_, __) =>
        {
            if (DataContext is TasksViewModel vm && vm.OpenDetailCommand.CanExecute(null))
                vm.OpenDetailCommand.Execute(null);
        };
    }

    private async void ExportCsv_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TasksViewModel vm) return;

        var owner = Window.GetWindow(this)!;

        // ✅ 1) 自前確認（⚠・×・フェードイン付き）
        var ok = ConfirmDialog.Show(
            owner,
            title: "CSV出力",
            message: "現在のタスク一覧をCSVとして保存します。\n続行しますか？",
            okText: "出力する",
            cancelText: "キャンセル");

        if (!ok) return;

        string csv;
        try
        {
            vm.ErrorMessage = null;
            vm.IsBusy = true;

            csv = await vm.BuildCsvAsync();
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = ex.Message;
            return;
        }
        finally
        {
            vm.IsBusy = false;
        }

        // ✅ 2) OS標準の保存ダイアログ
        var dlg = new SaveFileDialog
        {
            FileName = $"tasks_project_{vm.ProjectId}.csv",
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            DefaultExt = "csv"
        };
        if (dlg.ShowDialog() != true) return;

        // ✅ 3) 保存
        try
        {
            var utf8Bom = new UTF8Encoding(true);
            await File.WriteAllTextAsync(dlg.FileName, csv, utf8Bom);

            // 保存完了も同じダイアログでOK（⚠が気になるならInfoDialogを別途作る）
            ConfirmDialog.Show(
                owner,
                title: "完了",
                message: $"CSVを保存しました。\n\n{dlg.FileName}",
                okText: "OK",
                cancelText: "閉じる");
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = $"保存に失敗しました：{ex.Message}";
        }
    }
}
