using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskStatusWpf.Models;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.ViewModels;

public sealed partial class TaskDetailViewModel : ObservableObject
{
    private readonly ApiClient _api;

    public int TaskId { get; }
    public event Action? BackRequested;

    [ObservableProperty] private TaskDetailResponse? task;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsNotBusy => !IsBusy;

    partial void OnErrorMessageChanged(string? value)
        => OnPropertyChanged(nameof(HasError));

    partial void OnIsBusyChanged(bool value)
        => OnPropertyChanged(nameof(IsNotBusy));

    public TaskDetailViewModel(ApiClient api, int taskId)
    {
        _api = api;
        TaskId = taskId;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            Task = await _api.GetTaskAsync(TaskId);
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

    [RelayCommand]
    private void Back() => BackRequested?.Invoke();
}