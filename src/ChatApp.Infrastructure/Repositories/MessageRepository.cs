using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context) {}

    public async Task<Message?> GetMessageWithChatByIdAsync(int id)
    {
        return await _context.Messages
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PagedResponse<Message>?> GetAllPagedMessagesWithChatAsync(int page, int size)
    {
        var items = await _context.Messages
            .Include(m => m.Chat)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalItems = await _context.Messages.CountAsync();
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