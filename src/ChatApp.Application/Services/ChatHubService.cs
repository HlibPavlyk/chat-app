using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services;

public class ChatHubService : IChatHubService
{
    private readonly IChatHubRepository _chatHubRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChatHubService(IChatHubRepository chatHubRepository, IUnitOfWork unitOfWork)
    {
        _chatHubRepository = chatHubRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SendMessageAsync(SenderDto sender)
    {
        var chatId = await _chatHubRepository.SendMessageAsync(sender.Username, sender.Message);
        
        await _unitOfWork.Messages.AddAsync(new Message
        {
            ChatId = chatId,
            Author = sender.Username,
            Text = sender.Message,
            Timestamp = DateTime.Now
        });
        await _unitOfWork.CompleteAsync();
    }

    public async Task JoinChatAsync(int chatId, UserDto user )
    {
        var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);
        if (chat is null)
        {
            throw new ArgumentNullException(nameof(chat), "Chat does not exist");
        }
        
        await _chatHubRepository.JoinChatAsync(user.Username.GetHashCode().ToString(), user.Username, chatId);
    }

    public async Task LeaveChatAsync(UserDto user)
    {
        await _chatHubRepository.LeaveChatAsync(user.Username.GetHashCode().ToString(), user.Username);
    }
    
}