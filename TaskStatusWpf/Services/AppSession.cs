using TaskStatusWpf.Models;

namespace TaskStatusWpf.Services;

public sealed class AppSession
{
    public string? Token { get; private set; }
    public MeResponse? Me { get; private set; }

    public void SetToken(string token) => Token = token;
    public void SetMe(MeResponse me) => Me = me;

    // ✅ 追加：ログアウト用
    public void Clear()
    {
        Token = null;
        Me = null;
    }

    public bool IsLeader =>
        string.Equals(Me?.Role, "Leader", StringComparison.OrdinalIgnoreCase);
}