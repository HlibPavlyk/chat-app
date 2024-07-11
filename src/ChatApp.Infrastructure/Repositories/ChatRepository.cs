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

    public async Task<IEnumerable<Chat>?> GetAllChatsWithMessagesAsync()
    {
        return await _context.Chats
            .Include(c => c.Messages)
            .ToListAsync();
    }
}