using System.Collections;
using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Services;
using ChatApp.Domain.Entities;
using Moq;

namespace ChatApp.UnitTests.Services;

public class ChatServiceUnitTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ChatService _chatService;

    public ChatServiceUnitTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        Mock<IChatHubRepository> mockChatHubRepository = new();
        _chatService = new ChatService(_mockUnitOfWork.Object, mockChatHubRepository.Object);
    }

    [Fact]
    public async Task CreateChatAsync_CreatesChatSuccessfully_ReturnsChatWithDetails()
    {
        var chatPostDto = new ChatPostDto { Title = "Test Chat", Author = "Author" };
        var expectedChat = new Chat { Id = 1, Title = "Test Chat", Author = "Author", Messages = new List<Message>() };
        var pagedMessages = new PagedResponse<Message>
        {
            Items = new List<Message> { new Message { Id = 1, Author = "Author", Text = "Hello", Timestamp = DateTime.Now } },
            TotalPages = 1
        };

        _mockUnitOfWork.Setup(u => u.Chats.AddAsync(It.IsAny<Chat>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedChat);
        _mockUnitOfWork.Setup(u => u.Messages.GetAllPagedMessagesByChatIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedMessages);
        
        var result = await _chatService.CreateChatAsync(chatPostDto);

        Assert.NotNull(result);
        Assert.Equal(expectedChat.Title, result.Title);
        Assert.Equal(expectedChat.Author, result.Author);
    }

    [Fact]
    public async Task GetChatWithPagedMessagesByIdAsync_ChatExists_ReturnsChatWithMessages()
    {
        var chatId = 1;
        var chat = new Chat { Id = chatId, Title = "Test Chat", Author = "Author", Messages = new List<Message>() };
        var pagedMessages = new PagedResponse<Message>
        {
            Items = new List<Message> { new Message { Id = 1, Author = "Author", Text = "Hello", Timestamp = DateTime.Now } },
            TotalPages = 1
        };

        _mockUnitOfWork.Setup(u => u.Chats.GetByIdAsync(chatId)).ReturnsAsync(chat);
        _mockUnitOfWork.Setup(u => u.Messages.GetAllPagedMessagesByChatIdAsync(chatId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagedMessages);

        var result = await _chatService.GetChatWithPagedMessagesByIdAsync(chatId, 1, 10);

        Assert.NotNull(result);
        Assert.Equal(chat.Title, result.Title);
        if (result.Messages != null) 
            Assert.Single((IEnumerable)result.Messages.Items);
    }

    [Fact]
    public async Task GetChatWithPagedMessagesByIdAsync_ChatDoesNotExist_ThrowsArgumentNullException()
    {
        _mockUnitOfWork.Setup(u => u.Chats.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(value: null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => _chatService.GetChatWithPagedMessagesByIdAsync(999, 1, 10));
    }

    [Fact]
    public async Task DeleteChatAsync_UserIsAuthor_DeletesChat()
    {
        var chat = new Chat { Id = 1, Title = "Test Chat", Author = "Author" };
        var user = new UserDto { Username = "Author" };

        _mockUnitOfWork.Setup(u => u.Chats.GetByIdAsync(chat.Id)).ReturnsAsync(chat);
        _mockUnitOfWork.Setup(u => u.Chats.Remove(It.IsAny<Chat>()));
        _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

        await _chatService.DeleteChatAsync(chat.Id, user);

        _mockUnitOfWork.Verify(u => u.Chats.Remove(It.Is<Chat>(c => c.Id == chat.Id)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteChatAsync_UserIsNotAuthor_ThrowsUnauthorizedAccessException()
    {
        var chat = new Chat { Id = 1, Title = "Test Chat", Author = "Author" };
        var user = new UserDto { Username = "NotAuthor" };

        _mockUnitOfWork.Setup(u => u.Chats.GetByIdAsync(chat.Id)).ReturnsAsync(chat);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _chatService.DeleteChatAsync(chat.Id, user));
    }
}