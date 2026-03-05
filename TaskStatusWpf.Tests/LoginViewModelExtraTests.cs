using System;
using TaskStatusWpf.Services;
using TaskStatusWpf.Tests.Fakes;
using TaskStatusWpf.ViewModels;
using Xunit;

namespace TaskStatusWpf.Tests;

public sealed class LoginViewModelExtraTests
{
    [Fact]
    public async Task Login_Failure_Should_Reset_IsBusy()
    {
        var api = new FakeApiClient
        {
            LoginAsyncImpl = (_, _) => throw new Exception("boom")
        };
        var session = new AppSession();
        var vm = new LoginViewModel(api, session)
        {
            Email = "a@b.com",
            Password = "pw"
        };

        Assert.False(vm.IsBusy);

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.False(vm.IsBusy);      // ✅ finallyで戻る
        Assert.True(vm.HasError);
        Assert.False(vm.IsNotBusy == false); // IsNotBusy も正しく更新される想定
    }

    [Fact]
    public async Task Login_Success_Should_Clear_ErrorMessage()
    {
        var api = new FakeApiClient();
        var session = new AppSession();
        var vm = new LoginViewModel(api, session)
        {
            Email = "a@b.com",
            Password = "pw"
        };

        // 事前にエラーが入ってても成功で消えることを確認
        vm.ErrorMessage = "dummy error";
        Assert.True(vm.HasError);

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.False(vm.HasError);
        Assert.Null(vm.ErrorMessage);
    }
}
