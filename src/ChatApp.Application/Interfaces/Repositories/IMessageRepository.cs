using ChatApp.Application.Dto;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<Message?> GetMessageWithChatByIdAsync(int id);
    Task<PagedResponse<Message>?> GetAllPagedMessagesWithChatAsync(int page, int size);
}