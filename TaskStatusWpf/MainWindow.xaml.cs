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

        var shellVm = new ShellViewModel();
        DataContext = shellVm;

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

        // Login画面は保持（ログアウトで戻すため）
        var loginVm = new LoginViewModel(api, session);
        var loginView = new LoginView { DataContext = loginVm };
        Host.Content = loginView;

        // ✅ Logout（どの画面からでもヘッダーで発火）
        shellVm.LogoutRequested += () =>
        {
            session.Clear();
            api.SetBearerToken(null);

            shellVm.SetLoggedOut();
            Host.Content = loginView;
        };

        loginVm.LoginSucceeded += () =>
        {
            // ✅ ヘッダーにユーザー表示を反映（LoginViewModelが Me を取って session.Me に入れている前提）
            if (session.Me is not null)
                shellVm.SetLoggedIn(session.Me);

            var projectsVm = new ProjectsViewModel(api, session);
            var projectsView = new ProjectsView { DataContext = projectsVm };
            Host.Content = projectsView;

            _ = projectsVm.LoadCommand.ExecuteAsync(null);

            projectsVm.OpenTasksRequested += (projectId) =>
            {
                var tasksVm = new TasksViewModel(api, projectId);
                var tasksView = new TasksView { DataContext = tasksVm };
                Host.Content = tasksView;

                tasksVm.BackRequested += () =>
                {
                    Host.Content = projectsView;
                };

                tasksVm.OpenTaskRequested += (taskId) =>
                {
                    var detailVm = new TaskDetailViewModel(api, taskId);
                    var detailView = new TaskDetailView { DataContext = detailVm };
                    Host.Content = detailView;

                    detailVm.BackRequested += () =>
                    {
                        Host.Content = tasksView;
                    };

                    _ = detailVm.LoadCommand.ExecuteAsync(null);
                };

                _ = tasksVm.LoadCommand.ExecuteAsync(null);
            };
        };
    }
}