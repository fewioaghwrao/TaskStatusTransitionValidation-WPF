using System.Text.Json.Serialization;

namespace TaskStatusWpf.Models;

public sealed class AuthLoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class AuthTokenResponse
{
    public string Token { get; set; } = "";
}

public sealed class MeResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Role { get; set; } = "Worker";
    public string? RoleId { get; set; }
}

public sealed class ProjectListItemResponse
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = "";
    public bool IsArchived { get; set; }
}

// ProblemDetails / ValidationProblemDetails の最小受け皿
public sealed class ProblemDetailsDto
{
    public int? Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? Type { get; set; }

    // ValidationProblemDetailsの "errors"
    public Dictionary<string, string[]>? Errors { get; set; }
}

public sealed class TaskDetailResponse
{
    public int TaskId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";

    // もしAPIが返すなら（不要なら消してOK）
    public int ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class TaskListItemResponse
{
    public int TaskId { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
}