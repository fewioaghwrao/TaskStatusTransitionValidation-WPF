using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskStatusWpf.Models;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.ViewModels;

public sealed partial class ProjectsViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly AppSession _session;

    public ObservableCollection<ProjectListItemResponse> Projects { get; } = new();

    [ObservableProperty] private string? welcomeText;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private bool isBusy;

    // ✅ 追加：DataGrid の選択行（これが無いと遷移できない）
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenTasksCommand))]
    private ProjectListItemResponse? selectedProject;

    // ✅ 追加：MainWindow が購読するイベント
    public event Action<int>? OpenTasksRequested;

    public bool IsLeader => _session.IsLeader;

    public ProjectsViewModel(ApiClient api, AppSession session)
    {
        _api = api;
        _session = session;

        var name = _session.Me?.DisplayName;
        var role = _session.Me?.Role;
        WelcomeText = $"{name} ({role})";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            Projects.Clear();
            var list = await _api.GetProjectsAsync();
            foreach (var p in list) Projects.Add(p);
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

    // ✅ 追加：Tasks画面へ
    [RelayCommand(CanExecute = nameof(CanOpenTasks))]
    private void OpenTasks()
    {
        if (SelectedProject is null) return;

        // ★ここだけ ProjectListItemResponse のプロパティ名に合わせる
        // 例: p.Id の場合は SelectedProject.Id にする
        OpenTasksRequested?.Invoke(SelectedProject.ProjectId);
    }

    private bool CanOpenTasks() => SelectedProject is not null;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsNotBusy => !IsBusy;

    partial void OnErrorMessageChanged(string? value)
        => OnPropertyChanged(nameof(HasError));

    partial void OnIsBusyChanged(bool value)
        => OnPropertyChanged(nameof(IsNotBusy));
}