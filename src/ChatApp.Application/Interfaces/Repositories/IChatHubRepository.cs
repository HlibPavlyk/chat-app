namespace ChatApp.Application.Interfaces.Repositories;

public interface IChatHubRepository
{
    Task JoinChatAsync(string connectionId, string username, int chatId);
    Task LeaveChatAsync(string connectionId, string username);
    Task<int> SendMessageAsync(string username, string message);
    Task RemoveAllUsersFromGroupAsync(int chatId);
}