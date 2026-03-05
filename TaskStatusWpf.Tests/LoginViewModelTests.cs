using System;
using TaskStatusWpf.Services;
using TaskStatusWpf.Tests.Fakes;
using TaskStatusWpf.ViewModels;
using Xunit;

namespace TaskStatusWpf.Tests;

public sealed class LoginViewModelTests
{
    [Fact]
    public void CanLogin_Should_BeFalse_When_Empty()
    {
        var api = new FakeApiClient();
        var session = new AppSession();
        var vm = new LoginViewModel(api, session);

        Assert.False(vm.CanLogin);
        Assert.False(vm.LoginCommand.CanExecute(null));
    }

    [Fact]
    public void CanLogin_Should_BeTrue_When_Email_And_Password_Are_Set()
    {
        var api = new FakeApiClient();
        var session = new AppSession();
        var vm = new LoginViewModel(api, session);

        vm.Email = "a@b.com";
        vm.Password = "pw";

        Assert.True(vm.CanLogin);
        Assert.True(vm.LoginCommand.CanExecute(null));
    }

    [Fact]
    public async Task Login_Success_Should_SetSession_And_RaiseEvent()
    {
        var api = new FakeApiClient();
        var session = new AppSession();
        var vm = new LoginViewModel(api, session)
        {
            Email = "a@b.com",
            Password = "pw"
        };

        var raised = false;
        vm.LoginSucceeded += () => raised = true;

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.True(raised);
        Assert.Equal("token", session.Token);
        Assert.NotNull(session.Me);
        Assert.Equal("Leader", session.Me!.Role);
        Assert.Equal("token", api.LastBearerToken);

        Assert.False(vm.IsBusy);
        Assert.False(vm.HasError);
        Assert.Null(vm.ErrorMessage);
    }

    [Fact]
    public async Task Login_Failure_Should_SetError_And_NotSetSession()
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

        await vm.LoginCommand.ExecuteAsync(null);

        Assert.Null(session.Token);
        Assert.Null(session.Me);

        Assert.False(vm.IsBusy);
        Assert.True(vm.HasError);
        Assert.Equal("ログインに失敗しました。メールアドレスまたはパスワードを確認してください。", vm.ErrorMessage);
    }
}