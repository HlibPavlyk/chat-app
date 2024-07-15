using ChatApp.Application.Dto;

namespace ChatApp.Application.Interfaces.Services;

public interface IChatHubService
{
    Task SendMessageAsync(SenderDto sender);
    Task JoinChatAsync(int chatId, UserDto user );
    Task LeaveChatAsync(UserDto user);
}