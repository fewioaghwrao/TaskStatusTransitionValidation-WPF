using TaskStatusWpf.Models;

namespace TaskStatusWpf.Services;

public interface IApiClient
{
    Task<string> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<MeResponse> GetMeAsync(CancellationToken ct = default);
    void SetBearerToken(string? token);

    Task<IReadOnlyList<ProjectListItemResponse>> GetProjectsAsync(string? q = null, bool? archived = null, CancellationToken ct = default);
    Task<IReadOnlyList<TaskListItemResponse>> GetProjectTasksAsync(int projectId, CancellationToken ct = default);
    Task<TaskDetailResponse> GetTaskAsync(int taskId, CancellationToken ct = default);
    Task<string> GetProjectTasksCsvAsync(int projectId, CancellationToken ct = default);
}