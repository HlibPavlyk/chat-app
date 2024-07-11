﻿using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChatApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IChatRepository Chats { get; }
    public IMessageRepository Messages { get; }
   
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Chats = new ChatRepository(_context);
        Messages = new MessageRepository(_context);
    }

    public async Task CompleteAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message + " An entity with the same title may already exist.");
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}