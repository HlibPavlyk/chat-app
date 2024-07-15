using System.Collections.Concurrent;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Infrastructure.Repositories;

public class ChatHubRepository : IChatHubRepository
{
    private readonly IHubContext<ChatHub> _hubContext;
    private static ConcurrentDictionary<string, (string, int)> _userConnections = new();

    public ChatHubRepository(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task JoinChatAsync(string connectionId, string username, int chatId)
    {
        if (_userConnections.TryGetValue(username, out _))
        {
            throw new InvalidOperationException("User is already in chat");
        }

        if (!_userConnections.TryAdd(username, (connectionId, chatId)))
        {
            _userConnections[username] = (connectionId, chatId);
        }
        await _hubContext.Groups.AddToGroupAsync(connectionId, chatId.ToString());
        await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", username + $" joined {chatId} chat");
    }

    public async Task LeaveChatAsync(string connectionId, string username)
    {
        if (_userConnections.TryRemove(username, out var userConnection) && userConnection.Item1.Equals(connectionId))
        {
            await _hubContext.Clients.Group(userConnection.Item2.ToString())
                .SendAsync("ReceiveMessage", username + $" left {userConnection.Item2} chat");
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, userConnection.Item2.ToString());
        }
        else
        {
            throw new InvalidOperationException("You are not in any chats. You should join a chat first");
        }
    }

    public async Task<int> SendMessageAsync(string username, string message)
    {
        if (!_userConnections.TryGetValue(username, out var userConnection))
        {
            throw new InvalidOperationException("You cannot send messages. You should join a chat first");
        }
        
        await _hubContext.Clients.Group(userConnection.Item2.ToString())
            .SendAsync("ReceiveMessage", $"{username}: {message}");
        return userConnection.Item2;
    }
    
    public async Task RemoveAllUsersFromGroupAsync(int chatId)
    {
        var usersInGroup = _userConnections.Where(kvp => kvp.Value.Item2 == chatId ).ToList();
    
        if (!usersInGroup.Any())
        {
            throw new InvalidOperationException("There are no users in the chat");
        }

        foreach (var user in usersInGroup)
        {
            if (_userConnections.TryRemove(user.Key, out _))
            {
                await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", $"{user.Key} has been removed from the chat.");
                await _hubContext.Groups.RemoveFromGroupAsync(user.Value.Item1, chatId.ToString());
            }
        }
    }
}