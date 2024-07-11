using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IChatRepository : IGenericRepository<Chat>
{
    Task<Chat?> GetChatWithMessagesByIdAsync(int id);
    Task<IEnumerable<Chat>?> GetAllChatsWithMessagesAsync();
}