using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Infrastructure.Hubs;
using ChatApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.IntegrationTests;

public class ChatHubIntegrationTests
{
    private readonly HubConnection _connection;

    public ChatHubIntegrationTests()
    {
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSignalR();
                services.AddSingleton<IChatHubRepository, ChatHubRepository>();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<ChatHub>("/chatHub");
                });
            });
        
        var server = new TestServer(webHostBuilder);
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost/chatHub", options =>
            {
                options.HttpMessageHandlerFactory = _ => server.CreateHandler();
            })
            .Build();
    }

    [Fact]
    public async Task ChatFlowTest()
    {
        var joinMessageReceived = false;
        var username = "testUser";
        var chatId = 1;
        _connection.On<string>("ReceiveMessage", message =>
        {
            if (message == username + $" joined {chatId} chat")
            {
                joinMessageReceived = true;
            }
        });
        await _connection.StartAsync();
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("JoinChat", username, chatId);
            await Task.Delay(500);
        }
        Assert.True(joinMessageReceived);

        var messageReceived = false;
        var testMessage = "Hello World";
        _connection.On<string>("ReceiveMessage", message =>
        {
            if (message == $"{username}: {testMessage}")
            {
                messageReceived = true;
            }
        });
        await _connection.InvokeAsync("SendMessage", username, testMessage);
        await Task.Delay(500);
        Assert.True(messageReceived);

        var leaveMessageReceived = false;
        _connection.On<string>("ReceiveMessage", message =>
        {
            if (message == username + $" left {chatId} chat")
            {
                leaveMessageReceived = true;
            }
        });
        await _connection.InvokeAsync("LeaveChat", username);
        await Task.Delay(500);
        Assert.True(leaveMessageReceived);

        await _connection.StopAsync();
    }
    
    [Fact]
    public async Task JoinMultipleChats_ReceiveErrorMessage()
    {
        bool errorMessageReceived = false;
        var username = "testUser";
        var chatId1 = 2;
        var chatId2 = 3;
        await _connection.StartAsync();
        
        _connection.On<string>("ReceiveError", _ =>
        {
            errorMessageReceived = true;
        });

        await _connection.InvokeAsync("JoinChat", username, chatId1);
        await Task.Delay(500);

        await _connection.InvokeAsync("JoinChat", username, chatId2);
        await Task.Delay(500);

        Assert.True(errorMessageReceived);

        await _connection.StopAsync();
    }
    
    [Fact]
    public async Task RemoveAllUsersFromChat_ClearsChat()
    {
        var chatId = 4;
        var username1 = "testUser1";
        var username2 = "testUser2";
        var testMessage = "Hello World";
        await _connection.StartAsync();

        await _connection.InvokeAsync("JoinChat", username1, chatId);
        await Task.Delay(500);
        await _connection.InvokeAsync("JoinChat", username2, chatId);
        await Task.Delay(500);

        await _connection.InvokeAsync("RemoveAllUsersFromChat", chatId);
        await Task.Delay(500);

        bool messageReceived = false;
        _connection.On<string>("ReceiveMessage", _ =>
        {
            messageReceived = true;
        });
        
        await _connection.InvokeAsync("SendMessage", username1, testMessage);
        await Task.Delay(500);
        await _connection.InvokeAsync("SendMessage", username2, testMessage);
        await Task.Delay(500);

        Assert.False(messageReceived, "No messages should be received as all users are removed from the chat");

        await _connection.StopAsync();
    }
}