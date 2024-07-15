using ChatApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.SignalR;


namespace ChatApp.Infrastructure.Hubs;

public sealed class ChatHub : Hub
{
    private readonly IChatHubRepository _chatService;

    public ChatHub(IChatHubRepository chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinChat(string username, int chatId)
    {
        try
        {
            await _chatService.JoinChatAsync(Context.ConnectionId, username, chatId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", ex.Message);
        }
    }

    public async Task LeaveChat(string username)
    {
        try
        {
            await _chatService.LeaveChatAsync(Context.ConnectionId, username);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", ex.Message);
        }
    }

    public async Task SendMessage(string username, string message)
    {
        try
        {
            await _chatService.SendMessageAsync(username, message);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", ex.Message);
        }
    }
    
    public async Task RemoveAllUsersFromChat(int chatId)
    {
        try
        {
            await _chatService.RemoveAllUsersFromGroupAsync(chatId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", ex.Message);
        }
    }
}