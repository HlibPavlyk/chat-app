using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
   
    public async Task<PagedResponse<Chat>?> GetAllPagedChatsWithSearchAsync(string? title, int page, int size)
    {
        var query =  _context.Chats
            .Include(c => c.Messages)
            .AsQueryable();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(c => c.Title.Contains(title));
        }
        
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalItems = await query.CountAsync();
        if (items.IsNullOrEmpty() || totalItems is 0)
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