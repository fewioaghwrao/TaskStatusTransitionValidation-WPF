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
}