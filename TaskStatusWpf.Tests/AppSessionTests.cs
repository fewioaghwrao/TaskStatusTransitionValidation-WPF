using TaskStatusWpf.Models;
using TaskStatusWpf.Services;
using Xunit;

namespace TaskStatusWpf.Tests;

public sealed class AppSessionTests
{
    [Fact]
    public void Clear_Should_Reset_Token_And_Me()
    {
        var s = new AppSession();
        s.SetToken("abc");
        s.SetMe(new MeResponse { Role = "Leader" });

        s.Clear();

        Assert.Null(s.Token);
        Assert.Null(s.Me);
    }

    [Theory]
    [InlineData("Leader", true)]
    [InlineData("leader", true)]
    [InlineData("Worker", false)]
    [InlineData(null, false)]
    public void IsLeader_Should_Work(string? role, bool expected)
    {
        var s = new AppSession();
        if (role is not null)
            s.SetMe(new MeResponse { Role = role });

        Assert.Equal(expected, s.IsLeader);
    }

    [Fact]
    public void IsLeader_Should_Be_CaseInsensitive()
    {
        var s = new AppSession();
        s.SetMe(new MeResponse { Role = "lEaDeR" });

        Assert.True(s.IsLeader);
    }
}