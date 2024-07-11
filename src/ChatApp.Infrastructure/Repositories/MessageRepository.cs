using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Message?> GetMessageWithChatByIdAsync(int id)
    {
        return await _context.Messages
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Message>?> GetAllMessagesWithChatAsync()
    {
        return await _context.Messages
            .Include(m => m.Chat)
            .ToListAsync();
    }
}