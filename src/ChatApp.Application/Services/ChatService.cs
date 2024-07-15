using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChatHubRepository _chatHubRepository;
    private readonly int _defaultPageSize = 10;
    private readonly int _defaultPageNumber = 1;
    
    public ChatService(IUnitOfWork unitOfWork, IChatHubRepository chatHubRepository)
    {
        _unitOfWork = unitOfWork;
        _chatHubRepository = chatHubRepository;
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

        return await GetChatWithPagedMessagesByIdAsync(newChat.Id, _defaultPageNumber, _defaultPageSize);
    }

    public async Task<ChatWithMessagesGetDto> GetChatWithPagedMessagesByIdAsync(int id, int messagePageNumber, int messagePageSize)
    {
        var chat = await _unitOfWork.Chats.GetByIdAsync(id);
        if (chat is null)
        {
            throw new ArgumentNullException(nameof(chat), "Chat not found");
        }
        
        var pagedChatMessages = await _unitOfWork.Messages.GetAllPagedMessagesByChatIdAsync(id, messagePageNumber, messagePageSize);
        
        var chatDto = new ChatWithMessagesGetDto
        {
            Id = chat.Id,
            Title = chat.Title,
            Author = chat.Author,
            Messages = pagedChatMessages is null ? null : new PagedResponse<MessageGetDto>
            {
                Items = pagedChatMessages.Items.Select(m => new MessageGetDto
                {
                    Id = m.Id,
                    Author = m.Author,
                    Text = m.Text,
                    Timestamp = m.Timestamp
                }).ToList(),
                TotalPages = pagedChatMessages.TotalPages
                }
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

        await _chatHubRepository.RemoveAllUsersFromGroupAsync(chat.Id);
        
        _unitOfWork.Chats.Remove(chat);
        await _unitOfWork.CompleteAsync();
       
    }
}