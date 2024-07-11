using ChatApp.Application.Dto;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IChatRepository : IGenericRepository<Chat>
{
    Task<Chat?> GetChatWithMessagesByIdAsync(int id);
    Task<PagedResponse<Chat>?> GetAllPagedChatsWithSearchAsync(string? title, int page, int size);
}