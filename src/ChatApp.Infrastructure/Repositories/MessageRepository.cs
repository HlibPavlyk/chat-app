using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context) {}

    public async Task<PagedResponse<Message>?> GetAllPagedMessagesByChatIdAsync(int chatId, int page, int size)
    {
        var items = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalItems = items.Count();
        if (totalItems is 0)
        {
            return null;
        }

        var totalPages = (int)Math.Ceiling(totalItems / (double)size);
        return new PagedResponse<Message>
        {
            Items = items,
            TotalPages = totalPages
        };
    }
}