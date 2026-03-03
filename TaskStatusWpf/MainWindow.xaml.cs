using Microsoft.Extensions.Configuration;
using System.Windows;
using TaskStatusWpf.Services;
using TaskStatusWpf.ViewModels;
using TaskStatusWpf.Views;

namespace TaskStatusWpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .Build();

        var baseUrl = config["Api:BaseUrl"]
            ?? throw new InvalidOperationException("Api:BaseUrl is missing.");

        var api = new ApiClient(baseUrl);
        var session = new AppSession();

        var loginVm = new LoginViewModel(api, session);
        var loginView = new LoginView { DataContext = loginVm };
        Host.Content = loginView;

        loginVm.LoginSucceeded += () =>
        {
            var projectsVm = new ProjectsViewModel(api, session);
            var projectsView = new ProjectsView { DataContext = projectsVm };
            Host.Content = projectsView;

            _ = projectsVm.LoadCommand.ExecuteAsync(null);
        };
    }
}