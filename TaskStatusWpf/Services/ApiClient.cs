using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskStatusWpf.Models;

namespace TaskStatusWpf.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;

    public ApiClient(string baseUrl)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
        };

        _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // token / Token どちらでも吸収
        };
    }

    public void SetBearerToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization = null;
            return;
        }
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var req = new AuthLoginRequest { Email = email, Password = password };
        var json = JsonSerializer.Serialize(req, _json);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var res = await _http.PostAsync("api/v1/auth/login", content, ct);
        if (!res.IsSuccessStatusCode)
            throw await ToApiExceptionAsync(res, ct);

        var body = await res.Content.ReadAsStringAsync(ct);
        var token = JsonSerializer.Deserialize<AuthTokenResponse>(body, _json)?.Token;
        if (string.IsNullOrWhiteSpace(token))
            throw new ApiException((int)res.StatusCode, "Login failed", "Token is missing in response.");

        return token!;
    }

    public async Task<MeResponse> GetMeAsync(CancellationToken ct = default)
    {
        using var res = await _http.GetAsync("api/v1/users/me", ct);
        if (!res.IsSuccessStatusCode)
            throw await ToApiExceptionAsync(res, ct);

        var body = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<MeResponse>(body, _json)
            ?? throw new ApiException((int)res.StatusCode, "Parse failed", "Failed to parse /users/me response.");
    }

    public async Task<IReadOnlyList<ProjectListItemResponse>> GetProjectsAsync(string? q = null, bool? archived = null, CancellationToken ct = default)
    {
        var qs = new List<string>();
        if (!string.IsNullOrWhiteSpace(q)) qs.Add("q=" + Uri.EscapeDataString(q));
        if (archived is not null) qs.Add("archived=" + (archived.Value ? "true" : "false"));

        var url = "api/v1/projects" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

        using var res = await _http.GetAsync(url, ct);
        if (!res.IsSuccessStatusCode)
            throw await ToApiExceptionAsync(res, ct);

        var body = await res.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<ProjectListItemResponse>>(body, _json)
            ?? new List<ProjectListItemResponse>();
    }

    private async Task<ApiException> ToApiExceptionAsync(HttpResponseMessage res, CancellationToken ct)
    {
        var status = (int)res.StatusCode;
        var raw = await res.Content.ReadAsStringAsync(ct);

        // まず ProblemDetails として読んでみる
        try
        {
            var pd = JsonSerializer.Deserialize<ProblemDetailsDto>(raw, _json);
            if (pd is not null)
            {
                // ValidationProblemDetails なら errors を優先整形
                if (pd.Errors is not null && pd.Errors.Count > 0)
                {
                    var lines = pd.Errors
                        .SelectMany(kv => kv.Value.Select(v => $"{kv.Key}: {v}"))
                        .ToArray();
                    var detail = string.Join(Environment.NewLine, lines);
                    return new ApiException(status, pd.Title ?? "Validation failed", detail, raw);
                }

                return new ApiException(
                    status,
                    pd.Title ?? "Request failed",
                    pd.Detail ?? $"HTTP {status}",
                    raw
                );
            }
        }
        catch
        {
            // 解析できない場合は raw をそのまま
        }

        return new ApiException(status, "Request failed", $"HTTP {status}", raw);
    }
}

public sealed class ApiException : Exception
{
    public int StatusCode { get; }
    public string? RawBody { get; }

    public ApiException(int statusCode, string title, string detail, string? rawBody = null)
        : base($"{title}: {detail}")
    {
        StatusCode = statusCode;
        RawBody = rawBody;
    }
}
