using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
        
    public ChatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ChatWithMessagesGetDto> CreateChatAsync(ChatPostDto chat)
    {
        var newChat = new Chat
        {
            Title = chat.Title,
            Author = chat.Author,
            Messages = new List<Message>()
        };

        await _unitOfWork.Chats.AddAsync(newChat);
        await _unitOfWork.CompleteAsync();

        return await GetChatByIdAsync(newChat.Id);
    }

    public async Task<ChatWithMessagesGetDto> GetChatByIdAsync(int id)
    {
        var chat = await _unitOfWork.Chats.GetChatWithMessagesByIdAsync(id);
        
        if (chat is null)
        {
            throw new ArgumentNullException(nameof(chat), "Chat not found");
        }
        
        var chatDto = new ChatWithMessagesGetDto
        {
            Id = chat.Id,
            Title = chat.Title,
            Author = chat.Author,
            Messages = chat.Messages.Select(m => new MessageGetDto
            {
                Id = m.Id,
                Author = m.Author,
                Text = m.Text,
                Timestamp = m.Timestamp
            }).ToList()
        };
        
        return chatDto;
    }

    public async Task<PagedResponse<ChatGetDto>?> GetChatsWithSearchAsync(string? title, int pageNumber, int pageSize)
    {
        var chats = await _unitOfWork.Chats.GetAllPagedChatsWithSearchAsync(title, pageNumber, pageSize);
        if (chats is null)
        {
            throw new ArgumentNullException(nameof(chats), "No chats found");
        }
        
        var responseItems = chats.Items.Select(c => new ChatGetDto
        {
            Id = c.Id,
            Title = c.Title,
            Author = c.Author,
            MessageAmount = c.Messages.Count
        }).ToList();
        
        var response = new PagedResponse<ChatGetDto>
        {
            Items = responseItems,
            TotalPages = chats.TotalPages
        };

        return response;
    }

    public async Task DeleteChatAsync(int id, UserDto user)
    {
        var chat = await _unitOfWork.Chats.GetByIdAsync(id);
        
        if (chat is null)
        {
            throw new ArgumentNullException(nameof(chat), "Chat not found");
        }
        
        if (user.Username != chat.Author)
        {
            throw new UnauthorizedAccessException("You are not the author of this chat");
        }
        
        _unitOfWork.Chats.Remove(chat);
        await _unitOfWork.CompleteAsync();
       
    }
}