using ChatApp.Application.Dto;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<PagedResponse<Message>?> GetAllPagedMessagesByChatIdAsync(int chatId,int page, int size);
}