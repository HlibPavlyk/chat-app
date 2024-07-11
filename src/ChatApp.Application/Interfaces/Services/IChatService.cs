using ChatApp.Application.Dto;

namespace ChatApp.Application.Interfaces.Services;

public interface IChatService
{
    Task<ChatWithMessagesGetDto> CreateChatAsync(ChatPostDto chat);
    Task<ChatWithMessagesGetDto> GetChatByIdAsync(int id);
    Task<PagedResponse<ChatGetDto>?> GetChatsWithSearchAsync(string? title, int pageNumber, int pageSize);
    Task DeleteChatAsync(int id, UserDto user);
}