using TaskStatusWpf.Models;
using Xunit;

namespace TaskStatusWpf.Tests;

public sealed class SmokeTests
{
    [Fact]
    public void Can_Reference_Main_Project_Types()
    {
        var me = new MeResponse();
        Assert.NotNull(me);
    }
}