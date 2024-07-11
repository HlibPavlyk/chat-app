using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class ChatRepository : GenericRepository<Chat>, IChatRepository
{
    public ChatRepository(ApplicationDbContext context) : base(context) {}

    public async Task<Chat?> GetChatWithMessagesByIdAsync(int id)
    {
        return await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagedResponse<Chat>?> GetAllPagedChatsWithMessagesAsync(int page, int size)
    {
        var items = await _context.Chats
            .Include(c => c.Messages)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalItems = await _context.Messages.CountAsync();
        if (totalItems is 0)
        {
            return null;
        }

        var totalPages = (int)Math.Ceiling(totalItems / (double)size);
        return new PagedResponse<Chat>
        {
            Items = items,
            TotalPages = totalPages
        };
    }
}