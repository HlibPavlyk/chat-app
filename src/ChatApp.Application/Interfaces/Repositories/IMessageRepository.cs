using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetMessageWithChatByIdAsync(int id);
    Task<IEnumerable<Message>?> GetAllMessagesWithChatAsync();
}