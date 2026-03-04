using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using TaskStatusWpf.Models;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.ViewModels;

public sealed partial class TasksViewModel : ObservableObject
{
    private readonly ApiClient _api;

    public int ProjectId { get; }
    public ObservableCollection<TaskListItemResponse> Tasks { get; } = new();

    // ✅ 画面表示はこれを使う
    public ICollectionView TasksView { get; }

    public event Action? BackRequested;
    public event Action<int>? OpenTaskRequested; // taskId

    [ObservableProperty] private string? headerText;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private TaskListItemResponse? selectedTask;

    // =========================
    // 🔎 検索/絞り込み
    // =========================
    [ObservableProperty] private string searchText = "";

    // ComboBox用
    public IReadOnlyList<string> StatusOptions { get; } =
        new[] { "すべて", "ToDo", "Doing", "Blocked", "Done" };

    [ObservableProperty] private string selectedStatus = "すべて";

    // クイックフィルタ（enumで保持）
    private enum QuickFilter { All, NotDone, Blocked }
    private QuickFilter _quickFilter = QuickFilter.All;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsNotBusy => !IsBusy;

    public TasksViewModel(ApiClient api, int projectId)
    {
        _api = api;
        ProjectId = projectId;
        HeaderText = $"タスク一覧（プロジェクトID：{projectId}）";

        // ✅ CollectionView（フィルタをかける表示用）
        TasksView = CollectionViewSource.GetDefaultView(Tasks);
        TasksView.Filter = FilterTask;
        TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskListItemResponse.TaskId), ListSortDirection.Ascending));
    }

    // =========================
    // Commands
    // =========================
    [RelayCommand]
    private async Task LoadAsync()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            Tasks.Clear();
            var list = await _api.GetProjectTasksAsync(ProjectId);
            foreach (var t in list) Tasks.Add(t);

            ApplyFilters();
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

    [RelayCommand(CanExecute = nameof(CanOpenDetail))]
    private void OpenDetail()
    {
        if (SelectedTask is null) return;
        OpenTaskRequested?.Invoke(SelectedTask.TaskId);
    }
    private bool CanOpenDetail() => SelectedTask is not null;

    [RelayCommand]
    private void Back() => BackRequested?.Invoke();

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        ErrorMessage = null;

        try
        {
            var csv = await _api.GetProjectTasksCsvAsync(ProjectId);

            var dlg = new SaveFileDialog
            {
                FileName = $"tasks_project_{ProjectId}.csv",
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = "csv"
            };

            if (dlg.ShowDialog() != true) return;

            var utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            await File.WriteAllTextAsync(dlg.FileName, csv, utf8Bom);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    // =========================
    // ⚡ クイックボタン
    // =========================
    [RelayCommand]
    private void QuickAll()
    {
        _quickFilter = QuickFilter.All;
        ApplyFilters();
    }

    [RelayCommand]
    private void QuickNotDone()
    {
        _quickFilter = QuickFilter.NotDone;
        ApplyFilters();
    }

    [RelayCommand]
    private void QuickBlocked()
    {
        _quickFilter = QuickFilter.Blocked;
        ApplyFilters();
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = "";
        SelectedStatus = "すべて";
        _quickFilter = QuickFilter.All;
        ApplyFilters();
    }

    // =========================
    // フィルタ本体
    // =========================
    private bool FilterTask(object obj)
    {
        if (obj is not TaskListItemResponse t) return false;

        // 1) クイックフィルタ
        if (_quickFilter == QuickFilter.NotDone)
        {
            if (string.Equals(t.Status, "Done", StringComparison.OrdinalIgnoreCase))
                return false;
        }
        else if (_quickFilter == QuickFilter.Blocked)
        {
            if (!string.Equals(t.Status, "Blocked", StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // 2) ステータス絞り込み
        if (!string.Equals(SelectedStatus, "すべて", StringComparison.Ordinal))
        {
            if (!string.Equals(t.Status, SelectedStatus, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // 3) タイトル検索（部分一致）
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var hit = t.Title?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
            if (!hit) return false;
        }

        return true;
    }

    private void ApplyFilters()
    {
        TasksView.Refresh();
        // フィルタ後に選択が消えるケースに備える
        if (SelectedTask is not null && !TasksView.Cast<object>().Contains(SelectedTask))
            SelectedTask = null;
    }

    public async Task<string> BuildCsvAsync()
    {
        return await _api.GetProjectTasksCsvAsync(ProjectId);
    }

    // =========================
    // 変更通知 → フィルタ反映
    // =========================
    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedStatusChanged(string value) => ApplyFilters();

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsNotBusy));

    partial void OnSelectedTaskChanged(TaskListItemResponse? value)
        => OpenDetailCommand.NotifyCanExecuteChanged();
}