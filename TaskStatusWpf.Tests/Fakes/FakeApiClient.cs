using System.Net;
using TaskStatusWpf.Models;
using TaskStatusWpf.Services;

namespace TaskStatusWpf.Tests.Fakes;

internal sealed class FakeApiClient : IApiClient
{
    public Func<string, string, Task<string>> LoginAsyncImpl { get; set; }
        = (_, _) => Task.FromResult("token");

    public Func<Task<MeResponse>> GetMeAsyncImpl { get; set; }
        = () => Task.FromResult(new MeResponse { Role = "Leader" });

    public string? LastBearerToken { get; private set; }

    public void SetBearerToken(string? token) => LastBearerToken = token;

    public Task<string> LoginAsync(string email, string password, CancellationToken ct = default)
        => LoginAsyncImpl(email, password);

    public Task<MeResponse> GetMeAsync(CancellationToken ct = default)
        => GetMeAsyncImpl();

    // --- 以下は今回のLoginViewModelテストでは使わない（ダミー実装） ---
    public Task<IReadOnlyList<ProjectListItemResponse>> GetProjectsAsync(string? q = null, bool? archived = null, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ProjectListItemResponse>>(new List<ProjectListItemResponse>());

    public Task<IReadOnlyList<TaskListItemResponse>> GetProjectTasksAsync(int projectId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<TaskListItemResponse>>(new List<TaskListItemResponse>());

    public Task<TaskDetailResponse> GetTaskAsync(int taskId, CancellationToken ct = default)
        => Task.FromResult(new TaskDetailResponse());

    public Task<string> GetProjectTasksCsvAsync(int projectId, CancellationToken ct = default)
        => Task.FromResult("");
}