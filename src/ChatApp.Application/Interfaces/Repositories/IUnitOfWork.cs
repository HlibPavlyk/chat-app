using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IChatRepository Chats { get; }
    IMessageRepository Messages { get; }
    Task CompleteAsync();
}